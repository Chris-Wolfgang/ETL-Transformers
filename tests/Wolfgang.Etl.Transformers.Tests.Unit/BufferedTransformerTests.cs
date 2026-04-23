using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class BufferedTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_when_capacity_is_zero_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new BufferedTransformer<int>(capacity: 0)
        );

        Assert.Equal("capacity", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_capacity_is_negative_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new BufferedTransformer<int>(capacity: -3)
        );

        Assert.Equal("capacity", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_capacity_one_succeeds()
    {
        var sut = new BufferedTransformer<int>(capacity: 1);

        Assert.Equal(1, sut.Capacity);
    }



    [Fact]
    public void Capacity_property_reflects_constructor_argument()
    {
        Assert.Equal(7, new BufferedTransformer<int>(capacity: 7).Capacity);
        Assert.Equal(1024, new BufferedTransformer<int>(capacity: 1024).Capacity);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new BufferedTransformer<int>(capacity: 10);

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items()
    {
        var sut = new BufferedTransformer<int>(capacity: 10);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- pass-through fidelity ----------

    [Fact]
    public async Task TransformAsync_yields_each_item_unchanged_in_order()
    {
        var sut = new BufferedTransformer<int>(capacity: 10);
        var source = new[] { 1, 2, 3, 4, 5 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_preserves_order_for_large_source_with_small_buffer()
    {
        // Source larger than buffer exercises backpressure: producer fills, waits, fills,
        // waits, ... Consumer drains continuously. Output must still match source 1:1.
        var sut = new BufferedTransformer<int>(capacity: 4);
        var source = Enumerable.Range(1, 1000).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    // ---------- reference identity ----------

    [Fact]
    public async Task TransformAsync_preserves_reference_identity_of_yielded_items()
    {
        var a = new Box(1);
        var b = new Box(2);
        var c = new Box(3);

        var sut = new BufferedTransformer<Box>(capacity: 8);
        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { a, b, c })));

        Assert.Collection
        (
            result,
            x => Assert.Same(a, x),
            x => Assert.Same(b, x),
            x => Assert.Same(c, x)
        );
    }



    // ---------- producer error propagation ----------

    [Fact]
    public async Task TransformAsync_when_source_throws_exception_propagates_to_consumer()
    {
        var sut = new BufferedTransformer<int>(capacity: 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ThrowingSource()))
        );

        Assert.Equal("source-boom", ex.Message);

        static async IAsyncEnumerable<int> ThrowingSource()
        {
            yield return 1;
            yield return 2;
            await Task.Yield();
            throw new InvalidOperationException("source-boom");
        }
    }



    [Fact]
    public async Task TransformAsync_when_source_throws_yields_already_buffered_items_first()
    {
        var sut = new BufferedTransformer<int>(capacity: 100);
        var collected = new List<int>();

        await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () =>
            {
                await foreach (var item in sut.TransformAsync(ThrowingAfterFive()))
                {
                    collected.Add(item);
                }
            }
        );

        // The first 5 items must have been delivered before the exception surfaced.
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, collected);

        static async IAsyncEnumerable<int> ThrowingAfterFive()
        {
            for (var i = 1; i <= 5; i++)
            {
                yield return i;
                await Task.Yield();
            }
            throw new InvalidOperationException("after-five-boom");
        }
    }



    // ---------- consumer abandonment ----------

    [Fact]
    public async Task TransformAsync_when_consumer_breaks_early_disposes_source_enumerator()
    {
        var disposalSignal = new TaskCompletionSource<bool>();
        var sut = new BufferedTransformer<int>(capacity: 4);

        await foreach (var item in sut.TransformAsync(InfiniteSource(disposalSignal)))
        {
            if (item >= 5)
            {
                break;
            }
        }

        // After consumer breaks, the iterator's finally must cancel the producer, which
        // in turn disposes the source enumerator. This must happen quickly - well under
        // a generous timeout.
        var disposed = await Task.WhenAny
        (
            disposalSignal.Task,
            Task.Delay(TimeSpan.FromSeconds(5))
        ) == disposalSignal.Task;

        Assert.True(disposed, "Source enumerator was not disposed within 5s after consumer abandonment");
    }



    // ---------- external cancellation propagation ----------

    [Fact]
    public async Task TransformAsync_external_cancellation_via_WithCancellation_stops_producer()
    {
        var disposalSignal = new TaskCompletionSource<bool>();
        var sut = new BufferedTransformer<int>(capacity: 4);
        using var cts = new CancellationTokenSource();
        var consumed = 0;

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(InfiniteSource(disposalSignal)).WithCancellation(cts.Token))
                {
                    consumed++;
                    if (consumed == 5)
                    {
                        cts.Cancel();
                    }
                }
            }
        );

        // Producer should have been signaled and the source enumerator disposed.
        var disposed = await Task.WhenAny
        (
            disposalSignal.Task,
            Task.Delay(TimeSpan.FromSeconds(5))
        ) == disposalSignal.Task;

        Assert.True(disposed, "Source enumerator was not disposed within 5s after external cancellation");
        Assert.True(consumed >= 5);
    }



    // ---------- pre-cancelled external token ----------

    [Fact]
    public async Task TransformAsync_when_external_token_is_pre_cancelled_throws_OperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var sut = new BufferedTransformer<int>(capacity: 4);

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })).WithCancellation(cts.Token))
                {
                }
            }
        );
    }



    // ---------- backpressure sanity ----------

    [Fact]
    public async Task TransformAsync_buffer_does_not_grow_beyond_capacity()
    {
        // Track in-flight items: produced - consumed. With capacity N the in-flight count
        // must never exceed N + 1 (the +1 covers the item the producer just took off the
        // buffer to write next).
        var capacity = 8;
        var produced = 0;
        var consumed = 0;
        var maxInFlight = 0;
        var sut = new BufferedTransformer<int>(capacity);

        await foreach (var _ in sut.TransformAsync(TrackedSource(50, () => Interlocked.Increment(ref produced))))
        {
            Interlocked.Increment(ref consumed);
            var inFlight = produced - consumed;
            if (inFlight > maxInFlight)
            {
                maxInFlight = inFlight;
            }
        }

        Assert.Equal(50, consumed);
        // Buffer capacity is the contract; allow a small slack for the in-flight item
        // sitting between the source's yield and the channel write.
        Assert.True(maxInFlight <= capacity + 2,
            $"Max in-flight items {maxInFlight} exceeded capacity+2 ({capacity + 2})");

        static async IAsyncEnumerable<int> TrackedSource(int count, Action onYield)
        {
            for (var i = 0; i < count; i++)
            {
                onYield();
                yield return i;
                await Task.Yield();
            }
        }
    }



    // ---------- interface sanity ----------

    [Fact]
    public void BufferedTransformer_implements_ITransformAsync()
    {
        var sut = new BufferedTransformer<int>(capacity: 1);

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



    // ---------- helpers ----------

    private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }



    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> items)
    {
        var list = new List<T>();
        await foreach (var item in items)
        {
            list.Add(item);
        }
        return list;
    }



    /// <summary>
    /// An infinite source that signals via the supplied <see cref="TaskCompletionSource{TResult}"/>
    /// when its iterator is disposed - used to verify the BufferedTransformer cleans up the
    /// upstream when the consumer abandons or cancels.
    /// </summary>
    private static async IAsyncEnumerable<int> InfiniteSource(TaskCompletionSource<bool> disposalSignal)
    {
        var i = 0;
        try
        {
            while (true)
            {
                await Task.Yield();
                yield return i++;
            }
        }
        finally
        {
            disposalSignal.TrySetResult(true);
        }
    }



    private sealed class Box
    {
        public Box(int value)
        {
            Value = value;
        }



        public int Value { get; }
    }
}
