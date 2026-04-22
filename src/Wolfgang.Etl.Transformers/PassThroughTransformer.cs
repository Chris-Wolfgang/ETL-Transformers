using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that yields each item from the input sequence unchanged.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="PassThroughTransformer{T}"/> is useful as:
/// </para>
/// <list type="bullet">
///   <item><description>A placeholder in an ETL pipeline when a transformer is required by the pipeline shape but no transformation is needed.</description></item>
///   <item><description>A default implementation in dependency injection, letting consumers swap in a real transformer later without restructuring the pipeline.</description></item>
///   <item><description>A test seam for verifying extractor/loader behaviour without a real transformer in the way.</description></item>
/// </list>
/// <para>
/// This class deliberately does <b>not</b> inherit from <see cref="TransformerBase{TSource, TDestination, TProgress}"/>.
/// Because it performs no work, it does not need progress reporting, skip/max item counts, or a reporting timer,
/// and implementing the interface directly keeps the type minimal and allocation-free.
/// </para>
/// <para>
/// Formerly known internally as <c>NoOpTransformer</c>.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     var transformer = new PassThroughTransformer&lt;Customer&gt;();
///     await foreach (var customer in transformer.TransformAsync(source))
///     {
///         // items are yielded unchanged
///     }
/// </code>
/// </example>
public sealed class PassThroughTransformer<T> : ITransformWithCancellationAsync<T, T>
    where T : notnull
{
    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> unchanged.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing the same items as <paramref name="items"/>, in the same order.
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
        return TransformAsyncCore(items, CancellationToken.None);
    }



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> unchanged, observing the supplied
    /// <paramref name="token"/> for cancellation.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <param name="token">A token to observe while enumerating the source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing the same items as <paramref name="items"/>, in the same order.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="token"/> was cancelled before or during enumeration.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items, CancellationToken token)
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
        return TransformAsyncCore(items, token);
    }



    private static async IAsyncEnumerable<T> TransformAsyncCore
    (
        IAsyncEnumerable<T> items,
        [EnumeratorCancellation] CancellationToken token
    )
    {
        await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
        {
            token.ThrowIfCancellationRequested();
            yield return item;
        }
    }
}
