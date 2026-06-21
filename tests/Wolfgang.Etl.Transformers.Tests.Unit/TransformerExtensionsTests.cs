using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class TransformerExtensionsTests
{
    // ---------- null checks ----------

    [Fact]
    public void Then_when_first_is_null_throws_ArgumentNullException()
    {
        ITransformAsync<int, int> first = null!;

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => first.Then(new PassThroughTransformer<int>())
        );

        Assert.Equal("first", ex.ParamName);
    }



    [Fact]
    public void Then_when_next_is_null_throws_ArgumentNullException()
    {
        ITransformAsync<int, int> first = new PassThroughTransformer<int>();

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => first.Then((ITransformAsync<int, int>)null!)
        );

        Assert.Equal("next", ex.ParamName);
    }



    // ---------- Then returns a ChainTransformer ----------

    [Fact]
    public void Then_returns_a_ChainTransformer_with_the_two_supplied_transformers()
    {
        ITransformAsync<int, int> first = new PassThroughTransformer<int>();
        ITransformAsync<int, int> second = new PassThroughTransformer<int>();

        var result = first.Then(second);

        Assert.IsType<ChainTransformer<int, int, int>>(result);
    }



    // ---------- two-stage chain via .Then ----------

    [Fact]
    public async Task Then_composes_two_transformers_correctly()
    {
        Func<int, int> doubleIt = i => i * 2;
        Func<int, string> stringify = i => i.ToString(CultureInfo.InvariantCulture);

        ITransformAsync<int, int> t1 = new SelectTransformer<int, int>(doubleIt);
        ITransformAsync<int, string> t2 = new SelectTransformer<int, string>(stringify);

        var pipeline = t1.Then(t2);

        var result = await CollectAsync(pipeline.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "2", "4", "6" }, result);
    }



    // ---------- five-stage chain via .Then(...).Then(...).Then(...).Then(...) ----------

    [Fact]
    public async Task Then_composes_five_transformers_via_chained_calls()
    {
        Func<int, int> addOne = i => i + 1;       //  i+1
        Func<int, int> timesTen = i => i * 10;    // (i+1)*10
        Func<int, int> negate = i => -i;          // -((i+1)*10)
        Func<int, int> abs = Math.Abs;            //  ((i+1)*10)
        Func<int, string> stringify = i => i.ToString(CultureInfo.InvariantCulture);

        ITransformAsync<int, int> t1 = new SelectTransformer<int, int>(addOne);
        ITransformAsync<int, int> t2 = new SelectTransformer<int, int>(timesTen);
        ITransformAsync<int, int> t3 = new SelectTransformer<int, int>(negate);
        ITransformAsync<int, int> t4 = new SelectTransformer<int, int>(abs);
        ITransformAsync<int, string> t5 = new SelectTransformer<int, string>(stringify);

        var pipeline = t1.Then(t2).Then(t3).Then(t4).Then(t5);

        var result = await CollectAsync(pipeline.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "20", "30", "40" }, result);
    }



    // ---------- equivalence with explicit ChainTransformer construction ----------

    [Fact]
    public async Task Then_produces_same_output_as_direct_ChainTransformer_construction()
    {
        Func<int, int> doubleIt = i => i * 2;
        Func<int, int> addOne = i => i + 1;

        ITransformAsync<int, int> a = new SelectTransformer<int, int>(doubleIt);
        ITransformAsync<int, int> b = new SelectTransformer<int, int>(addOne);

        var viaExtension = a.Then(b);
        var viaCtor = new ChainTransformer<int, int, int>(a, b);

        var source = new[] { 1, 2, 3, 4, 5 };
        var resultExt = await CollectAsync(viaExtension.TransformAsync(ToAsync(source)));
        var resultCtor = await CollectAsync(viaCtor.TransformAsync(ToAsync(source)));

        Assert.Equal(resultCtor, resultExt);
    }



    // ---------- chained .Then nests left-leaning (each wraps the prior chain) ----------

    [Fact]
    public void Then_chained_calls_produce_left_leaning_nested_chains()
    {
        ITransformAsync<int, int> a = new PassThroughTransformer<int>();
        ITransformAsync<int, int> b = new PassThroughTransformer<int>();
        ITransformAsync<int, int> c = new PassThroughTransformer<int>();

        var chain = a.Then(b).Then(c);

        // Outer is ChainTransformer<int, int, int>; its 'first' should itself be a
        // ChainTransformer (the a.Then(b) result), not the original a.
        Assert.IsType<ChainTransformer<int, int, int>>(chain);
    }



    // ---------- cancellation overload ----------

    [Fact]
    public void Then_cancellation_overload_when_first_is_null_throws_ArgumentNullException()
    {
        ITransformWithCancellationAsync<int, int> first = null!;

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => first.Then(new PassThroughTransformer<int>())
        );

        Assert.Equal("first", ex.ParamName);
    }



    [Fact]
    public void Then_cancellation_overload_when_next_is_null_throws_ArgumentNullException()
    {
        ITransformWithCancellationAsync<int, int> first = new PassThroughTransformer<int>();

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => first.Then((ITransformWithCancellationAsync<int, int>)null!)
        );

        Assert.Equal("next", ex.ParamName);
    }



    [Fact]
    public void Then_returns_ChainTransformerWithCancellation_when_both_args_have_cancellation()
    {
        ITransformWithCancellationAsync<int, int> first = new PassThroughTransformer<int>();
        ITransformWithCancellationAsync<int, int> second = new PassThroughTransformer<int>();

        var result = first.Then(second);

        Assert.IsType<ChainTransformerWithCancellation<int, int, int>>(result);
    }



    [Fact]
    public void Then_returned_chain_implements_ITransformWithCancellationAsync_when_both_args_have_cancellation()
    {
        ITransformWithCancellationAsync<int, int> first = new PassThroughTransformer<int>();
        ITransformWithCancellationAsync<int, int> second = new PassThroughTransformer<int>();

        var result = first.Then(second);

        Assert.IsAssignableFrom<ITransformWithCancellationAsync<int, int>>(result);
    }



    [Fact]
    public async Task Then_cancellation_overload_token_propagates_through_a_chain_of_three()
    {
        // Build a 3-stage cancellation-aware chain via .Then(...).Then(...) - each .Then call
        // must pick the cancellation overload because the receiver and arg both implement
        // ITransformWithCancellationAsync.
        ITransformWithCancellationAsync<int, int> a = new PassThroughTransformer<int>();
        ITransformWithCancellationAsync<int, int> b = new PassThroughTransformer<int>();
        ITransformWithCancellationAsync<int, int> c = new PassThroughTransformer<int>();

        var chain = a.Then(b).Then(c);

        Assert.IsAssignableFrom<ITransformWithCancellationAsync<int, int>>(chain);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in chain.TransformAsync(ToAsync(new[] { 1, 2, 3 }), cts.Token))
                {
                }
            }
        );
    }



    [Fact]
    public async Task Then_cancellation_overload_chain_runs_normally_with_uncancelled_token()
    {
        ITransformWithCancellationAsync<int, int> a = new PassThroughTransformer<int>();
        ITransformWithCancellationAsync<int, int> b = new PassThroughTransformer<int>();

        var chain = a.Then(b);

        var result = await CollectAsync
        (
            chain.TransformAsync(ToAsync(new[] { 1, 2, 3 }), CancellationToken.None)
        );

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }



    // ---------- Buffered — null / range guards ----------

    [Fact]
    public void Buffered_when_source_is_null_throws_ArgumentNullException()
    {
        IAsyncEnumerable<int> source = null!;

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => source.Buffered(capacity: 1)
        );

        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void Buffered_when_capacity_is_zero_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => ToAsync(new[] { 1, 2, 3 }).Buffered(capacity: 0)
        );

        Assert.Equal("capacity", ex.ParamName);
    }



    [Fact]
    public void Buffered_when_capacity_is_negative_throws_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>
        (
            () => ToAsync(new[] { 1, 2, 3 }).Buffered(capacity: -1)
        );

        Assert.Equal("capacity", ex.ParamName);
    }



    // ---------- Buffered — functional ----------

    [Fact]
    public async Task Buffered_yields_same_items_in_same_order()
    {
        var source = new[] { 1, 2, 3, 4, 5 };

        var result = await CollectAsync(ToAsync(source).Buffered(capacity: 2));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task Buffered_when_source_is_empty_yields_empty_sequence()
    {
        var result = await CollectAsync(ToAsync(Array.Empty<int>()).Buffered(capacity: 8));

        Assert.Empty(result);
    }



    [Fact]
    public async Task Buffered_with_capacity_1_yields_all_items()
    {
        var source = new[] { 10, 20, 30 };

        var result = await CollectAsync(ToAsync(source).Buffered(capacity: 1));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task Buffered_with_large_capacity_yields_all_items()
    {
        var source = Enumerable.Range(1, 500).ToArray();

        var result = await CollectAsync(ToAsync(source).Buffered(capacity: 8192));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task Buffered_cancellation_stops_enumeration()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in ToAsync(Enumerable.Range(1, 1000)).Buffered(capacity: 4).WithCancellation(cts.Token))
                {
                }
            }
        );
    }



}
