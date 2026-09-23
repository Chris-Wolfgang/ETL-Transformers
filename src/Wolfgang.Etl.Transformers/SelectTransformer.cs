using System;
using System.Collections.Generic;
using System.Threading;
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
/// By default an exception thrown by the selector propagates to the caller. Pass a
/// <see cref="DelegateTransformerOptions"/> whose <see cref="DelegateTransformerOptions.ErrorPolicy"/>
/// returns <see cref="ItemErrorAction.Skip"/> to drop the failing item and continue instead; skipped
/// items are counted by <see cref="CurrentErrorItemCount"/>. That count is cumulative across every
/// enumeration of this instance, while the <see cref="ItemErrorContext.ItemNumber"/> handed to the
/// policy is the one-based position within the enumeration that failed.
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
public sealed class SelectTransformer<TSource, TDestination> : ITransformAsync<TSource, TDestination>, IReportsItemErrors
    where TSource : notnull
    where TDestination : notnull
{
    private readonly Func<TSource, TDestination>? _syncSelector;
    private readonly Func<TSource, ValueTask<TDestination>>? _asyncSelector;
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance with a synchronous selector function.
    /// </summary>
    /// <param name="selector">A function that projects each input item to an output item.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectTransformer(Func<TSource, TDestination> selector)
        : this(selector, new DelegateTransformerOptions())
    {
    }



    /// <summary>
    /// Initializes a new instance with a synchronous selector function and an error policy.
    /// </summary>
    /// <param name="selector">A function that projects each input item to an output item.</param>
    /// <param name="options">Controls what happens when <paramref name="selector"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="selector"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public SelectTransformer(Func<TSource, TDestination> selector, DelegateTransformerOptions options)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(options);

        _syncSelector = selector;
        _options = options;
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
        : this(selector, new DelegateTransformerOptions())
    {
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous selector function and an error policy.
    /// </summary>
    /// <param name="selector">
    /// A function that asynchronously projects each input item to an output item.
    /// </param>
    /// <param name="options">Controls what happens when <paramref name="selector"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="selector"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public SelectTransformer
    (
        Func<TSource, ValueTask<TDestination>> selector,
        DelegateTransformerOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(options);

        _asyncSelector = selector;
        _options = options;
    }



    /// <summary>
    /// The number of items dropped so far because the selector threw and the error policy returned
    /// <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Cumulative across every enumeration produced by this instance. An item whose failure aborted
    /// the run is not counted, because the exception is re-thrown instead.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



    /// <summary>
    /// Asynchronously projects each item from <paramref name="items"/> through the configured
    /// selector and yields the result.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence of projected items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<TDestination> TransformAsync(IAsyncEnumerable<TSource> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return _syncSelector is not null
            ? ProjectWithSyncSelectorAsync(items, _syncSelector)
            : ProjectWithAsyncSelectorAsync(items, _asyncSelector!);
    }



    private async IAsyncEnumerable<TDestination> ProjectWithSyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, TDestination> selector
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            // The try/catch cannot wrap the yield: a C# async iterator cannot resume after it
            // throws, so the projection completes first and only its result is yielded.
            TDestination projected;
            try
            {
                projected = selector(item);
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            yield return projected;
        }
    }



    private async IAsyncEnumerable<TDestination> ProjectWithAsyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, ValueTask<TDestination>> selector
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            TDestination projected;
            try
            {
                projected = await selector(item).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            yield return projected;
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
