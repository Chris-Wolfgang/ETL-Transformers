using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class TakeTransformerTests
{
    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new TakeTransformer<int>(count: 5);

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
        var sut = new TakeTransformer<int>(count: 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- count > source count ----------

    [Fact]
    public async Task TransformAsync_when_count_exceeds_source_yields_all_items()
    {
        var sut = new TakeTransformer<int>(count: 10);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }



    // ---------- count < source count ----------

    [Fact]
    public async Task TransformAsync_when_count_is_less_than_source_yields_first_count_items()
    {
        var sut = new TakeTransformer<int>(count: 3);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }



    // ---------- count = source count ----------

    [Fact]
    public async Task TransformAsync_when_count_equals_source_yields_all_items()
    {
        var sut = new TakeTransformer<int>(count: 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }



    // ---------- count = 0 ----------

    [Fact]
    public async Task TransformAsync_when_count_is_zero_yields_empty_sequence()
    {
        var sut = new TakeTransformer<int>(count: 0);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_when_count_is_zero_does_not_enumerate_source()
    {
        var enumerated = 0;
        var sut = new TakeTransformer<int>(count: 0);

        var result = await CollectAsync
        (
            sut.TransformAsync(CountingSource(new[] { 1, 2, 3 }, () => Interlocked.Increment(ref enumerated)))
        );

        Assert.Empty(result);
        Assert.Equal(0, enumerated);
    }



    // ---------- count < 0 ----------

    [Fact]
    public async Task TransformAsync_when_count_is_negative_yields_empty_sequence()
    {
        var sut = new TakeTransformer<int>(count: -1);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_when_count_is_negative_does_not_enumerate_source()
    {
        var enumerated = 0;
        var sut = new TakeTransformer<int>(count: -5);

        var result = await CollectAsync
        (
            sut.TransformAsync(CountingSource(new[] { 1, 2, 3 }, () => Interlocked.Increment(ref enumerated)))
        );

        Assert.Empty(result);
        Assert.Equal(0, enumerated);
    }



    // ---------- early termination (does not enumerate beyond count) ----------

    [Fact]
    public async Task TransformAsync_stops_enumerating_source_after_count_items()
    {
        var enumerated = 0;
        var sut = new TakeTransformer<int>(count: 3);

        var result = await CollectAsync
        (
            sut.TransformAsync(CountingSource(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, () => Interlocked.Increment(ref enumerated)))
        );

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Equal(3, enumerated);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_yielded_items()
    {
        var sut = new TakeTransformer<int>(count: 50);
        var source = Enumerable.Range(1, 100).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(Enumerable.Range(1, 50), result);
    }



    // ---------- Count property ----------

    [Fact]
    public void Count_property_reflects_constructor_argument()
    {
        Assert.Equal(7, new TakeTransformer<int>(count: 7).Count);
        Assert.Equal(0, new TakeTransformer<int>(count: 0).Count);
        Assert.Equal(-3, new TakeTransformer<int>(count: -3).Count);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void TakeTransformer_implements_ITransformAsync()
    {
        var sut = new TakeTransformer<int>(count: 1);

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
