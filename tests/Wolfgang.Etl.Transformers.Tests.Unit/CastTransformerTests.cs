using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class CastTransformerTests
{
    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new CastTransformer<object, string>();

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
        var sut = new CastTransformer<object, string>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<object>())));

        Assert.Empty(result);
    }



    // ---------- all items cast successfully ----------

    [Fact]
    public async Task TransformAsync_when_all_items_match_destination_type_yields_them_cast()
    {
        var sut = new CastTransformer<object, string>();
        var source = new object[] { "a", "b", "c" };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(new[] { "a", "b", "c" }, result);
    }



    // ---------- subtype downcast ----------

    [Fact]
    public async Task TransformAsync_downcasts_subtypes_in_an_inheritance_hierarchy()
    {
        var rex = new Dog("Rex");
        var buddy = new Dog("Buddy");
        var sut = new CastTransformer<Animal, Dog>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new Animal[] { rex, buddy })));

        Assert.Same(rex, result[0]);
        Assert.Same(buddy, result[1]);
    }



    // ---------- mismatched type throws ----------

    [Fact]
    public async Task TransformAsync_when_an_item_is_not_assignable_throws_InvalidCastException()
    {
        var sut = new CastTransformer<object, string>();
        var source = new object[] { "a", 1, "b" };

        await Assert.ThrowsAsync<InvalidCastException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(source)))
        );
    }



    [Fact]
    public async Task TransformAsync_throws_InvalidCastException_on_first_mismatch_yielding_prior_items()
    {
        var sut = new CastTransformer<object, string>();
        var source = new object[] { "a", "b", 42, "c" };
        var collected = new List<string>();

        await Assert.ThrowsAsync<InvalidCastException>
        (
            async () =>
            {
                await foreach (var item in sut.TransformAsync(ToAsync(source)))
                {
                    collected.Add(item);
                }
            }
        );

        Assert.Equal(new[] { "a", "b" }, collected);
    }



    // ---------- mismatched subtype downcast throws ----------

    [Fact]
    public async Task TransformAsync_when_subtype_downcast_encounters_wrong_subtype_throws()
    {
        var sut = new CastTransformer<Animal, Dog>();
        var source = new Animal[] { new Dog("Rex"), new Cat("Whiskers") };

        await Assert.ThrowsAsync<InvalidCastException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(source)))
        );
    }



    // ---------- reference identity ----------

    [Fact]
    public async Task TransformAsync_preserves_reference_identity_for_yielded_items()
    {
        var rex = new Dog("Rex");
        var buddy = new Dog("Buddy");
        var sut = new CastTransformer<Animal, Dog>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new Animal[] { rex, buddy })));

        Assert.Collection
        (
            result,
            d => Assert.Same(rex, d),
            d => Assert.Same(buddy, d)
        );
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_items()
    {
        var sut = new CastTransformer<object, int>();
        var source = new object[] { 1, 2, 3, 4, 5 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }



    // ---------- value types (boxing) ----------

    [Fact]
    public async Task TransformAsync_unboxes_value_types_correctly()
    {
        var sut = new CastTransformer<object, int>();
        var source = new object[] { 10, 20, 30 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(new[] { 10, 20, 30 }, result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void CastTransformer_implements_ITransformAsync()
    {
        var sut = new CastTransformer<object, string>();

        Assert.IsAssignableFrom<ITransformAsync<object, string>>(sut);
    }



    // test fixtures

    private abstract class Animal
    {
        protected Animal(string name)
        {
            Name = name;
        }



        public string Name { get; }
    }



    private sealed class Dog : Animal
    {
        public Dog(string name) : base(name)
        {
        }
    }



    private sealed class Cat : Animal
    {
        public Cat(string name) : base(name)
        {
        }
    }
}
