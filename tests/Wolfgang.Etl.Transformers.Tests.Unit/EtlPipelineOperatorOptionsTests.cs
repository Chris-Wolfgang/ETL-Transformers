using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// The <see cref="DelegateTransformerOptions"/> overloads of the fluent operators: a throwing
/// delegate is skipped by the supplied policy instead of aborting the run.
/// </summary>
public class EtlPipelineOperatorOptionsTests
{
    private sealed class ThrowingComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;

        public int GetHashCode(int obj) => obj == 2 ? throw new InvalidOperationException("bad") : obj;
    }



    private sealed class Counter
    {
        public int Skipped { get; private set; }

        public DelegateTransformerOptions Options => new()
        {
            ErrorPolicy = _ =>
            {
                Skipped++;
                return ItemErrorAction.Skip;
            }
        };
    }



    private sealed class SyncProgress : IProgress<int>
    {
        private readonly List<int> _reports;

        public SyncProgress(List<int> reports) => _reports = reports;

        public void Report(int value) => _reports.Add(value);
    }



    private static IEtlPipeline<int> Pipe(params int[] items) => EtlPipeline.Create().From(ToAsync(items));



    private static int Boom(int i) => i == 2 ? throw new InvalidOperationException("bad") : i;



    private static ValueTask<int> BoomAsync(int i) => i == 2 ? throw new InvalidOperationException("bad") : new ValueTask<int>(i);



    [Fact]
    public async Task Where_with_options_skips_a_throwing_predicate()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3).Where(i => Boom(i) > 0, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task Where_async_with_options_skips_a_throwing_predicate()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3).Where(async i => await BoomAsync(i) > 0, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task Select_with_options_skips_a_throwing_selector()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3).Select(Boom, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task Select_async_with_options_skips_a_throwing_selector()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3).Select(BoomAsync, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task SelectMany_with_options_skips_a_throwing_selector()
    {
        var c = new Counter();
        var result = await CollectAsync
        (
            Pipe(1, 2, 3).SelectMany(i => new[] { Boom(i), Boom(i) }, c.Options).AsAsyncEnumerable()
        );
        Assert.Equal(new[] { 1, 1, 3, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task SelectMany_async_with_options_skips_a_throwing_selector()
    {
        var c = new Counter();
        var result = await CollectAsync
        (
            Pipe(1, 2, 3).SelectMany(i => ToAsync(new[] { Boom(i) }), c.Options).AsAsyncEnumerable()
        );
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task Distinct_with_options_skips_an_item_whose_comparer_throws()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3, 1).Distinct(new ThrowingComparer(), c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task DistinctBy_with_options_skips_an_item_whose_key_selector_throws()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3, 1).DistinctBy(Boom, comparer: null, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task TakeWhile_with_options_skips_a_throwing_predicate()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3).TakeWhile(i => Boom(i) < 10, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task TakeWhile_async_with_options_skips_a_throwing_predicate()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3).TakeWhile(async i => await BoomAsync(i) < 10, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task SkipWhile_with_options_skips_a_throwing_predicate()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3, 4).SkipWhile(i => Boom(i) < 3, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 3, 4 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task SkipWhile_async_with_options_skips_a_throwing_predicate()
    {
        var c = new Counter();
        var result = await CollectAsync(Pipe(1, 2, 3, 4).SkipWhile(async i => await BoomAsync(i) < 3, c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 3, 4 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task Cast_with_options_skips_an_item_that_cannot_be_cast()
    {
        var c = new Counter();
        var source = EtlPipeline.Create().From(ToAsync(new object[] { 1, "x", 3 }));
        var result = await CollectAsync(source.Cast<object, int>(c.Options).AsAsyncEnumerable());
        Assert.Equal(new[] { 1, 3 }, result);
        Assert.Equal(1, c.Skipped);
    }



    [Fact]
    public async Task Chunk_with_progress_reports_the_running_count()
    {
        var reports = new List<int>();
        var result = await CollectAsync(Pipe(1, 2, 3, 4, 5).Chunk(2, new SyncProgress(reports)).AsAsyncEnumerable());
        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { 2, 4, 5 }, reports);
    }



    [Fact]
    public void Options_overloads_when_pipeline_is_null_throw_ArgumentNullException()
    {
        IEtlPipeline<int> nullPipeline = null!;
        var o = new DelegateTransformerOptions();

        Assert.Throws<ArgumentNullException>(() => nullPipeline.Where(_ => true, o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Where(_ => new ValueTask<bool>(true), o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Select(_ => 0, o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Select(_ => new ValueTask<int>(0), o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SelectMany(_ => new[] { 0 }, o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SelectMany(_ => ToAsync(new[] { 0 }), o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Distinct(null, o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.DistinctBy(_ => 0, null, o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.TakeWhile(_ => true, o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.TakeWhile(_ => new ValueTask<bool>(true), o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SkipWhile(_ => true, o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.SkipWhile(_ => new ValueTask<bool>(true), o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Cast<int, object>(o));
        Assert.Throws<ArgumentNullException>(() => nullPipeline.Chunk(2, null));
    }



    [Fact]
    public void Options_overloads_when_options_is_null_throw_ArgumentNullException()
    {
        var pipeline = Pipe(1);
        DelegateTransformerOptions o = null!;

        Assert.Throws<ArgumentNullException>(() => pipeline.Where(_ => true, o));
        Assert.Throws<ArgumentNullException>(() => pipeline.Select(_ => 0, o));
        Assert.Throws<ArgumentNullException>(() => pipeline.Cast<int, object>(o));
    }
}
