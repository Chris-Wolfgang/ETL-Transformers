using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class DistinctByTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_when_keySelector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new DistinctByTransformer<Person, int>(keySelector: null!)
        );

        Assert.Equal("keySelector", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_comparer_when_keySelector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new DistinctByTransformer<Person, int>(null!, EqualityComparer<int>.Default)
        );

        Assert.Equal("keySelector", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new DistinctByTransformer<Person, int>(p => p.Id);

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
        var sut = new DistinctByTransformer<Person, int>(p => p.Id);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<Person>())));

        Assert.Empty(result);
    }



    // ---------- all unique keys ----------

    [Fact]
    public async Task TransformAsync_when_all_keys_are_unique_yields_all_items()
    {
        var p1 = new Person(1, "Alice");
        var p2 = new Person(2, "Bob");
        var p3 = new Person(3, "Carol");

        var sut = new DistinctByTransformer<Person, int>(p => p.Id);
        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { p1, p2, p3 })));

        Assert.Equal(new[] { p1, p2, p3 }, result);
    }



    // ---------- duplicate keys ----------

    [Fact]
    public async Task TransformAsync_yields_first_item_per_distinct_key()
    {
        var p1a = new Person(1, "Alice");
        var p1b = new Person(1, "Alicia");   // same id, different name
        var p2 = new Person(2, "Bob");

        var sut = new DistinctByTransformer<Person, int>(p => p.Id);
        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { p1a, p1b, p2 })));

        Assert.Collection
        (
            result,
            p => Assert.Same(p1a, p),
            p => Assert.Same(p2, p)
        );
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_first_occurrences()
    {
        var sut = new DistinctByTransformer<Person, int>(p => p.Id);
        var people = new[]
        {
            new Person(3, "C"),
            new Person(1, "A"),
            new Person(2, "B"),
            new Person(1, "A2"),
            new Person(3, "C2"),
        };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(people)));

        Assert.Equal(new[] { 3, 1, 2 }, result.Select(p => p.Id));
    }



    // ---------- custom comparer ----------

    [Fact]
    public async Task TransformAsync_uses_supplied_key_comparer()
    {
        var sut = new DistinctByTransformer<Person, string>
        (
            p => p.Name,
            StringComparer.OrdinalIgnoreCase
        );

        var people = new[]
        {
            new Person(1, "Alice"),
            new Person(2, "ALICE"),  // same name case-insensitively
            new Person(3, "Bob"),
        };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(people)));

        Assert.Collection
        (
            result,
            p => Assert.Equal(1, p.Id),
            p => Assert.Equal(3, p.Id)
        );
    }



    [Fact]
    public async Task TransformAsync_when_comparer_is_null_uses_default_equality()
    {
        var sut = new DistinctByTransformer<Person, string>(p => p.Name, comparer: null);

        var people = new[]
        {
            new Person(1, "Alice"),
            new Person(2, "ALICE"),
            new Person(3, "Bob"),
        };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(people)));

        // case-sensitive default → all three are kept
        Assert.Equal(3, result.Count);
    }



    // ---------- key selector exception propagates ----------

    [Fact]
    public async Task TransformAsync_when_key_selector_throws_exception_propagates()
    {
        Func<Person, int> selector = _ => throw new InvalidOperationException("key-boom");
        var sut = new DistinctByTransformer<Person, int>(selector);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { new Person(1, "A") })))
        );

        Assert.Equal("key-boom", ex.Message);
    }



    // ---------- fresh state per call ----------

    [Fact]
    public async Task TransformAsync_uses_fresh_state_on_each_call()
    {
        var sut = new DistinctByTransformer<Person, int>(p => p.Id);
        var people = new[] { new Person(1, "A"), new Person(2, "B") };

        var first = await CollectAsync(sut.TransformAsync(ToAsync(people)));
        var second = await CollectAsync(sut.TransformAsync(ToAsync(people)));

        Assert.Equal(2, first.Count);
        Assert.Equal(2, second.Count);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void DistinctByTransformer_implements_ITransformAsync()
    {
        var sut = new DistinctByTransformer<Person, int>(p => p.Id);

        Assert.IsAssignableFrom<ITransformAsync<Person, Person>>(sut);
    }



    // test fixtures

    private sealed class Person
    {
        public Person(int id, string name)
        {
            Id = id;
            Name = name;
        }



        public int Id { get; }



        public string Name { get; }
    }
}
