using System;
using System.Collections.Generic;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// Combines two transformers into a single transformer: items flow through the first,
/// then through the second, with the intermediate type erased from the chain's public
/// signature.
/// </summary>
/// <typeparam name="TSource">The input type of the chain (the input type of the first transformer). Must be non-null.</typeparam>
/// <typeparam name="TIntermediate">The output type of the first transformer and input type of the second. Must be non-null.</typeparam>
/// <typeparam name="TDestination">The output type of the chain (the output type of the second transformer). Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="ChainTransformer{TSource, TIntermediate, TDestination}"/> is the building block for
/// composing multi-stage transformer pipelines as a single <see cref="ITransformAsync{TSource, TDestination}"/>
/// that can be passed to a loader, registered in DI, or otherwise treated as a unit.
/// </para>
/// <para>
/// For chains longer than two stages, prefer the
/// <see cref="TransformerExtensions.Then{TSource, TIntermediate, TDestination}(ITransformAsync{TSource, TIntermediate}, ITransformAsync{TIntermediate, TDestination})"/>
/// extension method, which nests <see cref="ChainTransformer{TSource, TIntermediate, TDestination}"/>
/// instances and lets the C# compiler infer all type parameters:
/// </para>
/// <example>
/// <code>
///     // 5-stage pipeline, all intermediate types inferred
///     var pipeline = parseRaw
///         .Then(normalize)
///         .Then(lookupCustomer)
///         .Then(validate)
///         .Then(formatForLoad);
/// </code>
/// </example>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - matching the
/// lightweight pattern of the rest of the library. The chain itself adds no per-item
/// state-machine wrapping; it returns the inner transformers' enumerables composed directly.
/// </para>
/// <para>
/// Formerly known internally as <c>MultiStepTransformer</c>.
/// </para>
/// </remarks>
public sealed class ChainTransformer<TSource, TIntermediate, TDestination>
    : ITransformAsync<TSource, TDestination>
    where TSource : notnull
    where TIntermediate : notnull
    where TDestination : notnull
{
    private readonly ITransformAsync<TSource, TIntermediate> _first;
    private readonly ITransformAsync<TIntermediate, TDestination> _second;



    /// <summary>
    /// Initializes a new instance with the two transformers that make up the chain.
    /// </summary>
    /// <param name="first">The first transformer; consumes <typeparamref name="TSource"/> items and produces <typeparamref name="TIntermediate"/> items.</param>
    /// <param name="second">The second transformer; consumes <typeparamref name="TIntermediate"/> items and produces <typeparamref name="TDestination"/> items.</param>
    /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
    public ChainTransformer
    (
        ITransformAsync<TSource, TIntermediate> first,
        ITransformAsync<TIntermediate, TDestination> second
    )
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
#else
        if (first == null)
        {
            throw new ArgumentNullException(nameof(first));
        }
        if (second == null)
        {
            throw new ArgumentNullException(nameof(second));
        }
#endif

        _first = first;
        _second = second;
    }



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> after passing it through
    /// the first transformer and then the second.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence of the items produced by the second transformer.</returns>
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

        // No wrapping iterator - returns the inner pipeline directly. Each inner transformer
        // handles its own iteration and any state-machine cost; the chain itself is a thin
        // composition with no per-item overhead.
        return _second.TransformAsync(_first.TransformAsync(items));
    }
}
