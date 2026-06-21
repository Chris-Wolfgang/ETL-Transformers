using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class ChainTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_when_first_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new ChainTransformer<int, int, int>(null!, new PassThroughTransformer<int>())
        );

        Assert.Equal("first", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_second_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new ChainTransformer<int, int, int>(new PassThroughTransformer<int>(), null!)
        );

        Assert.Equal("second", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new ChainTransformer<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

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
        var sut = new ChainTransformer<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- two-stage same-type composition ----------

    [Fact]
    public async Task TransformAsync_two_stage_same_type_chain_applies_both_in_order()
    {
        Func<int, int> doubleIt = i => i * 2;
        Func<int, int> addOne = i => i + 1;

        var sut = new ChainTransformer<int, int, int>
        (
            new SelectTransformer<int, int>(doubleIt),
            new SelectTransformer<int, int>(addOne)
        );

        // (i * 2) + 1
        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 3, 5, 7 }, result);
    }



    // ---------- two-stage cross-type composition ----------

    [Fact]
    public async Task TransformAsync_two_stage_cross_type_chain_applies_both_in_order()
    {
        Func<int, int> doubleIt = i => i * 2;
        Func<int, string> stringify = i => i.ToString(CultureInfo.InvariantCulture);

        var sut = new ChainTransformer<int, int, string>
        (
            new SelectTransformer<int, int>(doubleIt),
            new SelectTransformer<int, string>(stringify)
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "2", "4", "6" }, result);
    }



    // ---------- chain with filter (Where) drops items between stages ----------

    [Fact]
    public async Task TransformAsync_with_filter_in_first_stage_drops_items_before_second_runs()
    {
        var secondStageCalls = 0;
        Func<int, bool> isEven = i => i % 2 == 0;
        Func<int, int> identity = i =>
        {
            secondStageCalls++;
            return i;
        };

        var sut = new ChainTransformer<int, int, int>
        (
            new WhereTransformer<int>(isEven),
            new SelectTransformer<int, int>(identity)
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5, 6 })));

        Assert.Equal(new[] { 2, 4, 6 }, result);
        Assert.Equal(3, secondStageCalls);  // only the 3 even items reach the second stage
    }



    // ---------- chain with fan-out (SelectMany) yields more items than source ----------

    [Fact]
    public async Task TransformAsync_with_fan_out_in_first_stage_yields_more_items()
    {
        Func<int, IEnumerable<int>> fanOut = i => new[] { i, i * 10 };
        Func<int, int> negate = i => -i;

        var sut = new ChainTransformer<int, int, int>
        (
            new SelectManyTransformer<int, int>(fanOut),
            new SelectTransformer<int, int>(negate)
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { -1, -10, -2, -20, -3, -30 }, result);
    }



    // ---------- exception propagation from first stage ----------

    [Fact]
    public async Task TransformAsync_when_first_stage_throws_exception_propagates()
    {
        Func<int, int> throwIt = _ => throw new InvalidOperationException("first-boom");

        var sut = new ChainTransformer<int, int, int>
        (
            new SelectTransformer<int, int>(throwIt),
            new PassThroughTransformer<int>()
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("first-boom", ex.Message);
    }



    // ---------- exception propagation from second stage ----------

    [Fact]
    public async Task TransformAsync_when_second_stage_throws_exception_propagates()
    {
        Func<int, int> throwIt = _ => throw new InvalidOperationException("second-boom");

        var sut = new ChainTransformer<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new SelectTransformer<int, int>(throwIt)
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("second-boom", ex.Message);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_through_chain()
    {
        Func<int, int> addOne = i => i + 1;
        Func<int, int> timesTen = i => i * 10;

        var sut = new ChainTransformer<int, int, int>
        (
            new SelectTransformer<int, int>(addOne),
            new SelectTransformer<int, int>(timesTen)
        );

        var source = Enumerable.Range(1, 50).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source.Select(i => (i + 1) * 10), result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void ChainTransformer_implements_ITransformAsync()
    {
        var sut = new ChainTransformer<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



    // ---------- nested chains (3+ stages by manual nesting) ----------

    [Fact]
    public async Task TransformAsync_three_stage_pipeline_via_nested_chains_works()
    {
        Func<int, int> addOne = i => i + 1;
        Func<int, int> timesTen = i => i * 10;
        Func<int, string> stringify = i => i.ToString(CultureInfo.InvariantCulture);

        // ((i + 1) * 10).ToString()
        var inner = new ChainTransformer<int, int, int>
        (
            new SelectTransformer<int, int>(addOne),
            new SelectTransformer<int, int>(timesTen)
        );
        var outer = new ChainTransformer<int, int, string>
        (
            inner,
            new SelectTransformer<int, string>(stringify)
        );

        var result = await CollectAsync(outer.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "20", "30", "40" }, result);
    }



}
