using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields at most a fixed number of items from the start of the input
/// sequence.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="TakeTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.Take{TSource}(System.Collections.Generic.IEnumerable{TSource}, int)"/>:
/// it yields the first <c>Count</c> items from the source and stops, never enumerating beyond
/// what was requested.
/// </para>
/// <para>
/// A <c>count</c> of <c>0</c> or negative is not an error - the transformer yields no items and
/// does not enumerate the source at all (matches the BCL contract for <c>Enumerable.Take</c>).
/// </para>
/// <para>
/// Useful for windowing the head of a stream when the upstream extractor does not expose a
/// row-count limit, and as a building block in composed pipelines.
/// </para>
/// <para>
/// Implements <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/> - no progress,
/// no <c>SkipItemCount</c>/<c>MaximumItemCount</c> limits - to keep the hot loop minimal.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // sample the first 100 rows for a dev / debug loop
///     var head = new TakeTransformer&lt;Row&gt;(count: 100);
/// </code>
/// </example>
public sealed class TakeTransformer<T> : ITransformWithCancellationAsync<T, T>
    where T : notnull
{
    private readonly int _count;



    /// <summary>
    /// Initializes a new instance that yields at most <paramref name="count"/> items.
    /// </summary>
    /// <param name="count">
    /// The maximum number of items to yield. A value of <c>0</c> or less results in an empty
    /// output sequence and the source is not enumerated.
    /// </param>
    public TakeTransformer(int count)
    {
        _count = count;
    }



    /// <summary>
    /// Asynchronously yields at most <see cref="Count"/> items from the start of
    /// <paramref name="items"/>.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing the first <see cref="Count"/> items from
    /// <paramref name="items"/>, or all items if the source is shorter than <see cref="Count"/>.
    /// Empty if <see cref="Count"/> is <c>0</c> or negative.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return TakeAsync(items, _count);
    }



    /// <summary>
    /// Transforms <paramref name="items"/> like the overload without a token, and forwards
    /// <paramref name="token"/> to the stages above this one and to the source.
    /// </summary>
    /// <param name="items">The sequence to transform.</param>
    /// <param name="token">A token that cancels the enumeration, including the source.</param>
    /// <returns>The transformed sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(items);

        return TakeAsync(items, _count, token: token);
    }



    /// <summary>
    /// The maximum number of items this transformer will yield.
    /// </summary>
    public int Count => _count;



    private static async IAsyncEnumerable<T> TakeAsync
    (
        IAsyncEnumerable<T> items,
        int count,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        if (count <= 0)
        {
            // Nothing to take: return without ever touching the source.
            yield break;
        }

        var taken = 0;

        // Observe the token here, not only through WithCancellation: a source that ignores
        // the token would otherwise be drained after cancellation (issues #209, #813).
        token.ThrowIfCancellationRequested();

        await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
        {
            token.ThrowIfCancellationRequested();

            yield return item;
            taken++;
            if (taken >= count)
            {
                yield break;
            }
        }
    }
}
