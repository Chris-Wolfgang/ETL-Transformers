using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// Covers the opt-in item-error handling shared by the delegate-invoking transformers: the default
/// stays "propagate", <see cref="ItemErrorAction.Skip"/> drops the failing item and counts it, and
/// <see cref="ItemErrorAction.Abort"/> re-throws without counting.
/// </summary>
public class DelegateTransformerErrorPolicyTests
{
    private static DelegateTransformerOptions SkipAll() =>
        new() { ErrorPolicy = _ => ItemErrorAction.Skip };


    private static DelegateTransformerOptions AbortAll() =>
        new() { ErrorPolicy = _ => ItemErrorAction.Abort };



    // ---------- DelegateTransformerOptions ----------

    [Fact]
    public void Options_default_ErrorPolicy_returns_Abort()
    {
        var options = new DelegateTransformerOptions();

        Assert.Equal
        (
            ItemErrorAction.Abort,
            options.ErrorPolicy(new ItemErrorContext(1, new InvalidOperationException()))
        );
    }



    [Fact]
    public void Options_when_ErrorPolicy_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new DelegateTransformerOptions { ErrorPolicy = null! }
        );
    }



    // ---------- SelectTransformer ----------

    [Fact]
    public async Task SelectTransformer_by_default_propagates_a_throwing_selector()
    {
        var sut = new SelectTransformer<int, string>(i => i == 2 ? throw new InvalidOperationException("boom") : $"v{i}");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal("boom", ex.Message);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SelectTransformer_when_policy_skips_drops_the_item_and_counts_it()
    {
        var sut = new SelectTransformer<int, string>
        (
            i => i == 2 ? throw new InvalidOperationException("boom") : $"v{i}",
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1", "v3" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SelectTransformer_when_policy_aborts_rethrows_without_counting()
    {
        var sut = new SelectTransformer<int, string>
        (
            i => i == 2 ? throw new InvalidOperationException("boom") : $"v{i}",
            AbortAll()
        );

        await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SelectTransformer_reports_the_one_based_item_number_of_the_failure()
    {
        var seen = new List<long>();
        var options = new DelegateTransformerOptions
        {
            ErrorPolicy = context =>
            {
                seen.Add(context.ItemNumber);
                return ItemErrorAction.Skip;
            }
        };
        var sut = new SelectTransformer<int, string>
        (
            i => i % 2 == 0 ? throw new InvalidOperationException() : $"v{i}",
            options
        );

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        // The second and fourth items failed; numbering is one-based, matching the fleet convention.
        Assert.Equal(new[] { 2L, 4L }, seen);
    }



    [Fact]
    public async Task SelectTransformer_error_count_accumulates_across_enumerations()
    {
        var sut = new SelectTransformer<int, string>
        (
            i => i == 2 ? throw new InvalidOperationException() : $"v{i}",
            SkipAll()
        );

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2 })));
        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2 })));

        Assert.Equal(2, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SelectTransformer_with_async_selector_honours_the_skip_policy()
    {
        var sut = new SelectTransformer<int, string>
        (
            async i =>
            {
                await Task.Yield();
                return i == 2 ? throw new InvalidOperationException() : $"v{i}";
            },
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1", "v3" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public void SelectTransformer_when_options_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectTransformer<int, string>(i => $"v{i}", null!)
        );

        Assert.Equal("options", ex.ParamName);
    }



    // ---------- SelectManyTransformer ----------

    [Fact]
    public async Task SelectManyTransformer_when_the_selector_call_throws_and_policy_skips_drops_the_item()
    {
        var sut = new SelectManyTransformer<int, string>
        (
            i => i == 2 ? throw new InvalidOperationException() : new[] { $"v{i}a", $"v{i}b" },
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1a", "v1b", "v3a", "v3b" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SelectManyTransformer_when_the_inner_sequence_throws_midway_keeps_what_it_already_yielded()
    {
        // The selector's sequence is lazy, so this throw surfaces on the second MoveNext rather
        // than when the selector is called. Skip abandons the rest of THAT item's expansion and
        // moves on; results already yielded from it stay.
        static IEnumerable<string> Expand(int i)
        {
            yield return $"v{i}a";
            if (i == 2)
            {
                throw new InvalidOperationException();
            }

            yield return $"v{i}b";
        }

        var sut = new SelectManyTransformer<int, string>(Expand, SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1a", "v1b", "v2a", "v3a", "v3b" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SelectManyTransformer_by_default_propagates_a_throwing_inner_sequence()
    {
        static IEnumerable<string> Expand(int i)
        {
            yield return $"v{i}a";
            if (i == 2)
            {
                throw new InvalidOperationException("boom");
            }
        }

        var sut = new SelectManyTransformer<int, string>(Expand);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal("boom", ex.Message);
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task SelectManyTransformer_with_async_selector_honours_the_skip_policy()
    {
        static async IAsyncEnumerable<string> ExpandAsync(int i)
        {
            await Task.Yield();
            yield return $"v{i}a";
            if (i == 2)
            {
                throw new InvalidOperationException();
            }

            yield return $"v{i}b";
        }

        var sut = new SelectManyTransformer<int, string>(ExpandAsync, SkipAll());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1a", "v1b", "v2a", "v3a", "v3b" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public void SelectManyTransformer_when_options_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectManyTransformer<int, string>(i => new[] { $"v{i}" }, null!)
        );

        Assert.Equal("options", ex.ParamName);
    }
}
