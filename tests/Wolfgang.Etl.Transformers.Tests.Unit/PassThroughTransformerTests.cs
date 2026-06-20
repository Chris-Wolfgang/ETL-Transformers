using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class PassThroughTransformerTests
{
    [Fact]
    public async Task TransformAsync_when_source_has_items_yields_each_item_unchanged()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var sut = new PassThroughTransformer<int>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items()
    {
        var sut = new PassThroughTransformer<int>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_preserves_reference_identity_for_each_item()
    {
        var item1 = new Box<int>(1);
        var item2 = new Box<int>(2);
        var source = new[] { item1, item2 };
        var sut = new PassThroughTransformer<Box<int>>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Same(item1, result[0]);
        Assert.Same(item2, result[1]);
    }



    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new PassThroughTransformer<int>();

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public void TransformAsync_with_cancellation_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new PassThroughTransformer<int>();

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!, CancellationToken.None)
        );

        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public async Task TransformAsync_when_cancellation_already_requested_throws_OperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var sut = new PassThroughTransformer<int>();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(ToAsync(new[] { 1, 2, 3 }), cts.Token))
                {
                }
            }
        );
    }



    [Fact]
    public async Task TransformAsync_when_cancelled_mid_enumeration_throws_OperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        var sut = new PassThroughTransformer<int>();
        var yielded = 0;

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(ToAsync(InfiniteSource()), cts.Token))
                {
                    yielded++;
                    if (yielded == 3)
                    {
                        cts.Cancel();
                    }
                }
            }
        );

        Assert.True(yielded >= 3);
    }



    [Fact]
    public async Task TransformAsync_without_cancellation_completes_normally()
    {
        var source = new[] { "a", "b", "c" };
        var sut = new PassThroughTransformer<string>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_with_CancellationToken_None_completes_normally()
    {
        var source = new[] { "a", "b", "c" };
        var sut = new PassThroughTransformer<string>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source), CancellationToken.None));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_preserves_order_of_items()
    {
        var source = Enumerable.Range(0, 100).ToArray();
        var sut = new PassThroughTransformer<int>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public void PassThroughTransformer_implements_ITransformAsync()
    {
        var sut = new PassThroughTransformer<int>();

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



    [Fact]
    public void PassThroughTransformer_implements_ITransformWithCancellationAsync()
    {
        var sut = new PassThroughTransformer<int>();

        Assert.IsAssignableFrom<ITransformWithCancellationAsync<int, int>>(sut);
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



    private static IEnumerable<int> InfiniteSource()
    {
        var i = 0;
        while (true)
        {
            yield return i++;
        }
    }



    private sealed class Box<T>
    {
        public Box(T value)
        {
            Value = value;
        }



        public T Value { get; }
    }
}
