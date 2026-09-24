using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// Item-error handling for <see cref="CastTransformer{TSource, TDestination}"/>. Unlike the other
/// transformers taking <see cref="DelegateTransformerOptions"/>, there is no caller-supplied
/// delegate here - the failure being handled is the cast itself.
/// </summary>
public class CastTransformerErrorPolicyTests
{
    private static DelegateTransformerOptions SkipAll() =>
        new() { ErrorPolicy = _ => ItemErrorAction.Skip };



    [Fact]
    public async Task CastTransformer_by_default_throws_on_the_first_mismatch()
    {
        var sut = new CastTransformer<object, string>();

        await Assert.ThrowsAsync<InvalidCastException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new object[] { "a", 1, "b" })))
        );

        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task CastTransformer_when_policy_skips_drops_the_mismatch_and_counts_it()
    {
        var sut = new CastTransformer<object, string>(SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new object[] { "a", 1, "b" })));

        Assert.Equal(new[] { "a", "b" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task CastTransformer_hands_the_policy_an_InvalidCastException_and_the_item_number()
    {
        var seen = new List<ItemErrorContext>();
        var options = new DelegateTransformerOptions
        {
            ErrorPolicy = context =>
            {
                seen.Add(context);
                return ItemErrorAction.Skip;
            }
        };
        var sut = new CastTransformer<object, string>(options);

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new object[] { "a", 1, "b", 2 })));

        Assert.Equal(new[] { 2L, 4L }, seen.ConvertAll(c => c.ItemNumber));
        Assert.All(seen, c => Assert.IsType<InvalidCastException>(c.Exception));
    }



    [Fact]
    public async Task CastTransformer_when_policy_aborts_rethrows_without_counting()
    {
        var sut = new CastTransformer<object, string>
        (
            new DelegateTransformerOptions { ErrorPolicy = _ => ItemErrorAction.Abort }
        );

        await Assert.ThrowsAsync<InvalidCastException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new object[] { "a", 1 })))
        );

        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task CastTransformer_with_a_skip_policy_differs_from_OfType_by_accounting()
    {
        // Both drop the mismatches; only the policy form records that they happened. This is the
        // reason both exist, so it is worth pinning.
        var source = new object[] { "a", 1, "b" };

        var ofType = new OfTypeTransformer<object, string>();
        var cast = new CastTransformer<object, string>(SkipAll());

        var ofTypeResult = await CollectAsync(ofType.TransformAsync(ToAsync(source)));
        var castResult = await CollectAsync(cast.TransformAsync(ToAsync(source)));

        Assert.Equal(ofTypeResult, castResult);
        Assert.Equal(1, cast.CurrentErrorItemCount);
    }



    [Fact]
    public void CastTransformer_when_options_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new CastTransformer<object, string>(null!)
        );

        Assert.Equal("options", ex.ParamName);
    }
}
