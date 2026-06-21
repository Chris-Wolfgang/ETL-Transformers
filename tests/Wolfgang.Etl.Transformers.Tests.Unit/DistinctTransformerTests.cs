using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class DistinctTransformerTests
{
    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new DistinctTransformer<int>();

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
        var sut = new DistinctTransformer<int>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- all unique ----------

    [Fact]
    public async Task TransformAsync_when_all_items_are_unique_yields_all_items()
    {
        var sut = new DistinctTransformer<int>();
        var source = new[] { 1, 2, 3, 4, 5 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    // ---------- all duplicates ----------

    [Fact]
    public async Task TransformAsync_when_all_items_are_the_same_yields_first_only()
    {
        var sut = new DistinctTransformer<int>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 7, 7, 7, 7, 7 })));

        Assert.Equal(new[] { 7 }, result);
    }



    // ---------- mixed ----------

    [Fact]
    public async Task TransformAsync_yields_first_occurrence_of_each_distinct_item()
    {
        var sut = new DistinctTransformer<int>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 1, 3, 2, 4, 1, 5 })));

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_first_occurrences()
    {
        var sut = new DistinctTransformer<string>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { "c", "a", "b", "a", "c", "d" })));

        Assert.Equal(new[] { "c", "a", "b", "d" }, result);
    }



    // ---------- custom comparer ----------

    [Fact]
    public async Task TransformAsync_uses_supplied_comparer()
    {
        var sut = new DistinctTransformer<string>(StringComparer.OrdinalIgnoreCase);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { "Apple", "BANANA", "apple", "banana", "Cherry" })));

        Assert.Equal(new[] { "Apple", "BANANA", "Cherry" }, result);
    }



    [Fact]
    public async Task TransformAsync_when_comparer_is_null_uses_default_equality()
    {
        var sut = new DistinctTransformer<string>(comparer: null);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { "a", "A", "a", "A" })));

        // default string equality is case-sensitive
        Assert.Equal(new[] { "a", "A" }, result);
    }



    // ---------- reference identity ----------

    [Fact]
    public async Task TransformAsync_preserves_reference_identity_of_yielded_items()
    {
        var a1 = new Box(1);
        var a2 = new Box(1); // equal value, different reference
        var b = new Box(2);

        var sut = new DistinctTransformer<Box>(BoxValueComparer.Instance);
        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { a1, a2, b })));

        Assert.Collection
        (
            result,
            x => Assert.Same(a1, x),  // first wins
            x => Assert.Same(b, x)
        );
    }



    // ---------- fresh state per call ----------

    [Fact]
    public async Task TransformAsync_uses_fresh_state_on_each_call()
    {
        var sut = new DistinctTransformer<int>();

        var first = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));
        var second = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        // Second call should yield the same items - no leak from first
        Assert.Equal(new[] { 1, 2, 3 }, first);
        Assert.Equal(new[] { 1, 2, 3 }, second);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void DistinctTransformer_implements_ITransformAsync()
    {
        var sut = new DistinctTransformer<int>();

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
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



    private sealed class BoxValueComparer : IEqualityComparer<Box>
    {
        public static readonly BoxValueComparer Instance = new();



        public bool Equals(Box? x, Box? y) => (x?.Value) == (y?.Value);



        public int GetHashCode(Box obj) => obj.Value.GetHashCode();
    }
}
