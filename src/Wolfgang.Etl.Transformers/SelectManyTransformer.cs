using System;
using System.Collections.Generic;
using System.Threading;
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
/// By default, exceptions thrown by the selector or by enumerating the returned sequence propagate
/// to the caller. Pass a <see cref="DelegateTransformerOptions"/> whose
/// <see cref="DelegateTransformerOptions.ErrorPolicy"/> returns <see cref="ItemErrorAction.Skip"/>
/// to abandon the failing source item's expansion and continue with the next one instead; results
/// already yielded from that item are kept, the item is counted once in
/// <see cref="CurrentErrorItemCount"/>, and both the selector call and the enumeration of what it
/// returned are covered. If the selector returns <see langword="null"/> a <see cref="NullReferenceException"/>
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
public sealed class SelectManyTransformer<TSource, TDestination> : ITransformAsync<TSource, TDestination>, IReportsItemErrors
    where TSource : notnull
    where TDestination : notnull
{
    private readonly Func<TSource, IEnumerable<TDestination>>? _syncSelector;
    private readonly Func<TSource, IAsyncEnumerable<TDestination>>? _asyncSelector;
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance with a synchronous selector that returns an
    /// <see cref="IEnumerable{T}"/> for each input item.
    /// </summary>
    /// <param name="selector">A function that maps each input item to zero or more output items.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectManyTransformer(Func<TSource, IEnumerable<TDestination>> selector)
        : this(selector, new DelegateTransformerOptions())
    {
    }



    /// <summary>
    /// Initializes a new instance with a synchronous selector function and an error policy.
    /// </summary>
    /// <param name="selector">A function that expands each input item into a sequence.</param>
    /// <param name="options">
    /// Controls what happens when <paramref name="selector"/> throws, either when it is called or
    /// while the sequence it returned is being enumerated.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="selector"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public SelectManyTransformer
    (
        Func<TSource, IEnumerable<TDestination>> selector,
        DelegateTransformerOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(options);

        _syncSelector = selector;
        _options = options;
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
        : this(selector, new DelegateTransformerOptions())
    {
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous selector function and an error policy.
    /// </summary>
    /// <param name="selector">
    /// A function that expands each input item into an asynchronous sequence.
    /// </param>
    /// <param name="options">
    /// Controls what happens when <paramref name="selector"/> throws, either when it is called or
    /// while the sequence it returned is being enumerated.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="selector"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public SelectManyTransformer
    (
        Func<TSource, IAsyncEnumerable<TDestination>> selector,
        DelegateTransformerOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(options);

        _asyncSelector = selector;
        _options = options;
    }



    /// <summary>
    /// The number of source items abandoned so far because the selector threw and the error policy
    /// returned <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Counts source items, not expanded results, and is cumulative across every enumeration
    /// produced by this instance. A source item whose failure aborted the run is not counted,
    /// because the exception is re-thrown instead.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



    /// <summary>
    /// Asynchronously yields the concatenation of the per-item sequences produced by the
    /// configured selector.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous flattened sequence of items produced by the selector.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<TDestination> TransformAsync(IAsyncEnumerable<TSource> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return _syncSelector is not null
            ? FlattenWithSyncSelectorAsync(items, _syncSelector)
            : FlattenWithAsyncSelectorAsync(items, _asyncSelector!);
    }



    private async IAsyncEnumerable<TDestination> FlattenWithSyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, IEnumerable<TDestination>> selector
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            // The selector's sequence is usually lazy, so a throw can surface either here or on a
            // later MoveNext. The enumerator is therefore driven by hand: a try/catch cannot wrap
            // the yield, because a C# async iterator cannot resume after it throws.
            IEnumerator<TDestination> enumerator;
            try
            {
                enumerator = selector(item).GetEnumerator();
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            var failureHandled = false;
            try
            {
                while (true)
                {
                    TDestination current;
                    try
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }

                        current = enumerator.Current;
                    }
                    catch (Exception exception)
                    {
                        failureHandled = true;
                        if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                        {
                            break;
                        }

                        throw;
                    }

                    yield return current;
                }
            }
            finally
            {
                DisposeAfterItem(enumerator, failureHandled);
            }
        }
    }



    private async IAsyncEnumerable<TDestination> FlattenWithAsyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, IAsyncEnumerable<TDestination>> selector
    )
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            IAsyncEnumerator<TDestination> enumerator;
            try
            {
                enumerator = selector(item).GetAsyncEnumerator();
            }
            catch (Exception exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            var failureHandled = false;
            try
            {
                while (true)
                {
                    TDestination current;
                    try
                    {
                        if (!await enumerator.MoveNextAsync().ConfigureAwait(continueOnCapturedContext: false))
                        {
                            break;
                        }

                        current = enumerator.Current;
                    }
                    catch (Exception exception)
                    {
                        failureHandled = true;
                        if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                        {
                            break;
                        }

                        throw;
                    }

                    yield return current;
                }
            }
            finally
            {
                await DisposeAfterItemAsync(enumerator, failureHandled)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }



    /// <summary>
    /// Disposes the inner enumerator once a source item is finished with, deciding whether a
    /// failure from the cleanup itself may escape.
    /// </summary>
    /// <param name="enumerator">The inner enumerator to dispose.</param>
    /// <param name="failureHandled">
    /// Whether this source item already had a failure put through the error policy.
    /// </param>
    /// <remarks>
    /// When the item already failed, a cleanup error is swallowed. Letting it escape would end the
    /// run and defeat the <see cref="ItemErrorAction.Skip"/> just honoured, or displace the
    /// exception an <see cref="ItemErrorAction.Abort"/> is already propagating; and putting it
    /// through the policy again would count the same source item twice. On an item that did not
    /// fail, a disposal error is not an item-level data error and propagates unchanged.
    /// </remarks>
    private static void DisposeAfterItem(IEnumerator<TDestination> enumerator, bool failureHandled)
    {
        try
        {
            enumerator.Dispose();
        }
        catch when (failureHandled)
        {
        }
    }



    /// <summary>
    /// The asynchronous counterpart of <see cref="DisposeAfterItem"/>, applying the same rule.
    /// </summary>
    /// <param name="enumerator">The inner enumerator to dispose.</param>
    /// <param name="failureHandled">
    /// Whether this source item already had a failure put through the error policy.
    /// </param>
    private static async ValueTask DisposeAfterItemAsync
    (
        IAsyncEnumerator<TDestination> enumerator,
        bool failureHandled
    )
    {
        try
        {
            await enumerator.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
        }
        catch when (failureHandled)
        {
        }
    }



    /// <summary>
    /// Applies the configured error policy and, when it asks to skip, records the abandoned item.
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
