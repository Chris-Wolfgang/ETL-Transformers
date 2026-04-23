using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class SkipWhileTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_with_sync_predicate_when_predicate_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SkipWhileTransformer<int>((Func<int, bool>)null!)
        );

        Assert.Equal("predicate", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_async_predicate_when_predicate_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SkipWhileTransformer<int>((Func<int, ValueTask<bool>>)null!)
        );

        Assert.Equal("predicate", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new SkipWhileTransformer<int>(_ => true);

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- sync predicate ----------

    [Fact]
    public async Task TransformAsync_sync_predicate_skips_leading_items_then_yields_rest()
    {
        var sut = new SkipWhileTransformer<int>(i => i < 4);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6 })));

        Assert.Equal(new[] { 4, 5, 6 }, result);
    }



    [Fact]
    public async Task TransformAsync_sync_predicate_when_first_item_fails_yields_all_items()
    {
        var sut = new SkipWhileTransformer<int>(i => i > 100);
        var source = new[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_sync_predicate_when_all_items_pass_yields_empty_sequence()
    {
        var sut = new SkipWhileTransformer<int>(_ => true);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    // ---------- async predicate ----------

    [Fact]
    public async Task TransformAsync_async_predicate_skips_leading_items_then_yields_rest()
    {
        var sut = new SkipWhileTransformer<int>(i => new ValueTask<bool>(result: i < 4));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6 })));

        Assert.Equal(new[] { 4, 5, 6 }, result);
    }



    [Fact]
    public async Task TransformAsync_async_predicate_when_first_item_fails_yields_all_items()
    {
        var sut = new SkipWhileTransformer<int>(_ => new ValueTask<bool>(result: false));
        var source = new[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_async_predicate_when_all_items_pass_yields_empty_sequence()
    {
        var sut = new SkipWhileTransformer<int>(_ => new ValueTask<bool>(result: true));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items()
    {
        var sut = new SkipWhileTransformer<int>(_ => true);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- predicate stops being called once first false hit ----------

    [Fact]
    public async Task TransformAsync_does_not_call_predicate_after_first_false_sync()
    {
        var calls = 0;
        var sut = new SkipWhileTransformer<int>
        (
            i =>
            {
                Interlocked.Increment(ref calls);
                return i < 3;
            }
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })));

        Assert.Equal(new[] { 3, 4, 5, 6, 7, 8, 9, 10 }, result);
        // Predicate is called for items 1 (true), 2 (true), 3 (false).
        // After that it should never be called again.
        Assert.Equal(3, calls);
    }



    [Fact]
    public async Task TransformAsync_does_not_call_predicate_after_first_false_async()
    {
        var calls = 0;
        var sut = new SkipWhileTransformer<int>
        (
            i =>
            {
                Interlocked.Increment(ref calls);
                return new ValueTask<bool>(result: i < 3);
            }
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })));

        Assert.Equal(new[] { 3, 4, 5, 6, 7, 8, 9, 10 }, result);
        Assert.Equal(3, calls);
    }



    // ---------- failing item is yielded ----------

    [Fact]
    public async Task TransformAsync_yields_first_item_where_predicate_returns_false()
    {
        var sut = new SkipWhileTransformer<int>(i => i != 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6, 7 })));

        Assert.Equal(new[] { 5, 6, 7 }, result);
        Assert.Contains(5, result);
    }



    // ---------- exception propagation ----------

    [Fact]
    public async Task TransformAsync_when_sync_predicate_throws_exception_propagates()
    {
        Func<int, bool> predicate = _ => throw new InvalidOperationException("boom");
        var sut = new SkipWhileTransformer<int>(predicate);

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
        var sut = new SkipWhileTransformer<int>(predicate);

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
        var sut = new SkipWhileTransformer<int>(i => i < 50);
        var source = Enumerable.Range(1, 100).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(Enumerable.Range(50, 51), result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void SkipWhileTransformer_implements_ITransformAsync()
    {
        Func<int, bool> predicate = _ => true;
        var sut = new SkipWhileTransformer<int>(predicate);

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
}
