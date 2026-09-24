using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that casts each input item to <typeparamref name="TDestination"/>, throwing
/// <see cref="InvalidCastException"/> if any item is not of that type.
/// </summary>
/// <typeparam name="TSource">The type of items in the input sequence. Must be non-null.</typeparam>
/// <typeparam name="TDestination">
/// The type to cast to. Must be non-null. No compile-time relationship with
/// <typeparamref name="TSource"/> is required - the cast is performed at runtime.
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="CastTransformer{TSource, TDestination}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.Cast{TResult}(System.Collections.IEnumerable)"/>:
/// it casts every input item to <typeparamref name="TDestination"/> and throws
/// <see cref="InvalidCastException"/> if any item does not have a runtime conversion to that type.
/// </para>
/// <para>
/// There are three ways to handle an item that does not convert, and they differ in what you
/// learn about it:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// This type with no options - the default - throws <see cref="InvalidCastException"/> and ends
/// the run at the first mismatch.
/// </description>
/// </item>
/// <item>
/// <description>
/// <see cref="OfTypeTransformer{TSource, TDestination}"/> filters mismatches out <b>silently</b>:
/// no exception, no count, no record of what was dropped. Reach for it when a mixed sequence is
/// expected and the mismatches are of no interest.
/// </description>
/// </item>
/// <item>
/// <description>
/// This type with a <see cref="DelegateTransformerOptions"/> whose
/// <see cref="DelegateTransformerOptions.ErrorPolicy"/> returns <see cref="ItemErrorAction.Skip"/>
/// also drops mismatches, but <b>accounts</b> for them: each one is counted in
/// <see cref="CurrentErrorItemCount"/> and passed to the policy, which can log it or route it to a
/// dead-letter collection. Reach for it when mismatches are unexpected and you need to know which
/// ones occurred.
/// </description>
/// </item>
/// </list>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // narrow a heterogeneous object stream to strings, asserting that every item is one
///     var strings = new CastTransformer&lt;object, string&gt;();
///
///     // downcast every Animal to Dog (assumes the upstream guarantees this)
///     var dogs = new CastTransformer&lt;Animal, Dog&gt;();
/// </code>
/// </example>
public sealed class CastTransformer<TSource, TDestination> : ITransformAsync<TSource, TDestination>, IReportsItemErrors
    where TSource : notnull
    where TDestination : notnull
{
    private readonly DelegateTransformerOptions _options;
    private int _currentErrorItemCount;



    /// <summary>
    /// Initializes a new instance of the <see cref="CastTransformer{TSource, TDestination}"/> class
    /// that throws <see cref="InvalidCastException"/> on the first item that does not convert.
    /// </summary>
    public CastTransformer()
        : this(new DelegateTransformerOptions())
    {
    }



    /// <summary>
    /// Initializes a new instance with an error policy controlling what happens when an item does
    /// not convert to <typeparamref name="TDestination"/>.
    /// </summary>
    /// <param name="options">
    /// Controls the response to a failed conversion. Unlike the other transformers that take this
    /// record, there is no caller-supplied delegate here - the failure being handled is the cast
    /// itself, and the exception handed to the policy is an <see cref="InvalidCastException"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    public CastTransformer(DelegateTransformerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
    }



    /// <summary>
    /// The number of items dropped so far because they did not convert and the error policy
    /// returned <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    /// <remarks>
    /// Cumulative across every enumeration produced by this instance. An item whose failure aborted
    /// the run is not counted, because the exception is re-thrown instead.
    /// </remarks>
    public int CurrentErrorItemCount => Volatile.Read(ref _currentErrorItemCount);



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> cast to
    /// <typeparamref name="TDestination"/>.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence of the same items, cast to <typeparamref name="TDestination"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">
    /// An item in <paramref name="items"/> is not assignable to <typeparamref name="TDestination"/>.
    /// </exception>
    public IAsyncEnumerable<TDestination> TransformAsync(IAsyncEnumerable<TSource> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return CastAsync(items);
    }



    private async IAsyncEnumerable<TDestination> CastAsync(IAsyncEnumerable<TSource> items)
    {
        var itemNumber = 0L;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            itemNumber++;

            // The try/catch cannot wrap the yield: a C# async iterator cannot resume after it
            // throws, so the conversion happens first and only its result is yielded.
            TDestination converted;
            try
            {
                converted = (TDestination)(object)item;
            }
            catch (InvalidCastException exception)
            {
                if (HandleItemError(itemNumber, exception) == ItemErrorAction.Skip)
                {
                    continue;
                }

                throw;
            }

            yield return converted;
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
