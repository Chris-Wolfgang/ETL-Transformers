using System;
using System.Runtime.CompilerServices;
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
/// This type deliberately implements only <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/> and
/// does <b>not</b> inherit from <see cref="TransformerBase{TSource, TDestination, TProgress}"/>.
/// It carries no progress reporting, and counts nothing on the default
/// path - the only counter is the opt-in <see cref="CurrentErrorItemCount"/>, which stays at zero
/// unless an error policy skips an item - keeping the hot loop as small as possible for use as a
/// building block in composed pipelines.
/// </para>
/// <para>
/// By default an exception thrown by the selector propagates to the caller. Pass a
/// <see cref="DelegateTransformerOptions"/> whose <see cref="DelegateTransformerOptions.ErrorPolicy"/>
/// returns <see cref="ItemErrorAction.Skip"/> to drop the failing item and continue instead; skipped
/// items are counted by <see cref="CurrentErrorItemCount"/>. That count is per run (it restarts at zero when a
/// new enumeration begins while no other one is active), while the <see cref="ItemErrorContext.ItemNumber"/> handed to the
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
public sealed class SelectTransformer<TSource, TDestination> : ITransformWithCancellationAsync<TSource, TDestination>, IReportsItemErrors
    where TSource : notnull
    where TDestination : notnull
{
    private readonly Func<TSource, TDestination>? _syncSelector;
    private readonly Func<TSource, ValueTask<TDestination>>? _asyncSelector;
    private readonly ItemErrorCounter _errors;



    /// <summary>
    /// Initializes a new instance with a synchronous selector function.
    /// </summary>
    /// <param name="selector">A function that projects each input item to an output item.</param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectTransformer(Func<TSource, TDestination> selector)
        : this(selector, DelegateTransformerOptions.Default)
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
        _errors = new ItemErrorCounter(options);
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
        : this(selector, DelegateTransformerOptions.Default)
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
        _errors = new ItemErrorCounter(options);
    }



    /// <summary>
    /// The number of items dropped so far because the selector threw and the error policy returned
    /// <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Per run: restarts at zero when a new enumeration begins while no other one is active. An item whose failure aborted
    /// the run is not counted, because the exception is re-thrown instead.
    /// </remarks>
    public int CurrentErrorItemCount => _errors.Count;



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



    /// <summary>
    /// Transforms <paramref name="items"/> like the overload without a token, and forwards
    /// <paramref name="token"/> to the stages above this one and to the source.
    /// </summary>
    /// <param name="items">The sequence to transform.</param>
    /// <param name="token">A token that cancels the enumeration, including the source.</param>
    /// <returns>The transformed sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<TDestination> TransformAsync(IAsyncEnumerable<TSource> items, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(items);

        return _syncSelector is not null
            ? ProjectWithSyncSelectorAsync(items, _syncSelector, token: token)
            : ProjectWithAsyncSelectorAsync(items, _asyncSelector!, token: token);
    }



    private async IAsyncEnumerable<TDestination> ProjectWithSyncSelectorAsync
    (
        IAsyncEnumerable<TSource> items,
        Func<TSource, TDestination> selector,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        using var run = _errors.BeginRun();

        var itemNumber = 0L;

        // Observe the token here, not only through WithCancellation: a source that ignores
        // the token would otherwise be drained after cancellation (issues #209, #813).
        token.ThrowIfCancellationRequested();

        await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
        {
            token.ThrowIfCancellationRequested();

            itemNumber++;

            // The try/catch cannot wrap the yield: a C# async iterator cannot resume after it
            // throws, so the projection completes first and only its result is yielded.
            TDestination projected;
            try
            {
                projected = selector(item);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                if (_errors.Handle(itemNumber, exception) == ItemErrorAction.Skip)
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
        Func<TSource, ValueTask<TDestination>> selector,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        using var run = _errors.BeginRun();

        var itemNumber = 0L;

        // Observe the token here, not only through WithCancellation: a source that ignores
        // the token would otherwise be drained after cancellation (issues #209, #813).
        token.ThrowIfCancellationRequested();

        await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
        {
            token.ThrowIfCancellationRequested();

            itemNumber++;

            TDestination projected;
            try
            {
                projected = await selector(item).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                if (_errors.Handle(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            yield return projected;
        }
    }
}
