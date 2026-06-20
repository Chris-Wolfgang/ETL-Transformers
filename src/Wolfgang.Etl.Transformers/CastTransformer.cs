using System;
using System.Collections.Generic;
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
/// Use <see cref="OfTypeTransformer{TSource, TDestination}"/> instead when mismatched items
/// should be silently filtered out rather than raise an exception.
/// </para>
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
public sealed class CastTransformer<TSource, TDestination> : ITransformAsync<TSource, TDestination>
    where TSource : notnull
    where TDestination : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CastTransformer{TSource, TDestination}"/> class.
    /// </summary>
    public CastTransformer()
    {
    }



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

        return CastAsync(items);
    }



    private static async IAsyncEnumerable<TDestination> CastAsync(IAsyncEnumerable<TSource> items)
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            yield return (TDestination)(object)item;
        }
    }
}
