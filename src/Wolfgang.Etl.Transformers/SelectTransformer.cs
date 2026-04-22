using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
/// Two constructors are provided: one for synchronous selectors and one for asynchronous selectors
/// that accept the enumeration's <see cref="CancellationToken"/>. The asynchronous form is useful
/// for I/O-bound transformations (database lookups, HTTP calls, etc.) that should observe cancellation.
/// </para>
/// <para>
/// Exceptions thrown by the selector propagate to the caller. If an item needs to be skipped on error
/// the caller is expected to handle that inside the selector.
/// </para>
/// <para>
/// Inherits from <see cref="TransformerBase{TSource, TDestination, TProgress}"/> so consumers get
/// <see cref="TransformerBase{TSource, TDestination, TProgress}.SkipItemCount"/>,
/// <see cref="TransformerBase{TSource, TDestination, TProgress}.MaximumItemCount"/>, and periodic
/// progress callbacks via <see cref="Report"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // synchronous projection
///     var toUpper = new SelectTransformer&lt;string, string&gt;(s =&gt; s.ToUpperInvariant());
///
///     // asynchronous projection that respects cancellation
///     var lookup = new SelectTransformer&lt;int, Customer&gt;
///     (
///         async (id, token) =&gt; await customerService.GetByIdAsync(id, token).ConfigureAwait(false)
///     );
/// </code>
/// </example>
public sealed class SelectTransformer<TSource, TDestination> : TransformerBase<TSource, TDestination, Report>
    where TSource : notnull
    where TDestination : notnull
{
    private readonly Func<TSource, CancellationToken, ValueTask<TDestination>> _selector;



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

        _selector = (item, _) => new ValueTask<TDestination>(selector(item));
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous selector function that accepts a
    /// <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="selector">
    /// A function that asynchronously projects each input item to an output item.
    /// The supplied <see cref="CancellationToken"/> is the same token passed to
    /// <see cref="TransformerBase{TSource, TDestination, TProgress}.TransformAsync(IAsyncEnumerable{TSource}, CancellationToken)"/>
    /// and its progress-reporting overloads.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    public SelectTransformer(Func<TSource, CancellationToken, ValueTask<TDestination>> selector)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(selector);
#else
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }
#endif

        _selector = selector;
    }



    /// <summary>
    /// Projects each item from <paramref name="items"/> through the selector supplied at construction,
    /// honouring <see cref="TransformerBase{TSource, TDestination, TProgress}.SkipItemCount"/> and
    /// <see cref="TransformerBase{TSource, TDestination, TProgress}.MaximumItemCount"/>.
    /// </summary>
    protected override async IAsyncEnumerable<TDestination> TransformWorkerAsync
    (
        IAsyncEnumerable<TSource> items,
        [EnumeratorCancellation] CancellationToken token
    )
    {
        var skipRemaining = SkipItemCount;
        var maxRemaining = MaximumItemCount;

        await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
        {
            token.ThrowIfCancellationRequested();

            if (skipRemaining > 0)
            {
                skipRemaining--;
                IncrementCurrentSkippedItemCount();
                continue;
            }

            if (maxRemaining <= 0)
            {
                yield break;
            }

            var result = await _selector(item, token).ConfigureAwait(continueOnCapturedContext: false);
            maxRemaining--;
            IncrementCurrentItemCount();
            yield return result;
        }
    }



    /// <summary>
    /// Creates a <see cref="Report"/> snapshot reflecting the current transformed-item count.
    /// </summary>
    /// <returns>A new <see cref="Report"/> with <see cref="Report.CurrentItemCount"/> set to the current count.</returns>
    protected override Report CreateProgressReport()
    {
        return new Report(CurrentItemCount);
    }
}
