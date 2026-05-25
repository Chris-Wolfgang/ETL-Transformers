using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that projects each input item to a sequence of zero or more output items
/// and yields the concatenation of all those sequences.
/// </summary>
/// <typeparam name="TSource">The type of items in the input sequence. Must be non-null.</typeparam>
/// <typeparam name="TDestination">The type of items produced by the selector. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="SelectManyTransformer{TSource, TDestination}"/> is the transformer equivalent of
/// LINQ's <see cref="System.Linq.Enumerable.SelectMany{TSource, TResult}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, System.Collections.Generic.IEnumerable{TResult}})"/>:
/// it flattens a one-to-many projection into a single output stream.
/// </para>
/// <para>
/// Two constructors are provided:
/// </para>
/// <list type="bullet">
///   <item><description>A synchronous selector returning <see cref="IEnumerable{T}"/> for in-memory expansions.</description></item>
///   <item><description>An asynchronous selector returning <see cref="IAsyncEnumerable{T}"/> for streamed expansions such as a paged database query or a chunked HTTP response.</description></item>
/// </list>
/// <para>
/// Output is emitted depth-first: all items produced by the selector for the first input item
/// are yielded before any items produced for the second input item, and so on.
/// </para>
/// <para>
/// This type implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress,
/// no cancellation, no Skip/Max - to keep the hot loop minimal. Compose with dedicated
/// transformers when those concerns are needed.
/// </para>
/// <para>
/// Exceptions thrown by the selector or by enumerating the returned sequence propagate to the
/// caller. If the selector returns <see langword="null"/> a <see cref="NullReferenceException"/>
/// will be raised on iteration, matching the behaviour of LINQ's
/// <see cref="System.Linq.Enumerable.SelectMany{TSource, TResult}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, System.Collections.Generic.IEnumerable{TResult}})"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // synchronous fan-out: each Order yields all of its OrderLines
///     var lines = new SelectManyTransformer&lt;Order, OrderLine&gt;(o =&gt; o.Lines);
///
///     // asynchronous fan-out: each customer id yields a paged stream of related orders
///     var orders = new SelectManyTransformer&lt;int, Order&gt;
///     (
///         id =&gt; orderService.StreamForCustomerAsync(id)
///     );
/// </code>
/// </example>
public sealed class SelectManyTransformer<TSource, TDestination> : ITransformAsync<TSource, TDestination>
    where TSource : notnull
    where TDestination : notnull
{
    private readonly Func<TSource, IEnumerable<TDestination>>? _syncSelector;
    private readonly Func<TSource, IAsyncEnumerable<TDestination>>? _asyncSelector;



    /// <summary>
    /// Initializes a new instance with a synchronous selector that returns an
    /// <see cref="IEnumerable{T}"/> for each input item.
    /// </summary>
    /// <param name="selector">A function that maps each input item to zero or more output items.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectManyTransformer(Func<TSource, IEnumerable<TDestination>> selector)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(selector);
#else
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }
#endif

        _syncSelector = selector;
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous selector that returns an
    /// <see cref="IAsyncEnumerable{T}"/> for each input item.
    /// </summary>
    /// <param name="selector">
    /// A function that maps each input item to an asynchronous sequence of zero or more output items.
    /// Useful for streamed inner expansions such as paged database queries.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectManyTransformer(Func<TSource, IAsyncEnumerable<TDestination>> selector)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(selector);
#else
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }
#endif

        _asyncSelector = selector;
    }



    /// <summary>
    /// Asynchronously yields the concatenation of the per-item sequences produced by the
    /// configured selector.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous flattened sequence of items produced by the selector.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<TDestination> TransformAsync(IAsyncEnumerable<TSource> items)
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

        return _syncSelector is not null
            ? FlattenWithSyncSelectorAsync(items, _syncSelector)
            : FlattenWithAsyncSelectorAsync(items, _asyncSelector!);
    }



    private static async IAsyncEnumerable<TDestination> FlattenWithSyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, IEnumerable<TDestination>> selector
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            foreach (var inner in selector(item))
            {
                yield return inner;
            }
        }
    }



    private static async IAsyncEnumerable<TDestination> FlattenWithAsyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, IAsyncEnumerable<TDestination>> selector
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            await foreach (var inner in selector(item).ConfigureAwait(continueOnCapturedContext: false))
            {
                yield return inner;
            }
        }
    }
}
