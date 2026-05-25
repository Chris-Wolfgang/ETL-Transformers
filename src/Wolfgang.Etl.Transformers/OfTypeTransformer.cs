using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields each input item that is of type <typeparamref name="TDestination"/>,
/// silently skipping items of any other type.
/// </summary>
/// <typeparam name="TSource">The type of items in the input sequence. Must be non-null.</typeparam>
/// <typeparam name="TDestination">
/// The type to filter for. Must be non-null. No compile-time relationship with
/// <typeparamref name="TSource"/> is required - the check is a runtime <c>is</c> test.
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="OfTypeTransformer{TSource, TDestination}"/> is the transformer equivalent of LINQ's
/// <see cref="System.Linq.Enumerable.OfType{TResult}(System.Collections.IEnumerable)"/>:
/// it filters and casts in a single step, yielding only items that satisfy
/// <c>item is TDestination</c>.
/// </para>
/// <para>
/// Items that do not match <typeparamref name="TDestination"/> are silently skipped - this
/// transformer never throws <see cref="InvalidCastException"/>. Use
/// <c>CastTransformer&lt;TSource, TDestination&gt;</c> instead if mismatched items should
/// raise an exception rather than be filtered out.
/// </para>
/// <para>
/// <see langword="null"/> items (which can technically appear if the source bypasses the
/// <c>notnull</c> constraint via <c>null!</c>) are also skipped, since <c>null is T</c> is
/// <see langword="false"/> for any <c>T</c>.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // pull the strings out of a heterogeneous object stream
///     var strings = new OfTypeTransformer&lt;object, string&gt;();
///
///     // narrow an animal stream to only the dogs
///     var dogs = new OfTypeTransformer&lt;Animal, Dog&gt;();
/// </code>
/// </example>
public sealed class OfTypeTransformer<TSource, TDestination> : ITransformAsync<TSource, TDestination>
    where TSource : notnull
    where TDestination : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OfTypeTransformer{TSource, TDestination}"/> class.
    /// </summary>
    public OfTypeTransformer()
    {
    }



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> that is an instance of
    /// <typeparamref name="TDestination"/>.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing only those input items that are of type
    /// <typeparamref name="TDestination"/>, cast to that type.
    /// </returns>
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

        return FilterByTypeAsync(items);
    }



    private static async IAsyncEnumerable<TDestination> FilterByTypeAsync(IAsyncEnumerable<TSource> items)
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            if (item is TDestination typed)
            {
                yield return typed;
            }
        }
    }
}
