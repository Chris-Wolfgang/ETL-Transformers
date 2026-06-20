using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that projects each item from the input sequence through a caller-supplied
/// selector function, returning the result.
/// </summary>
/// <typeparam name="TSource">The type of items in the input sequence. Must be non-null.</typeparam>
/// <typeparam name="TDestination">The type of items produced by the selector. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="SelectTransformer{TSource, TDestination}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.Select{TSource, TResult}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, TResult})"/>:
/// it applies a function to each input item and yields the result.
/// </para>
/// <para>
/// Two constructors are provided: one for synchronous selectors and one for asynchronous
/// selectors returning <see cref="ValueTask{TResult}"/>. The asynchronous form is useful for
/// I/O-bound projections (database lookups, HTTP calls, etc.).
/// </para>
/// <para>
/// This type deliberately implements only <see cref="ITransformAsync{TSource, TDestination}"/> and
/// does <b>not</b> inherit from <see cref="TransformerBase{TSource, TDestination, TProgress}"/>.
/// It carries no progress reporting, no cancellation token, and no item counters - keeping the
/// hot loop as small as possible for use as a building block in composed pipelines. See the
/// benchmarks project for measurements motivating this choice.
/// </para>
/// <para>
/// Exceptions thrown by the selector propagate to the caller. Callers that need to handle
/// errors per item should do so inside the selector itself.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // synchronous projection
///     var toUpper = new SelectTransformer&lt;string, string&gt;(s =&gt; s.ToUpperInvariant());
///
///     // asynchronous projection (I/O-bound)
///     var lookup = new SelectTransformer&lt;int, Customer&gt;
///     (
///         async id =&gt; await customerService.GetByIdAsync(id).ConfigureAwait(false)
///     );
/// </code>
/// </example>
public sealed class SelectTransformer<TSource, TDestination> : ITransformAsync<TSource, TDestination>
    where TSource : notnull
    where TDestination : notnull
{
    private readonly Func<TSource, TDestination>? _syncSelector;
    private readonly Func<TSource, ValueTask<TDestination>>? _asyncSelector;



    /// <summary>
    /// Initializes a new instance with a synchronous selector function.
    /// </summary>
    /// <param name="selector">A function that projects each input item to an output item.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectTransformer(Func<TSource, TDestination> selector)
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
    /// Initializes a new instance with an asynchronous selector function.
    /// </summary>
    /// <param name="selector">
    /// A function that asynchronously projects each input item to an output item.
    /// Useful for I/O-bound projections.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectTransformer(Func<TSource, ValueTask<TDestination>> selector)
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
    /// Asynchronously projects each item from <paramref name="items"/> through the configured
    /// selector and yields the result.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence of projected items.</returns>
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
            ? ProjectWithSyncSelectorAsync(items, _syncSelector)
            : ProjectWithAsyncSelectorAsync(items, _asyncSelector!);
    }



    private static async IAsyncEnumerable<TDestination> ProjectWithSyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, TDestination> selector
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            yield return selector(item);
        }
    }



    private static async IAsyncEnumerable<TDestination> ProjectWithAsyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, ValueTask<TDestination>> selector
    )
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            yield return await selector(item).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
