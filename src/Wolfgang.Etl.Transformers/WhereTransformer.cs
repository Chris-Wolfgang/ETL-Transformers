using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields each item from the input sequence for which a caller-supplied
/// predicate returns <see langword="true"/>.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="WhereTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.Where{TSource}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, bool})"/>:
/// it tests each input item against a predicate and yields only those that pass.
/// </para>
/// <para>
/// Two constructors are provided: one for synchronous predicates and one for asynchronous
/// predicates returning <see cref="ValueTask{TResult}"/>. The asynchronous form is useful for
/// I/O-bound filter conditions such as a database existence check.
/// </para>
/// <para>
/// This type deliberately implements only <see cref="ITransformAsync{TSource, TDestination}"/> and
/// does <b>not</b> inherit from <see cref="TransformerBase{TSource, TDestination, TProgress}"/>.
/// It carries no progress reporting and no cancellation token, and counts nothing on the default
/// path - the only counter is the opt-in <see cref="CurrentErrorItemCount"/>, which stays at zero
/// unless an error policy skips an item - keeping the hot loop as small as possible for use as a
/// building block in composed pipelines. Callers needing
/// cancellation or windowing should compose with dedicated transformers (for example a future
/// <c>SkipTransformer</c> / <c>TakeTransformer</c> / <c>BufferedTransformer</c>).
/// </para>
/// <para>
/// By default an exception thrown by the predicate propagates to the caller. Pass a
/// <see cref="DelegateTransformerOptions"/> whose <see cref="DelegateTransformerOptions.ErrorPolicy"/>
/// returns <see cref="ItemErrorAction.Skip"/> to drop the failing item and continue instead; those
/// items are counted by <see cref="CurrentErrorItemCount"/>. An item the predicate simply returns
/// <see langword="false"/> for is ordinary filtering, not an error, and is never counted.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // synchronous filter
///     var activeOnly = new WhereTransformer&lt;Customer&gt;(c =&gt; c.IsActive);
///
///     // asynchronous filter (I/O-bound)
///     var existsInDb = new WhereTransformer&lt;int&gt;
///     (
///         async id =&gt; await db.CustomerExistsAsync(id).ConfigureAwait(false)
///     );
/// </code>
/// </example>
public sealed class WhereTransformer<T> : ITransformAsync<T, T>, IReportsItemErrors
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> for items to be yielded.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public WhereTransformer(Func<T, bool> predicate)
        : this(predicate, DelegateTransformerOptions.Default)
    {
    }



    /// <summary>
    /// Initializes a new instance with a synchronous predicate and an error policy.
    /// </summary>
    /// <param name="predicate">A function deciding whether an item is kept.</param>
    /// <param name="options">Controls what happens when <paramref name="predicate"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public WhereTransformer(Func<T, bool> predicate, DelegateTransformerOptions options)
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
    /// A function that asynchronously returns <see langword="true"/> for items to be yielded.
    /// Useful for I/O-bound filter conditions.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public WhereTransformer(Func<T, ValueTask<bool>> predicate)
        : this(predicate, DelegateTransformerOptions.Default)
    {
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous predicate and an error policy.
    /// </summary>
    /// <param name="predicate">A function asynchronously deciding whether an item is kept.</param>
    /// <param name="options">Controls what happens when <paramref name="predicate"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public WhereTransformer(Func<T, ValueTask<bool>> predicate, DelegateTransformerOptions options)
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
    /// Cumulative across every enumeration produced by this instance, and counts only failures - an
    /// item the predicate deliberately filtered out is not an error. An item whose failure aborted
    /// the run is not counted either, because the exception is re-thrown instead.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> for which the configured
    /// predicate returns <see langword="true"/>.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence containing only the items that satisfy the predicate.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return _syncPredicate is not null
            ? FilterWithSyncPredicateAsync(items, _syncPredicate)
            : FilterWithAsyncPredicateAsync(items, _asyncPredicate!);
    }



    private async IAsyncEnumerable<T> FilterWithSyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, bool> predicate
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            // The try/catch cannot wrap the yield: a C# async iterator cannot resume after it
            // throws, so the predicate runs first and only its verdict reaches the yield.
            bool keep;
            try
            {
                keep = predicate(item);
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            if (keep)
            {
                yield return item;
            }
        }
    }



    private async IAsyncEnumerable<T> FilterWithAsyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, ValueTask<bool>> predicate
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            bool keep;
            try
            {
                keep = await predicate(item).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            if (keep)
            {
                yield return item;
            }
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
