using System;
using System.Collections.Generic;
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
/// It carries no progress reporting, no cancellation token, and no item counters - keeping the
/// hot loop as small as possible for use as a building block in composed pipelines. Callers needing
/// cancellation or windowing should compose with dedicated transformers (for example a future
/// <c>SkipTransformer</c> / <c>TakeTransformer</c> / <c>BufferedTransformer</c>).
/// </para>
/// <para>
/// Exceptions thrown by the predicate propagate to the caller. Callers that need to handle errors
/// per item should do so inside the predicate itself.
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
public sealed class WhereTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;



    /// <summary>
    /// Initializes a new instance with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> for items to be yielded.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public WhereTransformer(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        _syncPredicate = predicate;
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
    {
        ArgumentNullException.ThrowIfNull(predicate);

        _asyncPredicate = predicate;
    }



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



    private static async IAsyncEnumerable<T> FilterWithSyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, bool> predicate
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }



    private static async IAsyncEnumerable<T> FilterWithAsyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, ValueTask<bool>> predicate
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (await predicate(item).ConfigureAwait(continueOnCapturedContext: false))
            {
                yield return item;
            }
        }
    }
}
