using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields items from the start of the input sequence as long as a
/// caller-supplied predicate returns <see langword="true"/>; the first item that fails
/// the predicate stops enumeration immediately and is not yielded.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="TakeWhileTransformer{T}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.TakeWhile{TSource}(System.Collections.Generic.IEnumerable{TSource}, System.Func{TSource, bool})"/>:
/// once the predicate returns <see langword="false"/> for an item, the transformer stops -
/// the failing item is not yielded and the source is not enumerated beyond it.
/// </para>
/// <para>
/// Two constructors are provided: one for synchronous predicates and one for asynchronous
/// predicates returning <see cref="ValueTask{TResult}"/>. The asynchronous form is useful for
/// I/O-bound stop conditions such as a state-check against an external system.
/// </para>
/// <para>
/// Implements <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/> and <see cref="IReportsItemErrors"/>
/// - no progress, no <c>SkipItemCount</c>/<c>MaximumItemCount</c> limits (unrelated to
/// <see cref="ItemErrorAction.Skip"/>, which the error policy uses to drop a failing item) - to keep
/// the hot loop minimal. The one counter is the opt-in <see cref="CurrentErrorItemCount"/>, which stays
/// at zero on the default path.
/// </para>
/// <para>
/// By default an exception thrown by the predicate propagates to the caller. Pass a
/// <see cref="DelegateTransformerOptions"/> whose <see cref="DelegateTransformerOptions.ErrorPolicy"/>
/// returns <see cref="ItemErrorAction.Skip"/> to drop the failing item and continue instead, counting
/// it in <see cref="CurrentErrorItemCount"/>. A skipped failure <b>does not end the sequence</b>:
/// terminating would be a stronger action than the policy asked for, and a predicate that threw
/// returned no verdict, so it cannot stand in for the <see langword="false"/> that normally stops the
/// run. A genuine <see langword="false"/> still ends it, and is never counted as an error.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // take rows until the first one that's incomplete
///     var head = new TakeWhileTransformer&lt;Row&gt;(r =&gt; r.IsComplete);
/// </code>
/// </example>
public sealed class TakeWhileTransformer<T> : ITransformWithCancellationAsync<T, T>, IReportsItemErrors
    where T : notnull
{
    private readonly Func<T, bool>? _syncPredicate;
    private readonly Func<T, ValueTask<bool>>? _asyncPredicate;
    private readonly ItemErrorCounter _errors;



    /// <summary>
    /// Initializes a new instance with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">A function that returns <see langword="true"/> while items should continue to be yielded.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public TakeWhileTransformer(Func<T, bool> predicate)
        : this(predicate, DelegateTransformerOptions.Default)
    {
    }



    /// <summary>
    /// Initializes a new instance with a synchronous predicate and an error policy.
    /// </summary>
    /// <param name="predicate">The predicate evaluated for each item.</param>
    /// <param name="options">Controls what happens when <paramref name="predicate"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public TakeWhileTransformer(Func<T, bool> predicate, DelegateTransformerOptions options)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(options);

        _syncPredicate = predicate;
        _errors = new ItemErrorCounter(options);
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous predicate.
    /// </summary>
    /// <param name="predicate">
    /// A function that asynchronously returns <see langword="true"/> while items should continue to be yielded.
    /// Useful for I/O-bound stop conditions.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public TakeWhileTransformer(Func<T, ValueTask<bool>> predicate)
        : this(predicate, DelegateTransformerOptions.Default)
    {
    }



    /// <summary>
    /// Initializes a new instance with an asynchronous predicate and an error policy.
    /// </summary>
    /// <param name="predicate">The predicate evaluated for each item.</param>
    /// <param name="options">Controls what happens when <paramref name="predicate"/> throws.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="predicate"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public TakeWhileTransformer(Func<T, ValueTask<bool>> predicate, DelegateTransformerOptions options)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(options);

        _asyncPredicate = predicate;
        _errors = new ItemErrorCounter(options);
    }



    /// <summary>
    /// The number of items dropped so far because the predicate threw and the error policy returned
    /// <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Per run: restarts at zero when a new enumeration begins while no other one is active. Only a thrown exception
    /// counts; ending the sequence because the predicate returned <see langword="false"/> is
    /// ordinary behaviour and is not an error.
    /// </remarks>
    public int CurrentErrorItemCount => _errors.Count;



    /// <summary>
    /// Asynchronously yields items from the start of <paramref name="items"/> while the
    /// configured predicate returns <see langword="true"/>.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing the leading run of items for which the predicate
    /// returns <see langword="true"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return _syncPredicate is not null
            ? TakeWhileWithSyncPredicateAsync(items, _syncPredicate)
            : TakeWhileWithAsyncPredicateAsync(items, _asyncPredicate!);
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

        return _syncPredicate is not null
            ? TakeWhileWithSyncPredicateAsync(items, _syncPredicate, token: token)
            : TakeWhileWithAsyncPredicateAsync(items, _asyncPredicate!, token: token);
    }



    private async IAsyncEnumerable<T> TakeWhileWithSyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, bool> predicate,
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

            bool keepTaking;
            try
            {
                keepTaking = predicate(item);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                if (_errors.Handle(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    // Drop the item and keep evaluating. Ending the sequence here would be a
                    // stronger action than Skip asks for: a predicate that threw gave no verdict,
                    // so it cannot stand in for the false that normally stops the run.
                    continue;
                }

                throw;
            }

            if (!keepTaking)
            {
                yield break;
            }

            yield return item;
        }
    }



    private async IAsyncEnumerable<T> TakeWhileWithAsyncPredicateAsync
    (
        IAsyncEnumerable<T> items,
        Func<T, ValueTask<bool>> predicate,
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

            bool keepTaking;
            try
            {
                keepTaking = await predicate(item).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                if (_errors.Handle(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    // Drop the item and keep evaluating. Ending the sequence here would be a
                    // stronger action than Skip asks for: a predicate that threw gave no verdict,
                    // so it cannot stand in for the false that normally stops the run.
                    continue;
                }

                throw;
            }

            if (!keepTaking)
            {
                yield break;
            }

            yield return item;
        }
    }
}
