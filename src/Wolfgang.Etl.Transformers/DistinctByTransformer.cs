using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields each input item with a unique key, where the key is produced by a
/// caller-supplied selector. Subsequent items whose key has already been seen are dropped.
/// </summary>
/// <typeparam name="TSource">The type of items in the input sequence. Must be non-null.</typeparam>
/// <typeparam name="TKey">The type of key used for de-duplication. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="DistinctByTransformer{TSource, TKey}"/> is the transformer equivalent of LINQ's
/// <c>Enumerable.DistinctBy</c> (introduced in .NET 6). Keys are compared using either the
/// supplied <see cref="IEqualityComparer{T}"/> or <see cref="EqualityComparer{T}.Default"/>
/// if none is provided. Order is preserved: the item bearing the first occurrence of each
/// key is yielded.
/// </para>
/// <para>
/// The key selector is synchronous - key extraction is normally a cheap pure function (a
/// property access, an integer ID, etc.) and an async overload would only add per-item
/// overhead. If the key requires I/O to compute, project the key in a preceding
/// <c>SelectTransformer</c> stage and use plain <see cref="DistinctTransformer{T}"/> on the
/// result.
/// </para>
/// <para>
/// <b>Memory footprint:</b> a fresh <see cref="HashSet{T}"/> of keys is allocated per call to
/// <see cref="TransformAsync"/> and grows in proportion to the number of unique keys in the
/// stream.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal. The one counter is the opt-in
/// <see cref="CurrentErrorItemCount"/>, which stays at zero on the default path.
/// </para>
/// <para>
/// By default an exception thrown by the key selector, or by the comparer while the key is being
/// added, propagates to the caller. Pass a <see cref="DelegateTransformerOptions"/> whose
/// <see cref="DelegateTransformerOptions.ErrorPolicy"/> returns <see cref="ItemErrorAction.Skip"/>
/// to drop the failing item and continue instead; those items are counted by
/// <see cref="CurrentErrorItemCount"/>. An item dropped because its key was already seen is the
/// transformer doing its job, not an error, and is never counted.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // first occurrence of each customer id
///     var uniqueCustomers = new DistinctByTransformer&lt;Order, int&gt;(o =&gt; o.CustomerId);
///
///     // case-insensitive distinct by name
///     var uniqueNames = new DistinctByTransformer&lt;Person, string&gt;
///     (
///         p =&gt; p.Name,
///         StringComparer.OrdinalIgnoreCase
///     );
/// </code>
/// </example>
public sealed class DistinctByTransformer<TSource, TKey> : ITransformAsync<TSource, TSource>, IReportsItemErrors
    where TSource : notnull
    where TKey : notnull
{
    private readonly Func<TSource, TKey> _keySelector;
    private readonly IEqualityComparer<TKey>? _comparer;
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance with the given key selector, using
    /// <see cref="EqualityComparer{T}.Default"/> for key comparison.
    /// </summary>
    /// <param name="keySelector">A function that extracts the comparison key from an input item.</param>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public DistinctByTransformer(Func<TSource, TKey> keySelector)
        : this(keySelector, comparer: null)
    {
    }



    /// <summary>
    /// Initializes a new instance with the given key selector and equality comparer.
    /// </summary>
    /// <param name="keySelector">A function that extracts the comparison key from an input item.</param>
    /// <param name="comparer">
    /// The comparer used to determine key equality. If <see langword="null"/>,
    /// <see cref="EqualityComparer{T}.Default"/> is used.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public DistinctByTransformer(Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : this(keySelector, comparer, new DelegateTransformerOptions())
    {
    }



    /// <summary>
    /// Initializes a new instance with the given key selector, equality comparer and error policy.
    /// </summary>
    /// <param name="keySelector">A function that extracts the comparison key from an input item.</param>
    /// <param name="comparer">
    /// The comparer used to determine key equality. If <see langword="null"/>,
    /// <see cref="EqualityComparer{T}.Default"/> is used.
    /// </param>
    /// <param name="options">
    /// Controls what happens when <paramref name="keySelector"/> or <paramref name="comparer"/>
    /// throws for one item.
    /// </param>
    /// <remarks>
    /// There is deliberately no two-argument <c>(keySelector, options)</c> overload. It would be
    /// ambiguous with <c>(keySelector, comparer)</c> at any existing call site passing a literal
    /// <see langword="null"/> comparer, which would be a source break. Pass
    /// <c>comparer: null</c> explicitly to use the default comparer with a policy.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="keySelector"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public DistinctByTransformer
    (
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        DelegateTransformerOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(options);

        _keySelector = keySelector;
        _comparer = comparer;
        _options = options;
    }



    /// <summary>
    /// The number of items dropped so far because the key selector or comparer threw and the error
    /// policy returned <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Cumulative across every enumeration produced by this instance, and counts only failures - an
    /// item dropped as a duplicate is not an error. An item whose failure aborted the run is not
    /// counted either, because the exception is re-thrown instead.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



    /// <summary>
    /// Asynchronously yields the first item in <paramref name="items"/> for each unique key
    /// produced by the configured selector, dropping later items with already-seen keys.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence containing the first item per distinct key.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<TSource> TransformAsync(IAsyncEnumerable<TSource> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return DistinctByAsync(items, _keySelector, _comparer);
    }



    private async IAsyncEnumerable<TSource> DistinctByAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer
    )
    {
        var seen = new HashSet<TKey>(comparer);
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            // Both the key selector and the comparer are caller-supplied, and either can throw for
            // a single item, so the whole key-and-add step is guarded. The try/catch cannot wrap
            // the yield: a C# async iterator cannot resume after it throws.
            bool isFirstOccurrence;
            try
            {
                isFirstOccurrence = seen.Add(keySelector(item));
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            if (isFirstOccurrence)
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
