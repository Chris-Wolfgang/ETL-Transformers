using System;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// Extension methods on <see cref="ITransformAsync{TSource, TDestination}"/> that compose
/// transformers into larger units.
/// </summary>
public static class TransformerExtensions
{
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
}
