using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;

namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// Verifies that each <see cref="EtlPipelineOperatorExtensions"/> operator wires the correct
/// transformer into the pipeline (the transformers themselves are covered by their own tests),
/// and that every operator validates its arguments.
/// </summary>
public class EtlPipelineOperatorExtensionsTests
{
    private static IEtlPipeline<T> Pipe<T>(params T[] items)
        where T : notnull
        => EtlPipeline.Create().From(ToAsync(items));

    [Fact]
    public async Task Where_when_sync_predicate_keeps_only_matching_items()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 4).Where(x => x % 2 == 0).AsAsyncEnumerable());

        Assert.Equal(new[] { 2, 4 }, result);
    }

    [Fact]
    public async Task Where_when_async_predicate_keeps_only_matching_items()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 4).Where(x => new ValueTask<bool>(x > 2)).AsAsyncEnumerable());

        Assert.Equal(new[] { 3, 4 }, result);
    }

    [Fact]
    public async Task Select_when_sync_selector_projects_each_item()
    {
        var result = await CollectAsync(Pipe(1, 2, 3).Select(x => x * 10).AsAsyncEnumerable());

        Assert.Equal(new[] { 10, 20, 30 }, result);
    }

    [Fact]
    public async Task Select_when_async_selector_projects_each_item()
    {
        var result = await CollectAsync(Pipe(1, 2, 3).Select(x => new ValueTask<string>($"#{x}")).AsAsyncEnumerable());

        Assert.Equal(new[] { "#1", "#2", "#3" }, result);
    }

    [Fact]
    public async Task SelectMany_when_sync_selector_flattens_results()
    {
        var result = await CollectAsync(Pipe(1, 2).SelectMany(x => new[] { x, x * 10 }).AsAsyncEnumerable());

        Assert.Equal(new[] { 1, 10, 2, 20 }, result);
    }

    [Fact]
    public async Task SelectMany_when_async_selector_flattens_results()
    {
        var result = await CollectAsync(Pipe(1, 2).SelectMany(x => ToAsync(new[] { x, x * 10 })).AsAsyncEnumerable());

        Assert.Equal(new[] { 1, 10, 2, 20 }, result);
    }

    [Fact]
    public async Task Distinct_when_no_comparer_removes_duplicates_preserving_order()
    {
        var result = await CollectAsync(Pipe(1, 2, 2, 3, 1).Distinct().AsAsyncEnumerable());

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public async Task Distinct_when_comparer_supplied_uses_it()
    {
        var result = await CollectAsync(Pipe("a", "A", "b").Distinct(StringComparer.OrdinalIgnoreCase).AsAsyncEnumerable());

        Assert.Equal(new[] { "a", "b" }, result);
    }

    [Fact]
    public async Task DistinctBy_keeps_first_item_per_key()
    {
        var result = await CollectAsync(Pipe("apple", "avocado", "banana").DistinctBy(s => s[0]).AsAsyncEnumerable());

        Assert.Equal(new[] { "apple", "banana" }, result);
    }

    [Fact]
    public async Task Take_yields_only_the_first_count_items()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 4, 5).Take(2).AsAsyncEnumerable());

        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public async Task Skip_skips_the_first_count_items()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 4, 5).Skip(3).AsAsyncEnumerable());

        Assert.Equal(new[] { 4, 5 }, result);
    }

    [Fact]
    public async Task TakeWhile_when_sync_predicate_yields_leading_run()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 1).TakeWhile(x => x < 3).AsAsyncEnumerable());

        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public async Task TakeWhile_when_async_predicate_yields_leading_run()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 1).TakeWhile(x => new ValueTask<bool>(x < 3)).AsAsyncEnumerable());

        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public async Task SkipWhile_when_sync_predicate_skips_leading_run()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 1).SkipWhile(x => x < 3).AsAsyncEnumerable());

        Assert.Equal(new[] { 3, 1 }, result);
    }

    [Fact]
    public async Task SkipWhile_when_async_predicate_skips_leading_run()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 1).SkipWhile(x => new ValueTask<bool>(x < 3)).AsAsyncEnumerable());

        Assert.Equal(new[] { 3, 1 }, result);
    }

    [Fact]
    public async Task Chunk_batches_items_into_fixed_size_lists()
    {
        var result = await CollectAsync(Pipe(1, 2, 3, 4, 5).Chunk(2).AsAsyncEnumerable());

        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { 1, 2 }, result[0]);
        Assert.Equal(new[] { 3, 4 }, result[1]);
        Assert.Equal(new[] { 5 }, result[2]);
    }

    [Fact]
    public async Task Buffered_preserves_items_and_order()
    {
        var result = await CollectAsync(Pipe(1, 2, 3).Buffered(2).AsAsyncEnumerable());

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public async Task Cast_casts_each_item_to_the_target_type()
    {
        var result = await CollectAsync(Pipe<object>("a", "b").Cast<object, string>().AsAsyncEnumerable());

        Assert.Equal(new[] { "a", "b" }, result);
    }

    [Fact]
    public async Task OfType_passes_through_only_items_of_the_target_type()
    {
        var result = await CollectAsync(Pipe<object>("a", 1, "b", 2).OfType<object, string>().AsAsyncEnumerable());

        Assert.Equal(new[] { "a", "b" }, result);
    }

    [Fact]
    public void Operators_when_pipeline_is_null_throw_ArgumentNullException()
    {
        IEtlPipeline<int> nullPipeline = null!;

        Assert.Throws<ArgumentNullException>(() => nullPipeline.Where(_ => true));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Where(_ => new ValueTask<bool>(true)));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Select(x => x));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Select(x => new ValueTask<int>(x)));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SelectMany(x => new[] { x }));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SelectMany(x => ToAsync(new[] { x })));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Distinct());
        Assert.Throws<ArgumentNullException>(() => nullPipeline.DistinctBy(x => x));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Take(1));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Skip(1));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.TakeWhile(_ => true));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.TakeWhile(_ => new ValueTask<bool>(true)));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SkipWhile(_ => true));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SkipWhile(_ => new ValueTask<bool>(true)));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Chunk(1));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Buffered(1));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Cast<int, object>());
        Assert.Throws<ArgumentNullException>(() => nullPipeline.OfType<int, object>());
    }

    [Fact]
    public void Operators_when_delegate_is_null_throw_ArgumentNullException()
    {
        var pipeline = Pipe(1, 2, 3);

        Assert.Throws<ArgumentNullException>(() => pipeline.Where((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.Where((Func<int, ValueTask<bool>>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.Select((Func<int, int>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.Select((Func<int, ValueTask<int>>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.SelectMany((Func<int, IEnumerable<int>>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.SelectMany((Func<int, IAsyncEnumerable<int>>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.DistinctBy((Func<int, int>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.TakeWhile((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.TakeWhile((Func<int, ValueTask<bool>>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.SkipWhile((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() => pipeline.SkipWhile((Func<int, ValueTask<bool>>)null!));
    }
}
