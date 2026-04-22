using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class SelectTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_with_sync_selector_when_selector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectTransformer<int, string>((Func<int, string>)null!)
        );

        Assert.Equal("selector", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_async_selector_when_selector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectTransformer<int, string>((Func<int, CancellationToken, ValueTask<string>>)null!)
        );

        Assert.Equal("selector", ex.ParamName);
    }



    // ---------- basic projection ----------

    [Fact]
    public async Task TransformAsync_sync_selector_projects_each_item()
    {
        var sut = new SelectTransformer<int, string>(i => i.ToString(CultureInfo.InvariantCulture));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "1", "2", "3" }, result);
    }



    [Fact]
    public async Task TransformAsync_async_selector_projects_each_item()
    {
        var sut = new SelectTransformer<int, string>
        (
            (i, _) => new ValueTask<string>(i.ToString(CultureInfo.InvariantCulture))
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "1", "2", "3" }, result);
    }



    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items()
    {
        var sut = new SelectTransformer<int, string>(i => i.ToString(CultureInfo.InvariantCulture));

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- counts ----------

    [Fact]
    public async Task TransformAsync_increments_CurrentItemCount_per_yielded_item()
    {
        var sut = new SelectTransformer<int, int>(i => i * 2);

        await CollectAsync(sut.TransformAsync(ToAsync(Enumerable.Range(0, 5))));

        Assert.Equal(5, sut.CurrentItemCount);
    }



    [Fact]
    public async Task TransformAsync_when_SkipItemCount_is_set_skips_first_n_items()
    {
        var sut = new SelectTransformer<int, int>(i => i * 10) { SkipItemCount = 2 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 })));

        Assert.Equal(new[] { 30, 40, 50 }, result);
        Assert.Equal(3, sut.CurrentItemCount);
        Assert.Equal(2, sut.CurrentSkippedItemCount);
    }



    [Fact]
    public async Task TransformAsync_when_MaximumItemCount_is_set_stops_after_max_items()
    {
        var sut = new SelectTransformer<int, int>(i => i * 10) { MaximumItemCount = 3 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Enumerable.Range(1, 10))));

        Assert.Equal(new[] { 10, 20, 30 }, result);
        Assert.Equal(3, sut.CurrentItemCount);
    }



    [Fact]
    public async Task TransformAsync_when_Skip_and_Max_are_both_set_applies_skip_then_max()
    {
        var sut = new SelectTransformer<int, int>(i => i * 10)
        {
            SkipItemCount = 2,
            MaximumItemCount = 3,
        };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Enumerable.Range(1, 10))));

        Assert.Equal(new[] { 30, 40, 50 }, result);
        Assert.Equal(3, sut.CurrentItemCount);
        Assert.Equal(2, sut.CurrentSkippedItemCount);
    }



    // ---------- cancellation ----------

    [Fact]
    public async Task TransformAsync_when_cancellation_already_requested_throws_OperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var sut = new SelectTransformer<int, int>(i => i);

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(ToAsync(new[] { 1, 2, 3 }), cts.Token))
                {
                }
            }
        );
    }



    [Fact]
    public async Task TransformAsync_async_selector_receives_same_token_passed_to_TransformAsync()
    {
        using var cts = new CancellationTokenSource();
        CancellationToken observed = default;

        var sut = new SelectTransformer<int, int>
        (
            (i, token) =>
            {
                observed = token;
                return new ValueTask<int>(i);
            }
        );

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 }), cts.Token));

        Assert.Equal(cts.Token, observed);
    }



    // ---------- selector exceptions ----------

    [Fact]
    public async Task TransformAsync_when_sync_selector_throws_exception_propagates()
    {
        var sut = new SelectTransformer<int, int>
        (
            _ => throw new InvalidOperationException("boom")
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("boom", ex.Message);
    }



    [Fact]
    public async Task TransformAsync_when_async_selector_throws_exception_propagates()
    {
        var sut = new SelectTransformer<int, int>
        (
            (_, _) => throw new InvalidOperationException("async-boom")
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("async-boom", ex.Message);
    }



    // ---------- type inheritance sanity ----------

    [Fact]
    public void SelectTransformer_inherits_TransformerBase()
    {
        var sut = new SelectTransformer<int, int>(i => i);

        Assert.IsAssignableFrom<TransformerBase<int, int, Report>>(sut);
    }



    [Fact]
    public void SelectTransformer_implements_ITransformWithProgressAndCancellationAsync()
    {
        var sut = new SelectTransformer<int, int>(i => i);

        Assert.IsAssignableFrom<ITransformWithProgressAndCancellationAsync<int, int, Report>>(sut);
    }



    // ---------- progress report ----------

    [Fact]
    public async Task TransformAsync_with_progress_reports_Report_with_current_item_count_at_completion()
    {
        var sut = new SelectTransformer<int, int>(i => i);
        var lastReport = new Report(0);
        var progress = new Progress<Report>(r => lastReport = r);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4, 5 }), progress));

        // Let the Progress<T> SynchronizationContext drain.
        await Task.Delay(100);

        Assert.Equal(5, result.Count);
        Assert.Equal(5, lastReport.CurrentItemCount);
    }



    // ---------- type conversion use case ----------

    [Fact]
    public async Task TransformAsync_supports_changing_type_from_source_to_destination()
    {
        var sut = new SelectTransformer<int, string>
        (
            i => $"value={i}"
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 10, 20 })));

        Assert.Equal(new[] { "value=10", "value=20" }, result);
    }



    // ---------- helpers ----------

    private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }



    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> items)
    {
        var list = new List<T>();
        await foreach (var item in items)
        {
            list.Add(item);
        }
        return list;
    }
}
