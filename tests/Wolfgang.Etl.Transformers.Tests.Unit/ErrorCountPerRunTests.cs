using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// <see cref="IReportsItemErrors.CurrentErrorItemCount"/> means "so far in the current run": running a
/// transformer a second time reports only that run's skips, not the first run's again.
/// </summary>
public class ErrorCountPerRunTests
{
    private static readonly DelegateTransformerOptions SkipAll = new() { ErrorPolicy = _ => ItemErrorAction.Skip };



    private static int FailOnTwo(int i) => i == 2 ? throw new InvalidOperationException("bad") : i;



    private static ValueTask<int> FailOnTwoAsync(int i) => i == 2 ? throw new InvalidOperationException("bad") : new ValueTask<int>(i);



    private sealed class ThrowingComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;

        public int GetHashCode(int obj) => obj == 2 ? throw new InvalidOperationException("bad") : obj;
    }



    public static TheoryData<string> Shapes() => new()
    {
        "Select-sync", "Select-async",
        "Where-sync", "Where-async",
        "SelectMany-sync", "SelectMany-async",
        "SkipWhile-sync", "SkipWhile-async",
        "TakeWhile-sync", "TakeWhile-async",
        "Distinct", "DistinctBy",
    };



    private static (ITransformAsync<int, int> Sut, IReportsItemErrors Reporter) Build(string name) => name switch // the final arm is "DistinctBy"
    {
        "Select-sync" => Pair(new SelectTransformer<int, int>(FailOnTwo, SkipAll)),
        "Select-async" => Pair(new SelectTransformer<int, int>(FailOnTwoAsync, SkipAll)),
        "Where-sync" => Pair(new WhereTransformer<int>(i => FailOnTwo(i) > 0, SkipAll)),
        "Where-async" => Pair(new WhereTransformer<int>(async i => await FailOnTwoAsync(i) > 0, SkipAll)),
        "SelectMany-sync" => Pair(new SelectManyTransformer<int, int>(i => new[] { FailOnTwo(i) }, SkipAll)),
        "SelectMany-async" => Pair(new SelectManyTransformer<int, int>(i => ToAsync(new[] { FailOnTwo(i) }), SkipAll)),
        "SkipWhile-sync" => Pair(new SkipWhileTransformer<int>(i => FailOnTwo(i) < 5, SkipAll)),
        "SkipWhile-async" => Pair(new SkipWhileTransformer<int>(async i => await FailOnTwoAsync(i) < 5, SkipAll)),
        "TakeWhile-sync" => Pair(new TakeWhileTransformer<int>(i => FailOnTwo(i) > 0, SkipAll)),
        "TakeWhile-async" => Pair(new TakeWhileTransformer<int>(async i => await FailOnTwoAsync(i) > 0, SkipAll)),
        "Distinct" => Pair(new DistinctTransformer<int>(new ThrowingComparer(), SkipAll)),
        _ => Pair(new DistinctByTransformer<int, int>(FailOnTwo, comparer: null, SkipAll)),
    };



    private static (ITransformAsync<int, int>, IReportsItemErrors) Pair<TSut>(TSut sut)
        where TSut : ITransformAsync<int, int>, IReportsItemErrors
        => (sut, sut);



    [Theory]
    [MemberData(nameof(Shapes))]
    public async Task CurrentErrorItemCount_when_the_transformer_is_run_twice_reports_only_the_latest_run(string name)
    {
        var (sut, reporter) = Build(name);

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));
        Assert.Equal(1, reporter.CurrentErrorItemCount);

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));
        Assert.Equal(1, reporter.CurrentErrorItemCount);
    }



    [Fact]
    public async Task CurrentErrorItemCount_when_a_run_has_no_failures_resets_to_zero()
    {
        var sut = new SelectTransformer<int, int>(FailOnTwo, SkipAll);

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));
        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 3 })));

        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task CurrentErrorItemCount_when_two_enumerations_overlap_counts_both_and_keeps_the_count_until_the_next_run()
    {
        var sut = new SelectTransformer<int, int>(FailOnTwo, SkipAll);
        var first = sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })).GetAsyncEnumerator();
        var second = sut.TransformAsync(ToAsync(new[] { 2, 4 })).GetAsyncEnumerator();

        Assert.True(await first.MoveNextAsync());
        Assert.True(await second.MoveNextAsync());
        while (await first.MoveNextAsync())
        {
        }

        while (await second.MoveNextAsync())
        {
        }

        await first.DisposeAsync();
        await second.DisposeAsync();

        Assert.Equal(2, sut.CurrentErrorItemCount);

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })));
        Assert.Equal(0, sut.CurrentErrorItemCount);
    }
}
