using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class ChunkTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_when_size_is_zero_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new ChunkTransformer<int>(size: 0)
        );

        Assert.Equal("size", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_size_is_negative_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new ChunkTransformer<int>(size: -3)
        );

        Assert.Equal("size", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_size_one_succeeds()
    {
        var sut = new ChunkTransformer<int>(size: 1);

        Assert.Equal(1, sut.Size);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new ChunkTransformer<int>(size: 5);

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_chunks()
    {
        var sut = new ChunkTransformer<int>(size: 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- exact division ----------

    [Fact]
    public async Task TransformAsync_when_source_length_is_exact_multiple_yields_full_chunks()
    {
        var sut = new ChunkTransformer<int>(size: 3);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Enumerable.Range(1, 9).ToArray())));

        Assert.Collection
        (
            result,
            chunk => Assert.Equal(new[] { 1, 2, 3 }, chunk),
            chunk => Assert.Equal(new[] { 4, 5, 6 }, chunk),
            chunk => Assert.Equal(new[] { 7, 8, 9 }, chunk)
        );
    }



    // ---------- partial final chunk ----------

    [Fact]
    public async Task TransformAsync_when_source_does_not_divide_evenly_yields_smaller_final_chunk()
    {
        var sut = new ChunkTransformer<int>(size: 3);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Enumerable.Range(1, 10).ToArray())));

        Assert.Collection
        (
            result,
            chunk => Assert.Equal(new[] { 1, 2, 3 }, chunk),
            chunk => Assert.Equal(new[] { 4, 5, 6 }, chunk),
            chunk => Assert.Equal(new[] { 7, 8, 9 }, chunk),
            chunk => Assert.Equal(new[] { 10 }, chunk)
        );
    }



    // ---------- source shorter than size ----------

    [Fact]
    public async Task TransformAsync_when_source_is_shorter_than_size_yields_one_partial_chunk()
    {
        var sut = new ChunkTransformer<int>(size: 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2 })));

        Assert.Single(result);
        Assert.Equal(new[] { 1, 2 }, result[0]);
    }



    // ---------- size = 1 ----------

    [Fact]
    public async Task TransformAsync_when_size_is_one_yields_one_chunk_per_item()
    {
        var sut = new ChunkTransformer<int>(size: 1);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 10, 20, 30 })));

        Assert.Collection
        (
            result,
            chunk => Assert.Equal(new[] { 10 }, chunk),
            chunk => Assert.Equal(new[] { 20 }, chunk),
            chunk => Assert.Equal(new[] { 30 }, chunk)
        );
    }



    // ---------- size larger than source ----------

    [Fact]
    public async Task TransformAsync_when_size_exceeds_source_yields_single_chunk_with_all_items()
    {
        var sut = new ChunkTransformer<int>(size: 1000);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Single(result);
        Assert.Equal(new[] { 1, 2, 3 }, result[0]);
    }



    // ---------- chunks are independent arrays (no aliasing) ----------

    [Fact]
    public async Task TransformAsync_yields_independent_chunk_arrays()
    {
        var sut = new ChunkTransformer<int>(size: 2);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(2, result.Count);
        Assert.NotSame(result[0], result[1]);

        // Mutating one chunk must not affect the other
        result[0][0] = 999;
        Assert.Equal(new[] { 3, 4 }, result[1]);
    }



    // ---------- partial final chunk is right-sized ----------

    [Fact]
    public async Task TransformAsync_partial_final_chunk_is_sized_to_actual_item_count()
    {
        var sut = new ChunkTransformer<int>(size: 5);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6, 7 })));

        Assert.Equal(2, result.Count);
        Assert.Equal(5, result[0].Length);
        Assert.Equal(2, result[1].Length);   // not 5 with default trailing values
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_across_chunks()
    {
        var sut = new ChunkTransformer<int>(size: 7);
        var source = Enumerable.Range(1, 100).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        var flattened = result.SelectMany(c => c).ToArray();
        Assert.Equal(source, flattened);
    }



    // ---------- reference identity within chunk ----------

    [Fact]
    public async Task TransformAsync_preserves_reference_identity_of_items_within_chunks()
    {
        var a = new Box(1);
        var b = new Box(2);
        var c = new Box(3);

        var sut = new ChunkTransformer<Box>(size: 2);
        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { a, b, c })));

        Assert.Same(a, result[0][0]);
        Assert.Same(b, result[0][1]);
        Assert.Same(c, result[1][0]);
    }



    // ---------- Size property ----------

    [Fact]
    public void Size_property_reflects_constructor_argument()
    {
        Assert.Equal(7, new ChunkTransformer<int>(size: 7).Size);
        Assert.Equal(1, new ChunkTransformer<int>(size: 1).Size);
        Assert.Equal(int.MaxValue, new ChunkTransformer<int>(size: int.MaxValue).Size);
        // Note: a buffer is only allocated when TransformAsync is enumerated, so
        // constructing with int.MaxValue here is harmless.
    }



    // ---------- interface sanity ----------

    [Fact]
    public void ChunkTransformer_implements_ITransformAsync()
    {
        var sut = new ChunkTransformer<int>(size: 1);

        Assert.IsAssignableFrom<ITransformAsync<int, int[]>>(sut);
    }



    // test fixtures

    private sealed class Box
    {
        public Box(int value)
        {
            Value = value;
        }



        public int Value { get; }
    }
}
