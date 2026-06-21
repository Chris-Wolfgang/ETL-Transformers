using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class SkipTransformerTests
{
    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new SkipTransformer<int>(count: 5);

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
        var sut = new SkipTransformer<int>(count: 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- count > source count ----------

    [Fact]
    public async Task TransformAsync_when_count_exceeds_source_yields_empty_sequence()
    {
        var sut = new SkipTransformer<int>(count: 10);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    // ---------- count < source count ----------

    [Fact]
    public async Task TransformAsync_when_count_is_less_than_source_yields_remaining_items()
    {
        var sut = new SkipTransformer<int>(count: 2);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Equal(new[] { 3, 4, 5 }, result);
    }



    // ---------- count = source count ----------

    [Fact]
    public async Task TransformAsync_when_count_equals_source_yields_empty_sequence()
    {
        var sut = new SkipTransformer<int>(count: 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Empty(result);
    }



    // ---------- count = 0 ----------

    [Fact]
    public async Task TransformAsync_when_count_is_zero_yields_all_items()
    {
        var sut = new SkipTransformer<int>(count: 0);
        var source = new[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public void TransformAsync_when_count_is_zero_returns_source_directly()
    {
        var sut = new SkipTransformer<int>(count: 0);
        var source = ToAsync(new[] { 1, 2, 3 });

        var result = sut.TransformAsync(source);

        Assert.Same(source, result);
    }



    // ---------- count < 0 ----------

    [Fact]
    public async Task TransformAsync_when_count_is_negative_yields_all_items()
    {
        var sut = new SkipTransformer<int>(count: -1);
        var source = new[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public void TransformAsync_when_count_is_negative_returns_source_directly()
    {
        var sut = new SkipTransformer<int>(count: -10);
        var source = ToAsync(new[] { 1, 2, 3 });

        var result = sut.TransformAsync(source);

        Assert.Same(source, result);
    }



    // ---------- count = 1 (header skip) ----------

    [Fact]
    public async Task TransformAsync_when_count_is_one_skips_first_item()
    {
        var sut = new SkipTransformer<string>(count: 1);

        var result = await CollectAsync
        (
            sut.TransformAsync(ToAsync(new[] { "header", "row1", "row2", "row3" }))
        );

        Assert.Equal(new[] { "row1", "row2", "row3" }, result);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_yielded_items()
    {
        var sut = new SkipTransformer<int>(count: 50);
        var source = Enumerable.Range(1, 100).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(Enumerable.Range(51, 50), result);
    }



    // ---------- Count property ----------

    [Fact]
    public void Count_property_reflects_constructor_argument()
    {
        Assert.Equal(7, new SkipTransformer<int>(count: 7).Count);
        Assert.Equal(0, new SkipTransformer<int>(count: 0).Count);
        Assert.Equal(-3, new SkipTransformer<int>(count: -3).Count);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void SkipTransformer_implements_ITransformAsync()
    {
        var sut = new SkipTransformer<int>(count: 1);

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



}
