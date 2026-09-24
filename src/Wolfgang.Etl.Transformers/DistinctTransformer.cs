using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields each unique item from the input sequence, dropping subsequent
/// duplicates.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="DistinctTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.Distinct{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
/// Items are compared using either the supplied <see cref="IEqualityComparer{T}"/> or
/// <see cref="EqualityComparer{T}.Default"/> if none is provided. Order is preserved: the
/// first occurrence of each item is yielded.
/// </para>
/// <para>
/// <b>Memory footprint:</b> a fresh <see cref="HashSet{T}"/> is allocated per call to
/// <see cref="TransformAsync"/> and grows in proportion to the number of unique items in the
/// stream. For unbounded streams or streams with many unique items, prefer
/// <see cref="DistinctByTransformer{TSource, TKey}"/> with a small key type, or apply Distinct
/// only after a stage that bounds cardinality.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // default equality
///     var unique = new DistinctTransformer&lt;string&gt;();
///
///     // case-insensitive
///     var insensitive = new DistinctTransformer&lt;string&gt;(StringComparer.OrdinalIgnoreCase);
/// </code>
/// </example>
public sealed class DistinctTransformer<T> : ITransformAsync<T, T>, IReportsItemErrors
    where T : notnull
{
    private readonly IEqualityComparer<T>? _comparer;
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance using <see cref="EqualityComparer{T}.Default"/>.
    /// </summary>
    public DistinctTransformer() : this(comparer: null)
    {
    }



    /// <summary>
    /// Initializes a new instance using the supplied equality comparer.
    /// </summary>
    /// <param name="comparer">
    /// The comparer used to determine equality. If <see langword="null"/>,
    /// <see cref="EqualityComparer{T}.Default"/> is used.
    /// </param>
    public DistinctTransformer(IEqualityComparer<T>? comparer)
        : this(comparer, DelegateTransformerOptions.Default)
    {
    }



    /// <summary>
    /// Initializes a new instance with an equality comparer and an error policy.
    /// </summary>
    /// <param name="comparer">
    /// The comparer used to determine equality. If <see langword="null"/>,
    /// <see cref="EqualityComparer{T}.Default"/> is used.
    /// </param>
    /// <param name="options">
    /// Controls what happens when <paramref name="comparer"/> throws for one item.
    /// </param>
    /// <remarks>
    /// There is deliberately no single-argument overload taking only the options. It would be
    /// ambiguous with <c>(comparer)</c> at any call site passing a literal <see langword="null"/>,
    /// which would be a source break. Pass <c>comparer: null</c> explicitly to use the default
    /// comparer with a policy.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    public DistinctTransformer(IEqualityComparer<T>? comparer, DelegateTransformerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
        _comparer = comparer;
    }



    /// <summary>
    /// Asynchronously yields the first occurrence of each unique item in <paramref name="items"/>,
    /// dropping later duplicates.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence containing each item from <paramref name="items"/> at most once.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return DistinctAsync(items, _comparer);
    }



    /// <summary>
    /// The number of items dropped so far because the comparer threw and the error policy returned
    /// <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Cumulative across every enumeration produced by this instance. An item dropped because an
    /// equal one was already seen is the transformer doing its job, not an error, and is never
    /// counted.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



    private async IAsyncEnumerable<T> DistinctAsync
    (
        IAsyncEnumerable<T> items,
        IEqualityComparer<T>? comparer
    )
    {
        var seen = new HashSet<T>(comparer);
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            // The comparer is caller-supplied and its GetHashCode or Equals can throw for a single
            // item. The try/catch cannot wrap the yield: a C# async iterator cannot resume after it
            // throws.
            bool isFirstOccurrence;
            try
            {
                isFirstOccurrence = seen.Add(item);
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    // The item never entered the set, so an equal item later in the stream is
                    // still treated as a first occurrence.
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
