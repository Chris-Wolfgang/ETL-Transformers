using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// The token overload of every transformer observes the token itself, not only by handing it to
/// the source with <c>WithCancellation</c>. A source that ignores the token (a plain async iterator
/// with no <c>[EnumeratorCancellation]</c> parameter) must not be drained after cancellation
/// (issues #209, #813). Regression: remove the <c>token.ThrowIfCancellationRequested()</c> calls
/// from a transformer's iterator and its cases here complete normally instead of throwing.
/// </summary>
public class TokenObservedOverNonCooperativeSourceTests
{
    private const int SourceLength = 10;

    private const int CancelOnPull = 3;



    public static TheoryData<string> Cases() => new()
    {
        "Select-sync", "Select-async",
        "Where-sync", "Where-async",
        "SelectMany-sync", "SelectMany-async",
        "Cast", "OfType",
        "Distinct", "DistinctBy",
        "Skip", "SkipWhile-sync", "SkipWhile-async",
        "Take", "TakeWhile-sync", "TakeWhile-async",
        "Chunk",
        "ProgressReporting-sync", "ProgressReporting-async",
    };



    [Theory]
    [MemberData(nameof(Cases))]
    public async Task TransformAsync_when_token_is_pre_cancelled_pulls_zero_items_from_source(string name)
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var source = new TokenIgnoringSource(cancelOnPull: null, cts);

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => Build(name)(source.Enumerate(), cts.Token)
        );

        Assert.Equal(0, source.PullCount);
    }



    [Theory]
    [MemberData(nameof(Cases))]
    public async Task TransformAsync_when_token_is_cancelled_mid_run_stops_pulling_from_source(string name)
    {
        using var cts = new CancellationTokenSource();
        var source = new TokenIgnoringSource(CancelOnPull, cts);

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => Build(name)(source.Enumerate(), cts.Token)
        );

        Assert.Equal(CancelOnPull, source.PullCount);
    }



    private static Func<IAsyncEnumerable<int>, CancellationToken, Task> Drain<TOut>(ITransformWithCancellationAsync<int, TOut> sut)
        where TOut : notnull
        => async (items, token) =>
        {
            await foreach (var _ in sut.TransformAsync(items, token))
            {
            }
        };



    private static Func<IAsyncEnumerable<int>, CancellationToken, Task> Build(string name) => name switch // the final arm is "ProgressReporting-async"
    {
        "Select-sync" => Drain(new SelectTransformer<int, int>(i => i)),
        "Select-async" => Drain(new SelectTransformer<int, int>((Func<int, ValueTask<int>>)(i => new ValueTask<int>(i)))),
        "Where-sync" => Drain(new WhereTransformer<int>((Func<int, bool>)(_ => true))),
        "Where-async" => Drain(new WhereTransformer<int>((Func<int, ValueTask<bool>>)(_ => new ValueTask<bool>(true)))),
        "SelectMany-sync" => Drain(new SelectManyTransformer<int, int>((Func<int, IEnumerable<int>>)(i => new[] { i }))),
        "SelectMany-async" => Drain(new SelectManyTransformer<int, int>((Func<int, IAsyncEnumerable<int>>)(i => ToAsync(new[] { i })))),
        "Cast" => Drain(new CastTransformer<int, int>()),
        "OfType" => Drain(new OfTypeTransformer<int, int>()),
        "Distinct" => Drain(new DistinctTransformer<int>()),
        "DistinctBy" => Drain(new DistinctByTransformer<int, int>(i => i)),
        "Skip" => Drain(new SkipTransformer<int>(1)),
        "SkipWhile-sync" => Drain(new SkipWhileTransformer<int>((Func<int, bool>)(_ => false))),
        "SkipWhile-async" => Drain(new SkipWhileTransformer<int>((Func<int, ValueTask<bool>>)(_ => new ValueTask<bool>(false)))),
        "Take" => Drain(new TakeTransformer<int>(SourceLength)),
        "TakeWhile-sync" => Drain(new TakeWhileTransformer<int>((Func<int, bool>)(_ => true))),
        "TakeWhile-async" => Drain(new TakeWhileTransformer<int>((Func<int, ValueTask<bool>>)(_ => new ValueTask<bool>(true)))),
        "Chunk" => Drain(new ChunkTransformer<int>(SourceLength)),
        "ProgressReporting-sync" => Drain(new ProgressReportingTransformer<int>((Action<int>)(_ => { }))),
        _ => Drain(new ProgressReportingTransformer<int>((Func<int, ValueTask>)(_ => default))),
    };



    // A source with no [EnumeratorCancellation] token: WithCancellation reaches nothing, so only the
    // transformer's own checks can stop it. It cancels the token itself when it hands out item
    // number cancelOnPull, so the mid-run case needs no timing.
    private sealed class TokenIgnoringSource
    {
        private readonly int? _cancelOnPull;

        private readonly CancellationTokenSource _cts;



        public TokenIgnoringSource(int? cancelOnPull, CancellationTokenSource cts)
        {
            _cancelOnPull = cancelOnPull;
            _cts = cts;
        }



        public int PullCount { get; private set; }



        public async IAsyncEnumerable<int> Enumerate()
        {
            for (var i = 0; i < SourceLength; i++)
            {
                PullCount++;
                if (PullCount == _cancelOnPull)
                {
                    _cts.Cancel();
                }

                yield return i;
                await Task.Yield();
            }
        }
    }
}
