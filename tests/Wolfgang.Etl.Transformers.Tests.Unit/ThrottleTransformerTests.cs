using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;

namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// Tests <see cref="ThrottleTransformer{T}"/> pacing deterministically via its internal test seam
/// (an injected delay recorder and a fixed timestamp source), so no real time passes.
/// </summary>
public class ThrottleTransformerTests
{
    [Fact]
    public async Task TransformAsync_yields_all_items_in_order()
    {
        var sut = new ThrottleTransformer<int>(TimeSpan.FromMilliseconds(1));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }


    [Fact]
    public async Task TransformAsync_delays_each_item_after_the_first_by_the_min_interval()
    {
        var recorder = new DelayRecorder();
        var interval = TimeSpan.FromMilliseconds(250);
        // Fixed timestamp => measured elapsed is always 0 => a full-interval wait is requested each time.
        var sut = new ThrottleTransformer<int>(interval, recorder.Record, () => 0L);
        var waits = recorder.Waits;

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Equal(2, waits.Count);          // the first item is never delayed
        Assert.All(waits, w => Assert.Equal(interval, w));
    }


    [Fact]
    public async Task TransformAsync_when_consumer_is_already_slower_than_the_interval_adds_no_delay()
    {
        var recorder = new DelayRecorder();
        long clock = 0;
        // Advance a full second per timestamp read — always >= the 250 ms interval, so the measured
        // elapsed already exceeds it and no additional wait should be requested (the adaptive path).
        var step = Stopwatch.Frequency;
        var sut = new ThrottleTransformer<int>(
            TimeSpan.FromMilliseconds(250),
            recorder.Record,
            () => { clock += step; return clock; });
        var waits = recorder.Waits;

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Empty(waits);
    }


    [Fact]
    public async Task TransformAsync_measures_the_interval_from_delivery_so_consumer_time_counts()
    {
        var recorder = new DelayRecorder();
        var interval = TimeSpan.FromMilliseconds(250);
        var oneInterval = (long)(Stopwatch.Frequency * interval.TotalSeconds);
        long clock = 0;
        // Timestamp advances only when the consumer (below) spends time between items — not on
        // every read — so this isolates the "interval measured from delivery, not from when the
        // consumer returns" contract: recording after the yield would restart elapsed at 0 and
        // wrongly request a full delay each time.
        var sut = new ThrottleTransformer<int>(
            interval,
            recorder.Record,
            // ReSharper disable once AccessToModifiedClosure — deliberately reads the current `clock`; the test mutates it between items to model consumer processing time and verify the throttle measures FROM delivery.
            () => clock);
        var waits = recorder.Waits;

        await foreach (var _ in sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        {
            // The consumer spends a full interval processing each item.
            clock += oneInterval;
        }

        // Elapsed since delivery already covers the interval, so no throttle delay is needed.
        Assert.Empty(waits);
    }


    [Fact]
    public void Internal_ctor_when_delay_or_timestamp_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ThrottleTransformer<int>(TimeSpan.Zero, null!, () => 0L));
        Assert.Throws<ArgumentNullException>(() => new ThrottleTransformer<int>(TimeSpan.Zero, (_, _) => Task.CompletedTask, null!));
    }


    [Fact]
    public async Task TransformAsync_when_min_interval_is_zero_never_delays()
    {
        var recorder = new DelayRecorder();
        var sut = new ThrottleTransformer<int>(TimeSpan.Zero, recorder.Record, () => 0L);
        var waits = recorder.Waits;

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Empty(waits);
    }


    [Fact]
    public void Ctor_when_min_interval_is_negative_throws_ArgumentOutOfRangeException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new ThrottleTransformer<int>(TimeSpan.FromMilliseconds(-1)));


    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => new ThrottleTransformer<int>(TimeSpan.Zero).TransformAsync(null!));



    // ---------- pre-cancelled token (#209) ----------

    // Guards the "no item is drained before observing an already-cancelled token" contract.
    // Regression: revert the pre-check at the top of ThrottleTransformer.TransformAsyncCore
    // and this test fails — CountingSource.PullCount becomes 1.
    [Fact]
    public async Task TransformAsync_when_token_is_pre_cancelled_pulls_zero_items_from_source()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var source = new TestHelpers.CountingSource(4);
        var sut = new ThrottleTransformer<int>(TimeSpan.FromMilliseconds(1));

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(source.Enumerate()).WithCancellation(cts.Token))
                {
                }
            }
        );

        Assert.Equal(0, source.PullCount);
    }



    // Shared delay-request recorder. Using an instance-method group (`recorder.Record`)
    // instead of an inline lambda per test means every test SHARES this method's coverage —
    // tests where the throttle DOES request a delay cover `Record`, and the tests where it
    // doesn't don't create their own uncovered closure.
    private sealed class DelayRecorder
    {
        public List<TimeSpan> Waits { get; } = new();


        [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "The CancellationToken is required by the Func<TimeSpan, CancellationToken, Task> delegate signature this method binds to via method-group conversion. `_` marks the intentional discard, but Sonar 10.32+ no longer exempts it.")]
        public Task Record(TimeSpan delay, CancellationToken _)
        {
            Waits.Add(delay);
            return Task.CompletedTask;
        }
    }
}
