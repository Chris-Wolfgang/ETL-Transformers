using System;
using System.Collections.Generic;
using System.Threading;
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
public sealed class TakeWhileTransformer<T> : ITransformAsync<T, T>, IReportsItemErrors
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> while items should continue to be yielded.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public TakeWhileTransformer(Func<T, bool> predicate)
        : this(predicate, DelegateTransformerOptions.Default)
    {
    }



    /// <summary>
    /// Initializes a new instance with a synchronous predicate and an error policy.
    /// </summary>
    /// <param name="predicate">The predicate evaluated for each item.</param>
    /// <param name="options">Controls what happens when <paramref name="predicate"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public TakeWhileTransformer(Func<T, bool> predicate, DelegateTransformerOptions options)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(options);

        _syncPredicate = predicate;
        _options = options;
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
        : this(predicate, DelegateTransformerOptions.Default)
    {
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous predicate and an error policy.
    /// </summary>
    /// <param name="predicate">The predicate evaluated for each item.</param>
    /// <param name="options">Controls what happens when <paramref name="predicate"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public TakeWhileTransformer(Func<T, ValueTask<bool>> predicate, DelegateTransformerOptions options)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(options);

        _asyncPredicate = predicate;
        _options = options;
    }



    /// <summary>
    /// The number of items dropped so far because the predicate threw and the error policy returned
    /// <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Cumulative across every enumeration produced by this instance. Only a thrown exception
    /// counts; ending the sequence because the predicate returned <see langword="false"/> is
    /// ordinary behaviour and is not an error.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



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
        ArgumentNullException.ThrowIfNull(items);

        return _syncPredicate is not null
            ? TakeWhileWithSyncPredicateAsync(items, _syncPredicate)
            : TakeWhileWithAsyncPredicateAsync(items, _asyncPredicate!);
    }



    private async IAsyncEnumerable<T> TakeWhileWithSyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, bool> predicate
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            bool keepTaking;
            try
            {
                keepTaking = predicate(item);
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    // Drop the item and keep evaluating. Ending the sequence here would be a
                    // stronger action than Skip asks for: a predicate that threw gave no verdict,
                    // so it cannot stand in for the false that normally stops the run.
                    continue;
                }

                throw;
            }

            if (!keepTaking)
            {
                yield break;
            }

            yield return item;
        }
    }



    private async IAsyncEnumerable<T> TakeWhileWithAsyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, ValueTask<bool>> predicate
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            bool keepTaking;
            try
            {
                keepTaking = await predicate(item).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    // Drop the item and keep evaluating. Ending the sequence here would be a
                    // stronger action than Skip asks for: a predicate that threw gave no verdict,
                    // so it cannot stand in for the false that normally stops the run.
                    continue;
                }

                throw;
            }

            if (!keepTaking)
            {
                yield break;
            }

            yield return item;
        }
    }



    /// <summary>
    /// Applies the configured error policy and, when it asks to skip, records the dropped item.
    /// </summary>
    private ItemErrorAction HandleItemError(long itemNumber, Exception exception)
    {
        var action = _options.ErrorPolicy(new ItemErrorContext(itemNumber, exception));
        if (action == ItemErrorAction.Skip)
        {
            _ = Interlocked.Increment(ref _currentErrorItemCount);
        }

        return action;
    }
}
