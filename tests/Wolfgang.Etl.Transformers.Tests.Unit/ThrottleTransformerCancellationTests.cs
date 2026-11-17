using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// The throttle's pacing delay must observe the pipeline's run token, which an
/// <c>EtlPipeline</c> hands only to transformers implementing
/// <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/>.
/// </summary>
public class ThrottleTransformerCancellationTests
{
    [Fact]
    public async Task TransformAsync_with_token_passes_that_token_to_the_delay()
    {
        using var cts = new CancellationTokenSource();
        var seen = new System.Collections.Generic.List<CancellationToken>();
        var sut = new ThrottleTransformer<int>
        (
            TimeSpan.FromMilliseconds(250),
            (_, token) =>
            {
                seen.Add(token);
                return Task.CompletedTask;
            },
            () => 0
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 }), cts.Token));

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Equal(2, seen.Count);
        Assert.All(seen, t => Assert.True(t.CanBeCanceled));
        cts.Cancel();
        Assert.All(seen, t => Assert.True(t.IsCancellationRequested));
    }



    [Fact]
    public async Task TransformAsync_with_pre_cancelled_token_throws_before_pulling_from_the_source()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var source = new TestHelpers.CountingSource(3);
        var sut = new ThrottleTransformer<int>(TimeSpan.FromMilliseconds(1));

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () => await CollectAsync(sut.TransformAsync(source.Enumerate(), cts.Token))
        );

        Assert.Equal(0, source.PullCount);
    }



    [Fact]
    public async Task Throttle_in_an_EtlPipeline_cancels_mid_delay_without_waiting_out_the_interval()
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(300);
        var watch = Stopwatch.StartNew();

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach
                (
                    var _ in EtlPipeline
                        .Create()
                        .From(ToAsync(new[] { 1, 2, 3 }))
                        .Throttle(TimeSpan.FromSeconds(30))
                        .AsAsyncEnumerable(cts.Token)
                )
                {
                }
            }
        );

        Assert.True(watch.Elapsed < TimeSpan.FromSeconds(10), $"took {watch.Elapsed}");
    }
}
