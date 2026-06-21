using System;
using System.Collections.Generic;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// Extension methods that compose transformers and wrap sequences with pipeline infrastructure.
/// </summary>
public static class TransformerExtensions
{
    /// <summary>
    /// Inserts a <see cref="BufferedTransformer{T}"/> into a sequence, decoupling the upstream
    /// producer from the downstream consumer so both stages can run concurrently.
    /// </summary>
    /// <typeparam name="T">The type of items in the sequence. Must be non-null.</typeparam>
    /// <param name="source">The source sequence to buffer. Must not be <see langword="null"/>.</param>
    /// <param name="capacity">
    /// The maximum number of items the internal buffer holds. Must be at least 1.
    /// </param>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{T}"/> containing the same items as <paramref name="source"/>,
    /// in the same order, but produced concurrently with consumption.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
    /// <remarks>
    /// <para>
    /// Sugar for <c>new <see cref="BufferedTransformer{T}"/>(capacity).TransformAsync(source)</c>.
    /// See <see cref="BufferedTransformer{T}"/> for a full description of the buffering semantics,
    /// cancellation handling, and error propagation.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    ///     var select = new SelectTransformer&lt;RawRecord, ParsedRecord&gt;(Parse);
    ///     var results = select.TransformAsync(extractor.ExtractAsync(token).Buffered(capacity: 500));
    ///     await foreach (var item in results) { ... }
    /// </code>
    /// </example>
    public static IAsyncEnumerable<T> Buffered<T>
    (
        this IAsyncEnumerable<T> source,
        int capacity
    )
        where T : notnull
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#else
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
#endif

        return new BufferedTransformer<T>(capacity).TransformAsync(source);
    }



    /// <summary>
    /// Composes two transformers into a single one: items flow through <paramref name="first"/>,
    /// then through <paramref name="next"/>. Equivalent to constructing
    /// <c>new ChainTransformer&lt;TSource, TIntermediate, TDestination&gt;(first, next)</c>
    /// but with all type parameters inferred at the call site.
    /// </summary>
    /// <typeparam name="TSource">The input type of the chain. Must be non-null.</typeparam>
    /// <typeparam name="TIntermediate">The intermediate type between the two transformers. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The output type of the chain. Must be non-null.</typeparam>
    /// <param name="first">The transformer that runs first.</param>
    /// <param name="next">The transformer that runs after <paramref name="first"/>.</param>
    /// <returns>An <see cref="ITransformAsync{TSource, TDestination}"/> representing the composed pipeline.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="first"/> or <paramref name="next"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Multiple stages compose by chaining successive <c>.Then(...)</c> calls. Each call returns
    /// a new <see cref="ChainTransformer{TSource, TIntermediate, TDestination}"/> with the
    /// previous chain as its first member, so the resulting structure is a left-leaning linked
    /// list of two-stage chains. The C# compiler infers all type parameters from the receiver
    /// and argument.
    /// </para>
    /// <example>
    /// <code>
    ///     var pipeline = parseRaw       // string  -> DataRow
    ///         .Then(normalize)          // DataRow -> DataRow
    ///         .Then(lookupCustomer)     // DataRow -> CustomerRow
    ///         .Then(validate)           // CustomerRow -> CustomerRow
    ///         .Then(formatForLoad);     // CustomerRow -> LoadRow
    ///
    ///     // pipeline is ITransformAsync&lt;string, LoadRow&gt;
    /// </code>
    /// </example>
    /// </remarks>
    public static ITransformAsync<TSource, TDestination> Then<TSource, TIntermediate, TDestination>
    (
        this ITransformAsync<TSource, TIntermediate> first,
        ITransformAsync<TIntermediate, TDestination> next
    )
        where TSource : notnull
        where TIntermediate : notnull
        where TDestination : notnull
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(next);
#else
        if (first == null)
        {
            throw new ArgumentNullException(nameof(first));
        }
        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }
#endif

        return new ChainTransformer<TSource, TIntermediate, TDestination>(first, next);
    }



    /// <summary>
    /// Composes two cancellation-aware transformers into a single one: items flow through
    /// <paramref name="first"/>, then through <paramref name="next"/>, and any
    /// <see cref="System.Threading.CancellationToken"/> supplied to the resulting chain is
    /// propagated to both stages.
    /// </summary>
    /// <typeparam name="TSource">The input type of the chain. Must be non-null.</typeparam>
    /// <typeparam name="TIntermediate">The intermediate type between the two transformers. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The output type of the chain. Must be non-null.</typeparam>
    /// <param name="first">The cancellation-aware transformer that runs first.</param>
    /// <param name="next">The cancellation-aware transformer that runs after <paramref name="first"/>.</param>
    /// <returns>An <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/> representing the composed pipeline.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="first"/> or <paramref name="next"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This overload is selected by the C# compiler when both arguments implement
    /// <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/> (more specific than
    /// the base <see cref="ITransformAsync{TSource, TDestination}"/> overload). The returned
    /// chain is itself <see cref="ITransformWithCancellationAsync{TSource, TDestination}"/>, so
    /// subsequent <c>.Then(...)</c> calls in a longer chain also pick this overload, allowing
    /// arbitrary-length cancellation-aware chains to compose without ceremony.
    /// </para>
    /// </remarks>
    public static ITransformWithCancellationAsync<TSource, TDestination> Then<TSource, TIntermediate, TDestination>
    (
        this ITransformWithCancellationAsync<TSource, TIntermediate> first,
        ITransformWithCancellationAsync<TIntermediate, TDestination> next
    )
        where TSource : notnull
        where TIntermediate : notnull
        where TDestination : notnull
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(next);
#else
        if (first == null)
        {
            throw new ArgumentNullException(nameof(first));
        }
        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }
#endif

        return new ChainTransformerWithCancellation<TSource, TIntermediate, TDestination>(first, next);
    }
}
