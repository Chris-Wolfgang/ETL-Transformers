using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that skips a fixed number of items from the start of the input sequence and
/// yields the rest unchanged.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="SkipTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.Skip{TSource}(System.Collections.Generic.IEnumerable{TSource}, int)"/>:
/// it discards the first <c>Count</c> items and yields everything after them.
/// </para>
/// <para>
/// A <c>count</c> of <c>0</c> or negative is not an error - the transformer yields the source
/// unchanged (matches the BCL contract for <c>Enumerable.Skip</c>). When <c>count &lt;= 0</c>
/// the source is returned directly without a wrapping iterator, so this is essentially free.
/// </para>
/// <para>
/// Useful for skipping headers, hopping past already-processed rows, or as a building block in
/// composed pipelines when the upstream extractor does not expose a row-skip property.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // skip the first row (header) before processing the rest
///     var dataOnly = new SkipTransformer&lt;Row&gt;(count: 1);
/// </code>
/// </example>
public sealed class SkipTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly int _count;



    /// <summary>
    /// Initializes a new instance that skips the first <paramref name="count"/> items.
    /// </summary>
    /// <param name="count">
    /// The number of items to skip from the start of the source. A value of <c>0</c> or less
    /// yields the source unchanged.
    /// </param>
    public SkipTransformer(int count)
    {
        _count = count;
    }



    /// <summary>
    /// Asynchronously yields all items from <paramref name="items"/> after skipping the first
    /// <see cref="Count"/> of them.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing the items from <paramref name="items"/> after
    /// position <see cref="Count"/>. Empty if the source has <see cref="Count"/> or fewer
    /// items. The source <paramref name="items"/> directly when <see cref="Count"/> is
    /// <c>0</c> or negative.
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

        return _count <= 0
            ? items
            : SkipAsync(items, _count);
    }



    /// <summary>
    /// The number of items this transformer will skip from the start of the source.
    /// </summary>
    public int Count => _count;



    private static async IAsyncEnumerable<T> SkipAsync(IAsyncEnumerable<T> items, int count)
    {
        var skipped = 0;
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (skipped < count)
            {
                skipped++;
                continue;
            }

            yield return item;
        }
    }
}
