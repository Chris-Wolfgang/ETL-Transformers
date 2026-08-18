using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Transformers;
using Xunit;

namespace Wolfgang.Etl.Transformers.Tests.Allocation;

/// <summary>
/// Verifies the library's documented zero-allocation hot paths stay allocation-free.
/// See <c>docs/allocation-budget.md</c> for the full list and the rationale for what
/// is (and deliberately is not) covered here.
/// </summary>
/// <remarks>
/// <para>
/// Measurement uses <see cref="GC.GetAllocatedBytesForCurrentThread"/>, a per-thread
/// cumulative counter. Each measured section is fully synchronous (no <c>await</c>) so
/// the before/after reads happen on the same thread and the delta is exact — an
/// <c>await</c> that resumes on a different thread would read a different thread's counter.
/// A warm-up loop first forces JIT compilation so tiered-compilation allocations do not
/// leak into the measured window.
/// </para>
/// </remarks>
public sealed class AllocationBudgetTests
{
    private const int WarmupIterations = 200;

    private const int MeasuredIterations = 1000;


    // A synchronous-completing async sequence. Constructed once, outside every measured
    // window, so building the iterator is never counted against the hot path under test.
    private static async IAsyncEnumerable<int> RangeAsync(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return i;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }


    // Sanity: none of the allocation-budget tests above ENUMERATE `source`, they just measure
    // the wrap. That means the RangeAsync iterator's state machine is never advanced by those
    // tests, so this companion happy-path test both proves the source works and exercises the
    // iterator body for coverage.
    [Fact]
    public async Task RangeAsync_helper_yields_expected_sequence()
    {
        var items = new List<int>();
        await foreach (var i in RangeAsync(4))
        {
            items.Add(i);
        }

        Assert.Equal(new[] { 0, 1, 2, 3 }, items);
    }


    /// <summary>
    /// The single-argument <see cref="PassThroughTransformer{T}.TransformAsync(IAsyncEnumerable{T})"/>
    /// overload is documented to return the source sequence directly, without allocating a
    /// wrapping iterator. This is the one intentional zero-allocation hot path in the library.
    /// </summary>
    [Fact]
    public void PassThroughTransformer_single_arg_TransformAsync_allocates_zero_bytes()
    {
        var transformer = new PassThroughTransformer<int>();
        var source = RangeAsync(4);

        // ReSharper disable PossibleMultipleEnumeration — TransformAsync is a non-enumerating wrapper (`return items;`); the result is discarded. Re-using `source` across iterations is what the zero-alloc contract requires.

        // Warm up: JIT the call, resolve the interface dispatch, tier up.
        for (var i = 0; i < WarmupIterations; i++)
        {
            _ = transformer.TransformAsync(source);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < MeasuredIterations; i++)
        {
            _ = transformer.TransformAsync(source);
        }

        // ReSharper restore PossibleMultipleEnumeration

        var delta = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal
        (
            0,
            delta
        );
    }


    /// <summary>
    /// The identity return is what makes the single-argument overload allocation-free:
    /// it hands back the very same sequence instance it was given.
    /// </summary>
    [Fact]
    public void PassThroughTransformer_single_arg_TransformAsync_returns_same_instance()
    {
        var transformer = new PassThroughTransformer<int>();
        var source = RangeAsync(4);

        var result = transformer.TransformAsync(source);

        Assert.Same
        (
            source,
            result
        );
    }


    /// <summary>
    /// Contrast case: the cancellation-aware overload wraps the source in a streaming
    /// iterator, so it necessarily allocates. This locks in the documented distinction
    /// between the two overloads — a regression that made the single-arg overload wrap
    /// would be caught by the zero-alloc test above; this guards the reverse assumption.
    /// </summary>
    [Fact]
    public void PassThroughTransformer_cancellation_overload_allocates()
    {
        var transformer = new PassThroughTransformer<int>();
        var source = RangeAsync(4);

        // ReSharper disable PossibleMultipleEnumeration — TransformAsync returns a wrapping iterator (never enumerated here); the result is discarded. We're measuring allocations of the *wrap*, not of consuming `source`.
        for (var i = 0; i < WarmupIterations; i++)
        {
            _ = transformer.TransformAsync(source, CancellationToken.None);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < MeasuredIterations; i++)
        {
            _ = transformer.TransformAsync(source, CancellationToken.None);
        }
        // ReSharper restore PossibleMultipleEnumeration

        var delta = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True
        (
            delta > 0,
            $"Expected the streaming overload to allocate a wrapping iterator, but it allocated {delta} bytes."
        );
    }
}
