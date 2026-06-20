using System;
using System.Collections.Generic;
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
public sealed class DistinctTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly IEqualityComparer<T>? _comparer;



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
    {
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

        return DistinctAsync(items, _comparer);
    }



    private static async IAsyncEnumerable<T> DistinctAsync
    (
        IAsyncEnumerable<T> items,
        IEqualityComparer<T>? comparer
    )
    {
        var seen = new HashSet<T>(comparer);
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (seen.Add(item))
            {
                yield return item;
            }
        }
    }
}
