using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class TakeWhileTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_with_sync_predicate_when_predicate_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new TakeWhileTransformer<int>((Func<int, bool>)null!)
        );

        Assert.Equal("predicate", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_async_predicate_when_predicate_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new TakeWhileTransformer<int>((Func<int, ValueTask<bool>>)null!)
        );

        Assert.Equal("predicate", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new TakeWhileTransformer<int>(_ => true);

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- sync predicate ----------

    [Fact]
    public async Task TransformAsync_sync_predicate_yields_items_until_first_false()
    {
        var sut = new TakeWhileTransformer<int>(i => i < 4);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }



    [Fact]
    public async Task TransformAsync_sync_predicate_when_first_item_fails_yields_empty_sequence()
    {
        var sut = new TakeWhileTransformer<int>(i => i > 100);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_sync_predicate_when_all_items_pass_yields_all_items()
    {
        var sut = new TakeWhileTransformer<int>(_ => true);
        var source = new[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    // ---------- async predicate ----------

    [Fact]
    public async Task TransformAsync_async_predicate_yields_items_until_first_false()
    {
        var sut = new TakeWhileTransformer<int>(i => new ValueTask<bool>(result: i < 4));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }



    [Fact]
    public async Task TransformAsync_async_predicate_when_first_item_fails_yields_empty_sequence()
    {
        var sut = new TakeWhileTransformer<int>(_ => new ValueTask<bool>(result: false));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_async_predicate_when_all_items_pass_yields_all_items()
    {
        var sut = new TakeWhileTransformer<int>(_ => new ValueTask<bool>(result: true));
        var source = new[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items()
    {
        var sut = new TakeWhileTransformer<int>(_ => true);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- early termination ----------

    [Fact]
    public async Task TransformAsync_stops_enumerating_source_after_first_failing_item()
    {
        var enumerated = 0;
        var sut = new TakeWhileTransformer<int>(i => i < 4);

        var result = await CollectAsync
        (
            sut.TransformAsync(CountingSource(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, () => Interlocked.Increment(ref enumerated)))
        );

        Assert.Equal(new[] { 1, 2, 3 }, result);
        // The transformer reads items 1, 2, 3 (yield), then 4 (fails predicate, stops).
        // Items 5-10 should not be enumerated.
        Assert.Equal(4, enumerated);
    }



    // ---------- failing item is not yielded ----------

    [Fact]
    public async Task TransformAsync_does_not_yield_first_failing_item()
    {
        var sut = new TakeWhileTransformer<int>(i => i != 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6, 7 })));

        Assert.Equal(new[] { 1, 2, 3, 4 }, result);
        Assert.DoesNotContain(5, result);
    }



    // ---------- exception propagation ----------

    [Fact]
    public async Task TransformAsync_when_sync_predicate_throws_exception_propagates()
    {
        Func<int, bool> predicate = _ => throw new InvalidOperationException("boom");
        var sut = new TakeWhileTransformer<int>(predicate);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("boom", ex.Message);
    }



    [Fact]
    public async Task TransformAsync_when_async_predicate_throws_exception_propagates()
    {
        Func<int, ValueTask<bool>> predicate = _ => throw new InvalidOperationException("async-boom");
        var sut = new TakeWhileTransformer<int>(predicate);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("async-boom", ex.Message);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_yielded_items()
    {
        var sut = new TakeWhileTransformer<int>(i => i < 50);
        var source = Enumerable.Range(1, 100).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(Enumerable.Range(1, 49), result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void TakeWhileTransformer_implements_ITransformAsync()
    {
        Func<int, bool> predicate = _ => true;
        var sut = new TakeWhileTransformer<int>(predicate);

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



    // ---------- helpers ----------

    private static async IAsyncEnumerable<T> CountingSource<T>(IEnumerable<T> items, Action onItem)
    {
        foreach (var item in items)
        {
            onItem();
            await Task.Yield();
            yield return item;
        }
    }



}
