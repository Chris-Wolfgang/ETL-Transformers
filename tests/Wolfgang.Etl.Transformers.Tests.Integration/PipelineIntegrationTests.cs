using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Transformers;
using Xunit;

namespace Wolfgang.Etl.Transformers.Tests.Integration;

/// <summary>
/// End-to-end pipeline tests that compose multiple transformers together.
/// Each test exercises a realistic multi-stage transformation scenario
/// rather than a single transformer in isolation.
/// </summary>
public class PipelineIntegrationTests
{
    [Fact]
    public async Task Filter_project_limit_pipeline_yields_correct_results()
    {
        // Arrange: integers 1-20, keep evens, square them, take first 4
        var source = Enumerable.Range(1, 20);
        var where = new WhereTransformer<int>(x => x % 2 == 0);
        var select = new SelectTransformer<int, int>(x => x * x);
        var take = new TakeTransformer<int>(4);

        // Act
        var step1 = where.TransformAsync(ToAsync(source));
        var step2 = select.TransformAsync(step1);
        var step3 = take.TransformAsync(step2);
        var result = await CollectAsync(step3);

        // Assert: evens are 2,4,6,8,... → squared: 4,16,36,64
        Assert.Equal(new[] { 4, 16, 36, 64 }, result);
    }



    [Fact]
    public async Task Then_extension_composes_two_transformers_in_order()
    {
        // Arrange: double then stringify via Then()
        var first = new SelectTransformer<int, int>(x => x * 2);
        var second = new SelectTransformer<int, string>(x => $"item-{x}");
        var pipeline = first.Then(second);

        // Act
        var result = await CollectAsync(pipeline.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        // Assert
        Assert.Equal(new[] { "item-2", "item-4", "item-6" }, result);
    }



    [Fact]
    public async Task Then_extension_three_stage_chain_applies_all_in_order()
    {
        // Arrange: filter odds, double, take 2
        var filter = new WhereTransformer<int>(x => x % 2 != 0);
        var doubler = new SelectTransformer<int, int>(x => x * 2);
        var limiter = new TakeTransformer<int>(2);
        var pipeline = filter.Then(doubler).Then(limiter);

        // Act
        var result = await CollectAsync(pipeline.TransformAsync(ToAsync(Enumerable.Range(1, 10))));

        // Assert: odds are 1,3,5,... → doubled: 2,6,10,... → take 2: 2,6
        Assert.Equal(new[] { 2, 6 }, result);
    }



    [Fact]
    public async Task Buffered_extension_preserves_all_items_in_order()
    {
        // Arrange: filter → buffer → select
        var source = Enumerable.Range(1, 50);
        var where = new WhereTransformer<int>(x => x % 5 == 0);
        var buffered = TransformerExtensions.Buffered(where.TransformAsync(ToAsync(source)), capacity: 4);
        var select = new SelectTransformer<int, string>(x => $"v{x}");

        // Act
        var result = await CollectAsync(select.TransformAsync(buffered));

        // Assert: multiples of 5 in range 1-50 = 5,10,...,50 → "v5","v10",...,"v50"
        var expected = Enumerable.Range(1, 10).Select(i => $"v{i * 5}").ToArray();
        Assert.Equal(expected, result);
    }



    [Fact]
    public async Task Skip_and_take_pipeline_windows_a_sequence()
    {
        // Arrange: page 2 of 3 from items 1-9 (page size 3)
        var source = Enumerable.Range(1, 9);
        var skip = new SkipTransformer<int>(3);
        var take = new TakeTransformer<int>(3);

        // Act
        var result = await CollectAsync(take.TransformAsync(skip.TransformAsync(ToAsync(source))));

        // Assert: skip 3 → [4,5,6,7,8,9], take 3 → [4,5,6]
        Assert.Equal(new[] { 4, 5, 6 }, result);
    }



    [Fact]
    public async Task Distinct_then_where_pipeline_deduplicates_then_filters()
    {
        // Arrange
        var source = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 };
        var distinct = new DistinctTransformer<int>();
        var where = new WhereTransformer<int>(x => x > 3);

        // Act
        var result = await CollectAsync(where.TransformAsync(distinct.TransformAsync(ToAsync(source))));

        // Assert: distinct → [3,1,4,5,9,2,6], then >3 → [4,5,9,6]
        Assert.Equal(new[] { 4, 5, 9, 6 }, result);
    }



    [Fact]
    public async Task SelectMany_then_where_pipeline_flattens_then_filters()
    {
        // Arrange: expand each int to [n, n*10], then keep multiples of 10
        var source = new[] { 1, 2, 3 };
        var selectMany = new SelectManyTransformer<int, int>(x => new[] { x, x * 10 });
        var where = new WhereTransformer<int>(x => x % 10 == 0);

        // Act
        var result = await CollectAsync(where.TransformAsync(selectMany.TransformAsync(ToAsync(source))));

        // Assert: expand → [1,10,2,20,3,30], keep %10==0 → [10,20,30]
        Assert.Equal(new[] { 10, 20, 30 }, result);
    }



    [Fact]
    public async Task Chunk_then_select_pipeline_groups_and_projects()
    {
        // Arrange: chunk into groups of 3, then sum each group
        var source = Enumerable.Range(1, 9);
        var chunk = new ChunkTransformer<int>(3);
        var select = new SelectTransformer<int[], int>(arr => arr.Sum());

        // Act
        var result = await CollectAsync(select.TransformAsync(chunk.TransformAsync(ToAsync(source))));

        // Assert: [1,2,3]=6, [4,5,6]=15, [7,8,9]=24
        Assert.Equal(new[] { 6, 15, 24 }, result);
    }



    [Fact]
    public async Task PassThrough_in_the_middle_of_a_pipeline_is_transparent()
    {
        // Arrange: where → passthrough → select
        var where = new WhereTransformer<int>(x => x % 2 == 0);
        var passThrough = new PassThroughTransformer<int>();
        var select = new SelectTransformer<int, int>(x => x * 3);

        // Act
        var step1 = where.TransformAsync(ToAsync(Enumerable.Range(1, 6)));
        var step2 = passThrough.TransformAsync(step1);
        var result = await CollectAsync(select.TransformAsync(step2));

        // Assert: evens [2,4,6] × 3 = [6,12,18]
        Assert.Equal(new[] { 6, 12, 18 }, result);
    }



    [Fact]
    public async Task ProgressReporting_in_pipeline_fires_callback_for_every_item()
    {
        // Arrange
        var seen = new List<int>();
        var where = new WhereTransformer<int>(x => x % 2 == 0);
        var progress = new ProgressReportingTransformer<int>(x => seen.Add(x));
        var take = new TakeTransformer<int>(3);

        // Act
        var step1 = where.TransformAsync(ToAsync(Enumerable.Range(1, 10)));
        var step2 = progress.TransformAsync(step1);
        var result = await CollectAsync(take.TransformAsync(step2));

        // Assert: take stops after 3 items so progress only sees the 3 items actually consumed
        Assert.Equal(new[] { 2, 4, 6 }, result);
        Assert.Equal(new[] { 2, 4, 6 }, seen);
    }



    [Fact]
    public async Task Cancellation_propagates_through_a_multi_stage_pipeline()
    {
        // Arrange: a slow source + filter + select; cancel after 3 items
        using var cts = new CancellationTokenSource();
        var yielded = 0;

        var passThrough = new PassThroughTransformer<int>();
        var select = new SelectTransformer<int, int>(x => x * x);

        // PassThroughTransformer.TransformAsync(source, token) calls source.WithCancellation(token)
        // internally, which propagates the token into SlowSource via [EnumeratorCancellation].
        var slowSource = SlowSource(100, () =>
        {
            yielded++;
            if (yielded >= 3) cts.Cancel();
        });
        var source = passThrough.TransformAsync(slowSource, cts.Token);

        // Act
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in select.TransformAsync(source))
            {
            }
        });

        // Assert: cancelled early, not all 100 items processed
        Assert.True(yielded < 100);
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



    private static async IAsyncEnumerable<int> SlowSource(
        int count,
        Action onItem,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        for (var i = 0; i < count; i++)
        {
            token.ThrowIfCancellationRequested();
            onItem();
            await Task.Yield();
            yield return i;
        }
    }
}
