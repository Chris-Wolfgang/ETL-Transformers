using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that skips items from the start of the input sequence as long as a
/// caller-supplied predicate returns <see langword="true"/>; once the predicate returns
/// <see langword="false"/> for an item, that item and every item after it are yielded without
/// further predicate evaluation.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="SkipWhileTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.SkipWhile{TSource}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, bool})"/>.
/// The predicate is called on each item only until it first returns <see langword="false"/>;
/// after that, every remaining item flows through unchanged.
/// </para>
/// <para>
/// Two constructors are provided: one for synchronous predicates and one for asynchronous
/// predicates returning <see cref="ValueTask{TResult}"/>. The asynchronous form is useful for
/// I/O-bound start conditions such as a state-check against an external system.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal.
/// </para>
/// <para>
/// Exceptions thrown by the predicate propagate to the caller.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // skip leading blank rows; once a non-blank row appears, yield everything from there
///     var fromFirstReal = new SkipWhileTransformer&lt;Row&gt;(r =&gt; r.IsBlank);
/// </code>
/// </example>
public sealed class SkipWhileTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;



    /// <summary>
    /// Initializes a new instance with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> while items should be skipped.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SkipWhileTransformer(Func<T, bool> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        _syncPredicate = predicate;
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous predicate.
    /// </summary>
    /// <param name="predicate">
    /// A function that asynchronously returns <see langword="true"/> while items should be skipped.
    /// Useful for I/O-bound start conditions.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public SkipWhileTransformer(Func<T, ValueTask<bool>> predicate)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(predicate);
#else
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
#endif

        _asyncPredicate = predicate;
    }



    /// <summary>
    /// Asynchronously skips items from the start of <paramref name="items"/> while the
    /// configured predicate returns <see langword="true"/>, then yields the rest unchanged.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing every item from the first one where the predicate
    /// returns <see langword="false"/> to the end of <paramref name="items"/>.
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

        return _syncPredicate is not null
            ? SkipWhileWithSyncPredicateAsync(items, _syncPredicate)
            : SkipWhileWithAsyncPredicateAsync(items, _asyncPredicate!);
    }



    private static async IAsyncEnumerable<T> SkipWhileWithSyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, bool> predicate
    )
    {
        var skipping = true;
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (skipping)
            {
                if (predicate(item))
                {
                    continue;
                }

                skipping = false;
            }

            yield return item;
        }
    }



    private static async IAsyncEnumerable<T> SkipWhileWithAsyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, ValueTask<bool>> predicate
    )
    {
        var skipping = true;
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (skipping)
            {
                if (await predicate(item).ConfigureAwait(continueOnCapturedContext: false))
                {
                    continue;
                }

                skipping = false;
            }

            yield return item;
        }
    }
}
