using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// An <see cref="OperationCanceledException"/> thrown by a caller delegate is cancellation, not an
/// item failure: it must never reach the <see cref="DelegateTransformerOptions.ErrorPolicy"/>,
/// even a skip-everything one (so nothing is counted), and must propagate to the caller.
/// </summary>
public class DelegateTransformerCancellationTests
{
    // Every collision forces an Equals call, so a comparer that throws from Equals is reached.
    private sealed class CollidingThrowingComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => throw new OperationCanceledException();

        public int GetHashCode(int obj) => 0;
    }



    private static readonly DelegateTransformerOptions SkipAll = new() { ErrorPolicy = _ => ItemErrorAction.Skip };



    private static int ThrowCancelled() => throw new OperationCanceledException();



    private static IAsyncEnumerable<int> CancelledInnerAsync() =>
        new SelectTransformer<int, int>(_ => ThrowCancelled()).TransformAsync(ToAsync(new[] { 1 }));



    public static TheoryData<string> Cases() => new()
    {
        "Select-sync", "Select-async",
        "Where-sync", "Where-async",
        "SelectMany-sync-outer", "SelectMany-sync-inner", "SelectMany-async-outer", "SelectMany-async-inner",
        "SkipWhile-sync", "SkipWhile-async",
        "TakeWhile-sync", "TakeWhile-async",
        "Distinct", "DistinctBy",
    };



    private static (IAsyncEnumerable<int> Items, Func<int> ErrorCount) Run<TSut>(TSut sut)
        where TSut : ITransformAsync<int, int>, IReportsItemErrors
        => (sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })), () => sut.CurrentErrorItemCount);



    private static (IAsyncEnumerable<int> Items, Func<int> ErrorCount) Build(string name) => name switch // the final arm is "DistinctBy"
    {
        "Select-sync" => Run(new SelectTransformer<int, int>(_ => ThrowCancelled(), SkipAll)),
        "Select-async" => Run(new SelectTransformer<int, int>((Func<int, ValueTask<int>>)(_ => throw new OperationCanceledException()), SkipAll)),
        "Where-sync" => Run(new WhereTransformer<int>((Func<int, bool>)(_ => throw new OperationCanceledException()), SkipAll)),
        "Where-async" => Run(new WhereTransformer<int>((Func<int, ValueTask<bool>>)(_ => throw new OperationCanceledException()), SkipAll)),
        "SelectMany-sync-outer" => Run(new SelectManyTransformer<int, int>((Func<int, IEnumerable<int>>)(_ => throw new OperationCanceledException()), SkipAll)),
        "SelectMany-sync-inner" => Run(new SelectManyTransformer<int, int>(i => new[] { i }.Select(_ => ThrowCancelled()), SkipAll)),
        "SelectMany-async-outer" => Run(new SelectManyTransformer<int, int>((Func<int, IAsyncEnumerable<int>>)(_ => throw new OperationCanceledException()), SkipAll)),
        "SelectMany-async-inner" => Run(new SelectManyTransformer<int, int>(_ => CancelledInnerAsync(), SkipAll)),
        "SkipWhile-sync" => Run(new SkipWhileTransformer<int>((Func<int, bool>)(_ => throw new OperationCanceledException()), SkipAll)),
        "SkipWhile-async" => Run(new SkipWhileTransformer<int>((Func<int, ValueTask<bool>>)(_ => throw new OperationCanceledException()), SkipAll)),
        "TakeWhile-sync" => Run(new TakeWhileTransformer<int>((Func<int, bool>)(_ => throw new OperationCanceledException()), SkipAll)),
        "TakeWhile-async" => Run(new TakeWhileTransformer<int>((Func<int, ValueTask<bool>>)(_ => throw new OperationCanceledException()), SkipAll)),
        "Distinct" => Run(new DistinctTransformer<int>(new CollidingThrowingComparer(), SkipAll)),
        _ => Run(new DistinctByTransformer<int, int>(_ => ThrowCancelled(), comparer: null, SkipAll)),
    };



    [Theory]
    [MemberData(nameof(Cases))]
    public async Task TransformAsync_when_delegate_throws_OperationCanceledException_does_not_consult_policy_and_propagates(string name)
    {
        var (items, errorCount) = Build(name);

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () => await CollectAsync(items)
        );

        Assert.Equal(0, errorCount());
    }
}
