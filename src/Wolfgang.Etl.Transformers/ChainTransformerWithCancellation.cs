using System;
using System.Collections.Generic;
using System.Threading;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// Combines two cancellation-aware transformers into a single
/// <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/>: items flow through
/// the first, then through the second, and a single <see cref="CancellationToken"/> supplied
/// to the chain is propagated to both stages.
/// </summary>
/// <typeparam name="TSource">The input type of the chain (the input type of the first transformer). Must be non-null.</typeparam>
/// <typeparam name="TIntermediate">The output type of the first transformer and input type of the second. Must be non-null.</typeparam>
/// <typeparam name="TDestination">The output type of the chain (the output type of the second transformer). Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// This is the cancellation-aware sibling of
/// <see cref="ChainTransformer{TSource, TIntermediate, TDestination}"/>. Use it when both inner
/// transformers implement <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/>
/// and you want a token passed to the chain to flow into both stages.
/// </para>
/// <para>
/// For chains longer than two stages, prefer the
/// <see cref="TransformerExtensions.Then{TSource, TIntermediate, TDestination}(ITransformWithCancellationAsync{TSource, TIntermediate}, ITransformWithCancellationAsync{TIntermediate, TDestination})"/>
/// overload, which lets the C# compiler infer all type parameters and pick this class
/// automatically when both arguments support cancellation.
/// </para>
/// <para>
/// Progress reporting is intentionally not supported at the chain level - two transformers
/// almost always declare different <c>TProgress</c> types and there is no general way to
/// combine them. Cancellation is different: <see cref="CancellationToken"/> is a single
/// concrete type, so propagating it to both stages is unambiguous.
/// </para>
/// <para>
/// The chain itself adds no per-item state-machine wrapping; <c>TransformAsync</c> in both
/// overloads composes the inner transformers' enumerables directly.
/// </para>
/// </remarks>
public sealed class ChainTransformerWithCancellation<TSource, TIntermediate, TDestination>
    : ITransformWithCancellationAsync<TSource, TDestination>
    where TSource : notnull
    where TIntermediate : notnull
    where TDestination : notnull
{
    private readonly ITransformWithCancellationAsync<TSource, TIntermediate> _first;
    private readonly ITransformWithCancellationAsync<TIntermediate, TDestination> _second;



    /// <summary>
    /// Initializes a new instance with the two cancellation-aware transformers that make up the chain.
    /// </summary>
    /// <param name="first">The first transformer; consumes <typeparamref name="TSource"/> items and produces <typeparamref name="TIntermediate"/> items.</param>
    /// <param name="second">The second transformer; consumes <typeparamref name="TIntermediate"/> items and produces <typeparamref name="TDestination"/> items.</param>
    /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
    public ChainTransformerWithCancellation
    (
        ITransformWithCancellationAsync<TSource, TIntermediate> first,
        ITransformWithCancellationAsync<TIntermediate, TDestination> second
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

        return _second.TransformAsync(_first.TransformAsync(items));
    }



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> after passing it through
    /// the first transformer and then the second, propagating <paramref name="token"/> to both
    /// stages.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <param name="token">The token observed by both stages of the chain.</param>
    /// <returns>An asynchronous sequence of the items produced by the second transformer.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<TDestination> TransformAsync(IAsyncEnumerable<TSource> items, CancellationToken token)
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

        return _second.TransformAsync(_first.TransformAsync(items, token), token);
    }
}
