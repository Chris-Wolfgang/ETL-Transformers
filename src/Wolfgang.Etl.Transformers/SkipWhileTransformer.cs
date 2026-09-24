using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that skips items from the start of the input sequence as long as a
/// caller-supplied predicate returns <see langword="true"/>; once the predicate returns
/// <see langword="false"/> for an item, that item and every item after it are yielded without
/// further predicate evaluation.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="SkipWhileTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.SkipWhile{TSource}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, bool})"/>.
/// The predicate is called on each item only until it first returns <see langword="false"/>;
/// after that, every remaining item flows through unchanged.
/// </para>
/// <para>
/// Two constructors are provided: one for synchronous predicates and one for asynchronous
/// predicates returning <see cref="ValueTask{TResult}"/>. The asynchronous form is useful for
/// I/O-bound start conditions such as a state-check against an external system.
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
///     // skip leading blank rows; once a non-blank row appears, yield everything from there
///     var fromFirstReal = new SkipWhileTransformer&lt;Row&gt;(r =&gt; r.IsBlank);
/// </code>
/// </example>
public sealed class SkipWhileTransformer<T> : ITransformAsync<T, T>, IReportsItemErrors
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> while items should be skipped.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SkipWhileTransformer(Func<T, bool> predicate)
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
    public SkipWhileTransformer(Func<T, bool> predicate, DelegateTransformerOptions options)
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
    /// A function that asynchronously returns <see langword="true"/> while items should be skipped.
    /// Useful for I/O-bound start conditions.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SkipWhileTransformer(Func<T, ValueTask<bool>> predicate)
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
    public SkipWhileTransformer(Func<T, ValueTask<bool>> predicate, DelegateTransformerOptions options)
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
    /// Cumulative across every enumeration produced by this instance. Only a thrown exception counts; an item skipped because the predicate returned <see langword="true"/> is ordinary behaviour.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



    /// <summary>
    /// Asynchronously skips items from the start of <paramref name="items"/> while the
    /// configured predicate returns <see langword="true"/>, then yields the rest unchanged.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing every item from the first one where the predicate
    /// returns <see langword="false"/> to the end of <paramref name="items"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return _syncPredicate is not null
            ? SkipWhileWithSyncPredicateAsync(items, _syncPredicate)
            : SkipWhileWithAsyncPredicateAsync(items, _asyncPredicate!);
    }



    private async IAsyncEnumerable<T> SkipWhileWithSyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, bool> predicate
    )
    {
        var skipping = true;
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            if (skipping)
            {
                bool stillSkipping;
                try
                {
                    stillSkipping = predicate(item);
                }
                catch (Exception exception)
                {
                    if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                    {
                        // Drop the item and stay in the skipping phase: a predicate that threw says
                        // nothing about whether the leading run has ended, so ending it here would
                        // let the rest of the prefix through on the strength of a failure.
                        continue;
                    }

                    throw;
                }

                if (stillSkipping)
                {
                    continue;
                }

                skipping = false;
            }

            yield return item;
        }
    }



    private async IAsyncEnumerable<T> SkipWhileWithAsyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, ValueTask<bool>> predicate
    )
    {
        var skipping = true;
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            if (skipping)
            {
                bool stillSkipping;
                try
                {
                    stillSkipping = await predicate(item).ConfigureAwait(continueOnCapturedContext: false);
                }
                catch (Exception exception)
                {
                    if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                    {
                        // Drop the item and stay in the skipping phase: a predicate that threw says
                        // nothing about whether the leading run has ended, so ending it here would
                        // let the rest of the prefix through on the strength of a failure.
                        continue;
                    }

                    throw;
                }

                if (stillSkipping)
                {
                    continue;
                }

                skipping = false;
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
