using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields items from the start of the input sequence as long as a
/// caller-supplied predicate returns <see langword="true"/>; the first item that fails
/// the predicate stops enumeration immediately and is not yielded.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="TakeWhileTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.TakeWhile{TSource}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, bool})"/>:
/// once the predicate returns <see langword="false"/> for an item, the transformer stops -
/// the failing item is not yielded and the source is not enumerated beyond it.
/// </para>
/// <para>
/// Two constructors are provided: one for synchronous predicates and one for asynchronous
/// predicates returning <see cref="ValueTask{TResult}"/>. The asynchronous form is useful for
/// I/O-bound stop conditions such as a state-check against an external system.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal.
/// </para>
/// <para>
/// Exceptions thrown by the predicate propagate to the caller.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // take rows until the first one that's incomplete
///     var head = new TakeWhileTransformer&lt;Row&gt;(r =&gt; r.IsComplete);
/// </code>
/// </example>
public sealed class TakeWhileTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;



    /// <summary>
    /// Initializes a new instance with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> while items should continue to be yielded.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public TakeWhileTransformer(Func<T, bool> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        _syncPredicate = predicate;
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous predicate.
    /// </summary>
    /// <param name="predicate">
    /// A function that asynchronously returns <see langword="true"/> while items should continue to be yielded.
    /// Useful for I/O-bound stop conditions.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public TakeWhileTransformer(Func<T, ValueTask<bool>> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        _asyncPredicate = predicate;
    }



    /// <summary>
    /// Asynchronously yields items from the start of <paramref name="items"/> while the
    /// configured predicate returns <see langword="true"/>.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing the leading run of items for which the predicate
    /// returns <see langword="true"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
#pragma warning disable RCS1140 // Roslynator does not associate throw inside #else block with method XML doc
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#pragma warning restore RCS1140
#endif

        return _syncPredicate is not null
            ? TakeWhileWithSyncPredicateAsync(items, _syncPredicate)
            : TakeWhileWithAsyncPredicateAsync(items, _asyncPredicate!);
    }



    private static async IAsyncEnumerable<T> TakeWhileWithSyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, bool> predicate
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (!predicate(item))
            {
                yield break;
            }

            yield return item;
        }
    }



    private static async IAsyncEnumerable<T> TakeWhileWithAsyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, ValueTask<bool>> predicate
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (!await predicate(item).ConfigureAwait(continueOnCapturedContext: false))
            {
                yield break;
            }

            yield return item;
        }
    }
}
