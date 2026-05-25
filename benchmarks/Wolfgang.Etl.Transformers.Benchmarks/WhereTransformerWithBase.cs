using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers.Benchmarks;

/// <summary>
/// Alternate implementation of <see cref="WhereTransformer{T}"/> that inherits from
/// <see cref="TransformerBase{TSource, TDestination, TProgress}"/> to measure the overhead
/// of TransformerBase (Interlocked counters, progress timer plumbing, property fields,
/// per-item ThrowIfCancellationRequested, WithCancellation wrapping) compared to the
/// lightweight implementation shipped in the library.
/// </summary>
/// <remarks>
/// This type is deliberately internal to the benchmarks project and does not leak into
/// the public API of Wolfgang.Etl.Transformers. Any design mirrors the public
/// <see cref="WhereTransformer{T}"/> where applicable (two ctors, propagation semantics,
/// null checks).
/// </remarks>
public sealed class WhereTransformerWithBase<T> : TransformerBase<T, T, Report>
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;



    public WhereTransformerWithBase(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _syncPredicate = predicate;
    }



    public WhereTransformerWithBase(Func<T, ValueTask<bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _asyncPredicate = predicate;
    }



    protected override async IAsyncEnumerable<T> TransformWorkerAsync
    (
        IAsyncEnumerable<T> items,
        [EnumeratorCancellation] CancellationToken token
    )
    {
        var skipRemaining = SkipItemCount;
        var maxRemaining = MaximumItemCount;

        await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
        {
            token.ThrowIfCancellationRequested();

            bool passed;
            if (_syncPredicate is not null)
            {
                passed = _syncPredicate(item);
            }
            else
            {
                passed = await _asyncPredicate!(item).ConfigureAwait(continueOnCapturedContext: false);
            }

            if (!passed)
            {
                IncrementCurrentSkippedItemCount();
                continue;
            }

            if (skipRemaining > 0)
            {
                skipRemaining--;
                IncrementCurrentSkippedItemCount();
                continue;
            }

            if (maxRemaining <= 0)
            {
                yield break;
            }

            maxRemaining--;
            IncrementCurrentItemCount();
            yield return item;
        }
    }



    protected override Report CreateProgressReport()
    {
        return new Report(CurrentItemCount);
    }
}
