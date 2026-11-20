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
        private CancellationToken _seen;

        public CancellationToken GetSeen() => _seen;

        public async IAsyncEnumerable<object> Items([EnumeratorCancellation] CancellationToken token = default)
        {
            _seen = token;
            await Task.Yield();
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
        }

        public async IAsyncEnumerable<int> IntItems([EnumeratorCancellation] CancellationToken token = default)
        {
            _seen = token;
            await Task.Yield();
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
        }
    }



    private sealed class TokenHolder
    {
        private readonly CancellationToken _token;

        public TokenHolder(CancellationToken token) => _token = token;

        public CancellationToken Get() => _token;
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
        "Where-sync" => new WhereTransformer<int>(i => i > 0).TransformAsync(s.IntItems()).Box(),
        "Where-async" => new WhereTransformer<int>(i => new ValueTask<bool>(i > 0)).TransformAsync(s.IntItems()).Box(),
        "Select-sync" => new SelectTransformer<int, int>(i => i).TransformAsync(s.IntItems()).Box(),
        "Select-async" => new SelectTransformer<int, int>(i => new ValueTask<int>(i)).TransformAsync(s.IntItems()).Box(),
        "SelectMany-sync" => new SelectManyTransformer<int, int>(i => new[] { i }).TransformAsync(s.IntItems()).Box(),
        "SelectMany-async" => new SelectManyTransformer<int, int>(i => ToAsync(new[] { i })).TransformAsync(s.IntItems()).Box(),
        "Skip" => new SkipTransformer<int>(1).TransformAsync(s.IntItems()).Box(),
        "Take" => new TakeTransformer<int>(3).TransformAsync(s.IntItems()).Box(),
        "SkipWhile-sync" => new SkipWhileTransformer<int>(i => i < 2).TransformAsync(s.IntItems()).Box(),
        "SkipWhile-async" => new SkipWhileTransformer<int>(i => new ValueTask<bool>(i < 2)).TransformAsync(s.IntItems()).Box(),
        "TakeWhile-sync" => new TakeWhileTransformer<int>(i => i < 3).TransformAsync(s.IntItems()).Box(),
        "TakeWhile-async" => new TakeWhileTransformer<int>(i => new ValueTask<bool>(i < 3)).TransformAsync(s.IntItems()).Box(),
        "Distinct" => new DistinctTransformer<int>().TransformAsync(s.IntItems()).Box(),
        "DistinctBy" => new DistinctByTransformer<int, int>(i => i).TransformAsync(s.IntItems()).Box(),
        "Cast" => new CastTransformer<object, int>().TransformAsync(s.Items()).Box(),
        "OfType" => new OfTypeTransformer<object, int>().TransformAsync(s.Items()).Box(),
        "Chunk" => new ChunkTransformer<int>(2).TransformAsync(s.IntItems()).Box(),
        "ProgressReporting-sync" => new ProgressReportingTransformer<int>(_ => { }).TransformAsync(s.IntItems()).Box(),
        _ => new ProgressReportingTransformer<int>(_ => default).TransformAsync(s.IntItems()).Box(), // "ProgressReporting-async"
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

        Assert.Equal(cts.Token, source.GetSeen());
    }




    public static TheoryData<string> TokenOverloadCases() => new()
    {
        "Where-sync", "Where-async", "Select-sync", "Select-async",
        "SelectMany-sync", "SelectMany-async",
        "Skip", "Skip-zero", "Take", "SkipWhile-sync", "SkipWhile-async", "TakeWhile-sync", "TakeWhile-async",
        "Distinct", "DistinctBy", "Cast", "OfType", "Chunk",
        "ProgressReporting-sync", "ProgressReporting-async",
    };



    private static IAsyncEnumerable<object> BuildWithToken(string name, TokenRecordingSource s, TokenHolder token) => name switch
    {
        "Where-sync" => new WhereTransformer<int>(i => i > 0).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Where-async" => new WhereTransformer<int>(i => new ValueTask<bool>(i > 0)).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Select-sync" => new SelectTransformer<int, int>(i => i).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Select-async" => new SelectTransformer<int, int>(i => new ValueTask<int>(i)).TransformAsync(s.IntItems(), token.Get()).Box(),
        "SelectMany-sync" => new SelectManyTransformer<int, int>(i => new[] { i }).TransformAsync(s.IntItems(), token.Get()).Box(),
        "SelectMany-async" => new SelectManyTransformer<int, int>(i => ToAsync(new[] { i })).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Skip" => new SkipTransformer<int>(1).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Skip-zero" => new SkipTransformer<int>(0).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Take" => new TakeTransformer<int>(3).TransformAsync(s.IntItems(), token.Get()).Box(),
        "SkipWhile-sync" => new SkipWhileTransformer<int>(i => i < 2).TransformAsync(s.IntItems(), token.Get()).Box(),
        "SkipWhile-async" => new SkipWhileTransformer<int>(i => new ValueTask<bool>(i < 2)).TransformAsync(s.IntItems(), token.Get()).Box(),
        "TakeWhile-sync" => new TakeWhileTransformer<int>(i => i < 3).TransformAsync(s.IntItems(), token.Get()).Box(),
        "TakeWhile-async" => new TakeWhileTransformer<int>(i => new ValueTask<bool>(i < 3)).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Distinct" => new DistinctTransformer<int>().TransformAsync(s.IntItems(), token.Get()).Box(),
        "DistinctBy" => new DistinctByTransformer<int, int>(i => i).TransformAsync(s.IntItems(), token.Get()).Box(),
        "Cast" => new CastTransformer<object, int>().TransformAsync(s.Items(), token.Get()).Box(),
        "OfType" => new OfTypeTransformer<object, int>().TransformAsync(s.Items(), token.Get()).Box(),
        "Chunk" => new ChunkTransformer<int>(2).TransformAsync(s.IntItems(), token.Get()).Box(),
        "ProgressReporting-sync" => new ProgressReportingTransformer<int>(_ => { }).TransformAsync(s.IntItems(), token.Get()).Box(),
        _ => new ProgressReportingTransformer<int>(_ => default(ValueTask)).TransformAsync(s.IntItems(), token.Get()).Box(), // "ProgressReporting-async"
    };



    [Theory]
    [MemberData(nameof(TokenOverloadCases))]
    public async Task TransformAsync_with_a_token_forwards_it_to_the_source(string name)
    {
        using var cts = new CancellationTokenSource();
        var source = new TokenRecordingSource();

        await foreach (var _ in BuildWithToken(name, source, new TokenHolder(cts.Token)))
        {
        }

        Assert.Equal(cts.Token, source.GetSeen());
    }


    [Fact]
    public async Task Then_when_both_sides_do_real_work_composes_cancellation_aware_and_forwards_the_token()
    {
        using var cts = new CancellationTokenSource();
        var holder = new TokenHolder(cts.Token);
        var source = new TokenRecordingSource();
        var chain = new WhereTransformer<int>(i => i > 1).Then(new SelectTransformer<int, int>(i => i * 10));

        var result = await CollectAsync(chain.TransformAsync(source.IntItems(), holder.Get()));

        Assert.Equal(new[] { 20, 30, 40 }, result);
        Assert.Equal(cts.Token, source.GetSeen());
    }


    [Fact]
    public async Task Buffered_TransformAsync_with_a_token_cancels_promptly_when_the_source_is_stalled()
    {
        using var cts = new CancellationTokenSource();
        var holder = new TokenHolder(cts.Token);
        var sut = new BufferedTransformer<int>(4);

        static async IAsyncEnumerable<int> Stalled([EnumeratorCancellation] CancellationToken token = default)
        {
            yield return 1;

            try { await Task.Delay(TimeSpan.FromSeconds(30), token); } catch (OperationCanceledException) { }
        }

        cts.CancelAfter(200);
        var consume = Task.Run
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(Stalled(), holder.Get()))
                {
                }
            }
        );

        var finished = await Task.WhenAny(consume, Task.Delay(TimeSpan.FromSeconds(10)));

        Assert.Same(consume, finished);
        Assert.True(EndedByCancellation(consume));
    }


    [Fact]
    public async Task Buffered_over_Where_cancels_promptly_when_the_source_is_stalled()
    {
        using var cts = new CancellationTokenSource();
        var sut = new BufferedTransformer<int>(4);
        var where = new WhereTransformer<int>(_ => true);

        // A source that stalls until the token fires, then ends quietly, so every line is reachable.
        static async IAsyncEnumerable<int> Stalled([EnumeratorCancellation] CancellationToken token = default)
        {
            yield return 1;

            try { await Task.Delay(TimeSpan.FromSeconds(30), token); } catch (OperationCanceledException) { }
        }

        var token = cts.Token;
        cts.CancelAfter(200);
        var watch = Stopwatch.StartNew();

        var consume = Task.Run
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(where.TransformAsync(Stalled())).WithCancellation(token))
                {
                }
            }
        );

        var finished = await Task.WhenAny(consume, Task.Delay(TimeSpan.FromSeconds(10)));

        Assert.Same(consume, finished);
        Assert.True(EndedByCancellation(consume));
        Assert.True(watch.Elapsed < TimeSpan.FromSeconds(10), $"took {watch.Elapsed}");
    }

    // The task has already completed (asserted by the caller), so inspect it rather than await it: an
    // OperationCanceledException thrown by an async lambda surfaces as a Canceled task.
    private static bool EndedByCancellation(Task finished)
    {
        return finished.IsCanceled || (finished.IsFaulted && finished.Exception?.GetBaseException() is OperationCanceledException);
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
