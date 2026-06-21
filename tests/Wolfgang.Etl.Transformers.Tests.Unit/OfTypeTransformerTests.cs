using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class OfTypeTransformerTests
{
    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new OfTypeTransformer<object, string>();

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
        var sut = new OfTypeTransformer<object, string>();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<object>())));

        Assert.Empty(result);
    }



    // ---------- all items match ----------

    [Fact]
    public async Task TransformAsync_when_all_items_match_destination_type_yields_all_items()
    {
        var sut = new OfTypeTransformer<object, string>();
        var source = new object[] { "a", "b", "c" };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(new[] { "a", "b", "c" }, result);
    }



    // ---------- no items match ----------

    [Fact]
    public async Task TransformAsync_when_no_items_match_destination_type_yields_empty_sequence()
    {
        var sut = new OfTypeTransformer<object, string>();
        var source = new object[] { 1, 2, 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Empty(result);
    }



    // ---------- mixed types ----------

    [Fact]
    public async Task TransformAsync_when_source_has_mixed_types_yields_only_matching_items()
    {
        var sut = new OfTypeTransformer<object, string>();
        var source = new object[] { "a", 1, "b", 2.0, "c", true };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(new[] { "a", "b", "c" }, result);
    }



    // ---------- subtype filtering ----------

    [Fact]
    public async Task TransformAsync_filters_to_specific_subtype_in_an_inheritance_hierarchy()
    {
        var sut = new OfTypeTransformer<Animal, Dog>();
        var source = new Animal[]
        {
            new Dog("Rex"),
            new Cat("Whiskers"),
            new Dog("Buddy"),
            new Cat("Mittens"),
        };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(2, result.Count);
        Assert.Equal("Rex", result[0].Name);
        Assert.Equal("Buddy", result[1].Name);
    }



    // ---------- reference identity ----------

    [Fact]
    public async Task TransformAsync_preserves_reference_identity_for_yielded_items()
    {
        var dog1 = new Dog("Rex");
        var dog2 = new Dog("Buddy");
        var cat = new Cat("Whiskers");

        var sut = new OfTypeTransformer<Animal, Dog>();
        var result = await CollectAsync(sut.TransformAsync(ToAsync(new Animal[] { dog1, cat, dog2 })));

        Assert.Collection
        (
            result,
            d => Assert.Same(dog1, d),
            d => Assert.Same(dog2, d)
        );
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_matching_items()
    {
        var sut = new OfTypeTransformer<object, int>();
        var source = new object[] { 1, "skip", 2, "skip", 3, "skip", 4 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(new[] { 1, 2, 3, 4 }, result);
    }



    // ---------- never throws on type mismatch ----------

    [Fact]
    public async Task TransformAsync_does_not_throw_when_items_are_not_of_destination_type()
    {
        var sut = new OfTypeTransformer<object, string>();
        var source = new object[] { 1, 2.5, true, new object() };

        // Should not throw even though no items can be cast to string
        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Empty(result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void OfTypeTransformer_implements_ITransformAsync()
    {
        var sut = new OfTypeTransformer<object, string>();

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
