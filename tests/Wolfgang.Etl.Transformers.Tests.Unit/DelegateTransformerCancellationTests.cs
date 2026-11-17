using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// An <see cref="OperationCanceledException"/> thrown by a caller delegate is cancellation, not an
/// item failure: it must never reach the <see cref="DelegateTransformerOptions.ErrorPolicy"/>,
/// even a skip-everything one, and must propagate to the caller.
/// </summary>
public class DelegateTransformerCancellationTests
{
    private sealed class PolicyProbe
    {
        public int Calls { get; private set; }

        public DelegateTransformerOptions Options => new()
        {
            ErrorPolicy = _ =>
            {
                Calls++;
                return ItemErrorAction.Skip;
            }
        };
    }



    private sealed class ThrowingComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;

        public int GetHashCode(int obj) => throw new OperationCanceledException();
    }



    private static async IAsyncEnumerable<int> CancelledInnerAsync()
    {
        await Task.Yield();
        throw new OperationCanceledException();
#pragma warning disable CS0162 // Unreachable code: makes this an iterator
        yield break;
#pragma warning restore CS0162
    }



    private static IEnumerable<int> CancelledInner()
    {
        throw new OperationCanceledException();
#pragma warning disable CS0162 // Unreachable code: makes this an iterator
        yield break;
#pragma warning restore CS0162
    }



    public static TheoryData<string> Cases() => new()
    {
        "Select-sync", "Select-async",
        "Where-sync", "Where-async",
        "SelectMany-sync-outer", "SelectMany-sync-inner", "SelectMany-async-outer", "SelectMany-async-inner",
        "SkipWhile-sync", "SkipWhile-async",
        "TakeWhile-sync", "TakeWhile-async",
        "Distinct", "DistinctBy",
    };



    private static IAsyncEnumerable<int> Build(string name, DelegateTransformerOptions o)
    {
        var src = ToAsync(new[] { 1, 2, 3 });
        return name switch
        {
            "Select-sync" => new SelectTransformer<int, int>((Func<int, int>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "Select-async" => new SelectTransformer<int, int>((Func<int, ValueTask<int>>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "Where-sync" => new WhereTransformer<int>((Func<int, bool>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "Where-async" => new WhereTransformer<int>((Func<int, ValueTask<bool>>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "SelectMany-sync-outer" => new SelectManyTransformer<int, int>((Func<int, IEnumerable<int>>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "SelectMany-sync-inner" => new SelectManyTransformer<int, int>((Func<int, IEnumerable<int>>)(_ => CancelledInner()), o).TransformAsync(src),
            "SelectMany-async-outer" => new SelectManyTransformer<int, int>((Func<int, IAsyncEnumerable<int>>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "SelectMany-async-inner" => new SelectManyTransformer<int, int>((Func<int, IAsyncEnumerable<int>>)(_ => CancelledInnerAsync()), o).TransformAsync(src),
            "SkipWhile-sync" => new SkipWhileTransformer<int>((Func<int, bool>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "SkipWhile-async" => new SkipWhileTransformer<int>((Func<int, ValueTask<bool>>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "TakeWhile-sync" => new TakeWhileTransformer<int>((Func<int, bool>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "TakeWhile-async" => new TakeWhileTransformer<int>((Func<int, ValueTask<bool>>)(_ => throw new OperationCanceledException()), o).TransformAsync(src),
            "Distinct" => new DistinctTransformer<int>(new ThrowingComparer(), o).TransformAsync(src),
            "DistinctBy" => new DistinctByTransformer<int, int>(_ => throw new OperationCanceledException(), comparer: null, o).TransformAsync(src),
            _ => throw new ArgumentOutOfRangeException(nameof(name)),
        };
    }



    [Theory]
    [MemberData(nameof(Cases))]
    public async Task TransformAsync_when_delegate_throws_OperationCanceledException_does_not_consult_policy_and_propagates(string name)
    {
        var probe = new PolicyProbe();

        _ = await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () => await CollectAsync(Build(name, probe.Options))
        );

        Assert.Equal(0, probe.Calls);
    }
}
