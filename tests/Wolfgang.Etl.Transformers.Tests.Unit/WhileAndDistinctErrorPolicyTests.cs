using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// Item-error handling for <see cref="SkipWhileTransformer{T}"/>,
/// <see cref="TakeWhileTransformer{T}"/> and <see cref="DistinctTransformer{T}"/>. The two While
/// transformers each need a decision the other adopters did not: their predicate controls sequence
/// state, so "skip this item" has to say what happens to that state.
/// </summary>
public class WhileAndDistinctErrorPolicyTests
{
    private static DelegateTransformerOptions SkipAll() =>
        new() { ErrorPolicy = _ => ItemErrorAction.Skip };



    // ---------- SkipWhileTransformer ----------

    [Fact]
    public async Task SkipWhile_by_default_propagates_a_throwing_predicate()
    {
        var sut = new SkipWhileTransformer<int>(i => i == 2 ? throw new InvalidOperationException("boom") : true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal("boom", ex.Message);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SkipWhile_when_the_predicate_throws_stays_in_the_skipping_phase()
    {
        // The decision being pinned: a predicate that threw gave no verdict, so it must not be
        // treated as the false that ends the leading run. Item 2 is dropped and 3 is still
        // evaluated as part of the prefix; only 4 (predicate false) starts the output.
        var sut = new SkipWhileTransformer<int>
        (
            i => i switch
            {
                2 => throw new InvalidOperationException(),
                4 => false,
                _ => true
            },
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Equal(new[] { 4, 5 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SkipWhile_does_not_count_an_item_skipped_by_a_true_predicate()
    {
        var sut = new SkipWhileTransformer<int>(i => i < 3, SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(new[] { 3, 4 }, result);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SkipWhile_does_not_evaluate_the_predicate_after_the_prefix_ends()
    {
        // Once skipping stops the predicate is never called again, so a later throw cannot occur.
        var sut = new SkipWhileTransformer<int>
        (
            i => i switch
            {
                1 => true,
                2 => false,
                _ => throw new InvalidOperationException()
            },
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(new[] { 2, 3, 4 }, result);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SkipWhile_with_async_predicate_throwing_stays_in_the_skipping_phase()
    {
        // The async predicate is a separate await/catch path from the synchronous one, and the
        // state decision is the feature's key semantic choice here, so it is asserted on both.
        var sut = new SkipWhileTransformer<int>
        (
            async i =>
            {
                await Task.Yield();
                return i switch
                {
                    2 => throw new InvalidOperationException(),
                    4 => false,
                    _ => true
                };
            },
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        // 2 dropped, 3 still treated as part of the prefix, 4 ends it.
        Assert.Equal(new[] { 4, 5 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    // ---------- TakeWhileTransformer ----------

    [Fact]
    public async Task TakeWhile_by_default_propagates_a_throwing_predicate()
    {
        var sut = new TakeWhileTransformer<int>(i => i == 2 ? throw new InvalidOperationException("boom") : true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal("boom", ex.Message);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task TakeWhile_when_the_predicate_throws_drops_the_item_and_keeps_evaluating()
    {
        // The decision being pinned: ending the sequence would be a stronger action than Skip asks
        // for, so the failing item is dropped and evaluation continues.
        var sut = new TakeWhileTransformer<int>
        (
            i => i == 2 ? throw new InvalidOperationException() : true,
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task TakeWhile_still_ends_the_sequence_on_a_false_predicate()
    {
        // A real false still terminates - the skip path must not have weakened that.
        var sut = new TakeWhileTransformer<int>(i => i != 3, SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(new[] { 1, 2 }, result);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task TakeWhile_with_async_predicate_honours_the_skip_policy()
    {
        var sut = new TakeWhileTransformer<int>
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



    // ---------- DistinctTransformer ----------

    [Fact]
    public async Task Distinct_by_default_propagates_a_throwing_comparer()
    {
        var sut = new DistinctTransformer<int>(new ThrowingComparer(throwOnValue: 2));

        await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task Distinct_when_the_comparer_throws_drops_the_item_and_counts_it()
    {
        var sut = new DistinctTransformer<int>(new ThrowingComparer(throwOnValue: 2), SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task Distinct_does_not_count_a_duplicate_as_an_error()
    {
        var sut = new DistinctTransformer<int>(comparer: null, SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 1, 2, 2 })));

        Assert.Equal(new[] { 1, 2 }, result);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public void Distinct_when_options_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new DistinctTransformer<int>(comparer: null, null!)
        );

        Assert.Equal("options", ex.ParamName);
    }



    private sealed class ThrowingComparer : IEqualityComparer<int>
    {
        private readonly int _throwOnValue;


        public ThrowingComparer(int throwOnValue) => _throwOnValue = throwOnValue;


        public bool Equals(int x, int y) => x == y;


        public int GetHashCode(int obj) =>
            obj == _throwOnValue ? throw new InvalidOperationException() : obj.GetHashCode();
    }
}
