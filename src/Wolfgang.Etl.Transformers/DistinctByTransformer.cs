using System;
using System.Collections.Generic;
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
/// cancellation, no Skip/Max - to keep the hot loop minimal.
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
public sealed class DistinctByTransformer<TSource, TKey> : ITransformAsync<TSource, TSource>
    where TSource : notnull
    where TKey : notnull
{
    private readonly Func<TSource, TKey> _keySelector;
    private readonly IEqualityComparer<TKey>? _comparer;



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
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(keySelector);
#else
        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }
#endif

        _keySelector = keySelector;
        _comparer = comparer;
    }



    /// <summary>
    /// Asynchronously yields the first item in <paramref name="items"/> for each unique key
    /// produced by the configured selector, dropping later items with already-seen keys.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence containing the first item per distinct key.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<TSource> TransformAsync(IAsyncEnumerable<TSource> items)
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

        return DistinctByAsync(items, _keySelector, _comparer);
    }



    private static async IAsyncEnumerable<TSource> DistinctByAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer
    )
    {
        var seen = new HashSet<TKey>(comparer);
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (seen.Add(keySelector(item)))
            {
                yield return item;
            }
        }
    }
}
