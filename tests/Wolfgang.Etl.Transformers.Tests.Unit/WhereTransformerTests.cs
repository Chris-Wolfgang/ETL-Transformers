using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class WhereTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_with_sync_predicate_when_predicate_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new WhereTransformer<int>((Func<int, bool>)null!)
        );

        Assert.Equal("predicate", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_async_predicate_when_predicate_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new WhereTransformer<int>((Func<int, ValueTask<bool>>)null!)
        );

        Assert.Equal("predicate", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new WhereTransformer<int>(_ => true);

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- sync predicate ----------

    [Fact]
    public async Task TransformAsync_sync_predicate_yields_only_items_where_predicate_is_true()
    {
        var sut = new WhereTransformer<int>(i => i % 2 == 0);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6 })));

        Assert.Equal(new[] { 2, 4, 6 }, result);
    }



    [Fact]
    public async Task TransformAsync_sync_predicate_when_all_items_pass_yields_all_items()
    {
        var sut = new WhereTransformer<int>(_ => true);
        var source = new[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_sync_predicate_when_no_items_pass_yields_empty_sequence()
    {
        var sut = new WhereTransformer<int>(_ => false);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    // ---------- async predicate ----------

    [Fact]
    public async Task TransformAsync_async_predicate_yields_only_items_where_predicate_is_true()
    {
        var sut = new WhereTransformer<int>(i => new ValueTask<bool>(i > 2));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Equal(new[] { 3, 4, 5 }, result);
    }



    [Fact]
    public async Task TransformAsync_async_predicate_when_all_items_pass_yields_all_items()
    {
        var sut = new WhereTransformer<int>(_ => new ValueTask<bool>(result: true));
        var source = new[] { 10, 20, 30 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_async_predicate_when_no_items_pass_yields_empty_sequence()
    {
        var sut = new WhereTransformer<int>(_ => new ValueTask<bool>(result: false));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items_sync_predicate()
    {
        var sut = new WhereTransformer<int>(_ => true);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items_async_predicate()
    {
        var sut = new WhereTransformer<int>(_ => new ValueTask<bool>(result: true));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- exception propagation ----------

    [Fact]
    public async Task TransformAsync_when_sync_predicate_throws_exception_propagates()
    {
        Func<int, bool> predicate = _ => throw new InvalidOperationException("boom");
        var sut = new WhereTransformer<int>(predicate);

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
        var sut = new WhereTransformer<int>(predicate);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("async-boom", ex.Message);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_passing_items()
    {
        var sut = new WhereTransformer<int>(i => i % 3 == 0);
        var source = Enumerable.Range(1, 30).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(new[] { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30 }, result);
    }



    // ---------- reference identity ----------

    [Fact]
    public async Task TransformAsync_preserves_reference_identity_for_yielded_items()
    {
        var a = new Box(1);
        var b = new Box(2);
        var c = new Box(3);

        Func<Box, bool> predicate = x => x.Value % 2 == 1;
        var sut = new WhereTransformer<Box>(predicate);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { a, b, c })));

        Assert.Collection
        (
            result,
            item => Assert.Same(a, item),
            item => Assert.Same(c, item)
        );
    }



    // ---------- interface sanity ----------

    [Fact]
    public void WhereTransformer_implements_ITransformAsync()
    {
        Func<int, bool> predicate = _ => true;
        var sut = new WhereTransformer<int>(predicate);

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
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
