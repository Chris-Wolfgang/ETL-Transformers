using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// Item-error handling for the filtering transformers. These need one distinction the projecting
/// ones do not: an item the predicate deliberately rejects, or a duplicate key, is ordinary
/// filtering and must never be counted as an error.
/// </summary>
public class DelegateTransformerErrorPolicyFilterTests
{
    private static DelegateTransformerOptions SkipAll() =>
        new() { ErrorPolicy = _ => ItemErrorAction.Skip };



    // ---------- WhereTransformer ----------

    [Fact]
    public async Task WhereTransformer_by_default_propagates_a_throwing_predicate()
    {
        var sut = new WhereTransformer<int>(i => i == 2 ? throw new InvalidOperationException("boom") : true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal("boom", ex.Message);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task WhereTransformer_when_policy_skips_drops_the_item_and_counts_it()
    {
        var sut = new WhereTransformer<int>
        (
            i => i == 2 ? throw new InvalidOperationException() : true,
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task WhereTransformer_does_not_count_an_item_the_predicate_merely_rejects()
    {
        // Filtering is not failing: only a thrown exception counts.
        var sut = new WhereTransformer<int>(i => i % 2 != 0, SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task WhereTransformer_with_async_predicate_honours_the_skip_policy()
    {
        var sut = new WhereTransformer<int>
        (
            async i =>
            {
                await Task.Yield();
                return i == 2 ? throw new InvalidOperationException() : true;
            },
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public void WhereTransformer_when_options_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new WhereTransformer<int>(_ => true, null!)
        );

        Assert.Equal("options", ex.ParamName);
    }



    // ---------- DistinctByTransformer ----------

    [Fact]
    public async Task DistinctByTransformer_by_default_propagates_a_throwing_key_selector()
    {
        var sut = new DistinctByTransformer<int, int>(i => i == 2 ? throw new InvalidOperationException("boom") : i);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal("boom", ex.Message);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task DistinctByTransformer_when_policy_skips_drops_the_item_and_counts_it()
    {
        var sut = new DistinctByTransformer<int, int>
        (
            i => i == 2 ? throw new InvalidOperationException() : i,
            comparer: null,
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task DistinctByTransformer_does_not_count_a_duplicate_as_an_error()
    {
        // Dropping a repeated key is the transformer doing its job, not an item failure.
        var sut = new DistinctByTransformer<int, int>(i => i % 2, comparer: null, SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(new[] { 1, 2 }, result);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task DistinctByTransformer_applies_the_policy_when_the_comparer_throws()
    {
        // The comparer is caller-supplied too, so a throw from GetHashCode is an item failure.
        var sut = new DistinctByTransformer<int, int>
        (
            i => i,
            new ThrowingComparer(throwOnKey: 2),
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task DistinctByTransformer_when_the_comparer_throws_still_removes_later_duplicates()
    {
        var sut = new DistinctByTransformer<int, int>
        (
            i => i,
            new ThrowingComparer(throwOnKey: 2),
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 1, 3 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public void DistinctByTransformer_when_options_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new DistinctByTransformer<int, int>(i => i, comparer: null, null!)
        );

        Assert.Equal("options", ex.ParamName);
    }



    private sealed class ThrowingComparer : IEqualityComparer<int>
    {
        private readonly int _throwOnKey;


        public ThrowingComparer(int throwOnKey) => _throwOnKey = throwOnKey;


        public bool Equals(int x, int y) => x == y;


        public int GetHashCode(int obj) =>
            obj == _throwOnKey ? throw new InvalidOperationException() : obj.GetHashCode();
    }
}
