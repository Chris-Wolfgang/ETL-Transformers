using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// A token supplied with <c>.WithCancellation(token)</c> must reach the source through every
/// transformer, not stop at the first one.
/// </summary>
public class EnumeratorCancellationPropagationTests
{
    private sealed class TokenRecordingSource
    {
        public CancellationToken Seen { get; private set; }

        public async IAsyncEnumerable<object> Items([EnumeratorCancellation] CancellationToken token = default)
        {
            Seen = token;
            await Task.Yield();
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
        }

        public async IAsyncEnumerable<int> IntItems([EnumeratorCancellation] CancellationToken token = default)
        {
            Seen = token;
            await Task.Yield();
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
        }
    }



    public static TheoryData<string> Cases() => new()
    {
        "Where-sync", "Where-async", "Select-sync", "Select-async",
        "SelectMany-sync", "SelectMany-async",
        "Skip", "Take", "SkipWhile-sync", "SkipWhile-async", "TakeWhile-sync", "TakeWhile-async",
        "Distinct", "DistinctBy", "Cast", "OfType", "Chunk",
        "ProgressReporting-sync", "ProgressReporting-async",
    };



    private static IAsyncEnumerable<object> Build(string name, TokenRecordingSource s) => name switch
    {
        "Where-sync" => new WhereTransformer<int>((Func<int, bool>)(i => i > 0)).TransformAsync(s.IntItems()).Box(),
        "Where-async" => new WhereTransformer<int>((Func<int, ValueTask<bool>>)(i => new ValueTask<bool>(i > 0))).TransformAsync(s.IntItems()).Box(),
        "Select-sync" => new SelectTransformer<int, int>((Func<int, int>)(i => i)).TransformAsync(s.IntItems()).Box(),
        "Select-async" => new SelectTransformer<int, int>((Func<int, ValueTask<int>>)(i => new ValueTask<int>(i))).TransformAsync(s.IntItems()).Box(),
        "SelectMany-sync" => new SelectManyTransformer<int, int>((Func<int, IEnumerable<int>>)(i => new[] { i })).TransformAsync(s.IntItems()).Box(),
        "SelectMany-async" => new SelectManyTransformer<int, int>((Func<int, IAsyncEnumerable<int>>)(i => ToAsync(new[] { i }))).TransformAsync(s.IntItems()).Box(),
        "Skip" => new SkipTransformer<int>(1).TransformAsync(s.IntItems()).Box(),
        "Take" => new TakeTransformer<int>(3).TransformAsync(s.IntItems()).Box(),
        "SkipWhile-sync" => new SkipWhileTransformer<int>((Func<int, bool>)(i => i < 2)).TransformAsync(s.IntItems()).Box(),
        "SkipWhile-async" => new SkipWhileTransformer<int>((Func<int, ValueTask<bool>>)(i => new ValueTask<bool>(i < 2))).TransformAsync(s.IntItems()).Box(),
        "TakeWhile-sync" => new TakeWhileTransformer<int>((Func<int, bool>)(i => i < 3)).TransformAsync(s.IntItems()).Box(),
        "TakeWhile-async" => new TakeWhileTransformer<int>((Func<int, ValueTask<bool>>)(i => new ValueTask<bool>(i < 3))).TransformAsync(s.IntItems()).Box(),
        "Distinct" => new DistinctTransformer<int>().TransformAsync(s.IntItems()).Box(),
        "DistinctBy" => new DistinctByTransformer<int, int>(i => i).TransformAsync(s.IntItems()).Box(),
        "Cast" => new CastTransformer<object, int>().TransformAsync(s.Items()).Box(),
        "OfType" => new OfTypeTransformer<object, int>().TransformAsync(s.Items()).Box(),
        "Chunk" => new ChunkTransformer<int>(2).TransformAsync(s.IntItems()).Box(),
        "ProgressReporting-sync" => new ProgressReportingTransformer<int>((Action<int>)(_ => { })).TransformAsync(s.IntItems()).Box(),
        _ => new ProgressReportingTransformer<int>((Func<int, ValueTask>)(_ => default)).TransformAsync(s.IntItems()).Box(), // "ProgressReporting-async"
    };



    [Theory]
    [MemberData(nameof(Cases))]
    public async Task WithCancellation_token_reaches_the_source(string name)
    {
        using var cts = new CancellationTokenSource();
        var source = new TokenRecordingSource();

        await foreach (var _ in Build(name, source).WithCancellation(cts.Token))
        {
        }

        Assert.Equal(cts.Token, source.Seen);
    }



    [Fact]
    public async Task Buffered_over_Where_cancels_promptly_when_the_source_is_stalled()
    {
        using var cts = new CancellationTokenSource();
        var sut = new BufferedTransformer<int>(4);
        var where = new WhereTransformer<int>(_ => true);

        // A source that stalls until the token fires, then ends quietly, so every line is reachable.
        async IAsyncEnumerable<int> Stalled([EnumeratorCancellation] CancellationToken token = default)
        {
            yield return 1;

            try { await Task.Delay(TimeSpan.FromSeconds(30), token); } catch (OperationCanceledException) { yield break; }
        }

        cts.CancelAfter(200);
        var watch = Stopwatch.StartNew();

        // Whether the consumer sees an OperationCanceledException or a quietly truncated sequence
        // depends on the target framework's channel implementation; the property under test here is
        // that the stalled source is interrupted promptly, so completion in either form is accepted.
        var consume = Task.Run
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(where.TransformAsync(Stalled())).WithCancellation(cts.Token))
                {
                }
            }
        );

        var finished = await Task.WhenAny(consume, Task.Delay(TimeSpan.FromSeconds(10)));

        Assert.Same(consume, finished);
        Assert.True(watch.Elapsed < TimeSpan.FromSeconds(10), $"took {watch.Elapsed}");
    }
}



internal static class BoxExtensions
{
    internal static async IAsyncEnumerable<object> Box<T>(this IAsyncEnumerable<T> items, [EnumeratorCancellation] CancellationToken token = default)
    {
        await foreach (var item in items.WithCancellation(token).ConfigureAwait(false))
        {
            yield return item!;
        }
    }
}
