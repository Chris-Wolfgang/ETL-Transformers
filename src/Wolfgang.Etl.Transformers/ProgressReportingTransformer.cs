using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A decorator transformer that calls a user-supplied callback for each item as it passes
/// through, then yields the item unchanged. Useful for reporting progress, logging, or
/// collecting metrics on any stage in a composed pipeline.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="ProgressReportingTransformer{T}"/> is a pure pass-through: it does not filter,
/// project, or reorder items. The callback is the only observable side-effect.
/// </para>
/// <para>
/// Insert it between any two stages in a chain to observe items at that point:
/// </para>
/// <code>
///     long count = 0;
///     var reporter = new ProgressReportingTransformer&lt;Order&gt;(item =&gt;
///         myProgress.Report(Interlocked.Increment(ref count)));
///
///     var pipeline = parse
///         .Then(reporter)       // taps the stream after parsing
///         .Then(validate);
/// </code>
/// <para>
/// Because it implements <see cref="ITransformAsync{TSource, TDestination}"/>, it composes
/// naturally with <see cref="TransformerExtensions"/>.
/// </para>
/// <para>
/// Callbacks are invoked on the caller's thread (or task) for each item before it is yielded
/// to the downstream stage. Long-running synchronous callbacks will block the pipeline; use the
/// async overload if the callback itself performs I/O.
/// </para>
/// <para>
/// Exceptions thrown by the callback propagate to the consumer through the normal
/// <see cref="IAsyncEnumerable{T}"/> pull contract.
/// </para>
/// </remarks>
public sealed class ProgressReportingTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly Action<T>?            _syncCallback;
    private readonly Func<T, ValueTask>?   _asyncCallback;



    /// <summary>
    /// Initializes a new instance that invokes a synchronous callback for each item.
    /// </summary>
    /// <param name="callback">
    /// The action to invoke for each item before it is yielded downstream.
    /// Must not be <see langword="null"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <see langword="null"/>.</exception>
    public ProgressReportingTransformer(Action<T> callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        _syncCallback = callback;
    }



    /// <summary>
    /// Initializes a new instance that invokes an asynchronous callback for each item.
    /// </summary>
    /// <param name="callback">
    /// The async function to await for each item before it is yielded downstream.
    /// Must not be <see langword="null"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <see langword="null"/>.</exception>
    public ProgressReportingTransformer(Func<T, ValueTask> callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        _asyncCallback = callback;
    }



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/>, invoking the callback
    /// once per item before yielding it downstream.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>
    /// An asynchronous sequence containing the same items as <paramref name="items"/>, in the
    /// same order.
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

        return _asyncCallback != null
            ? ReportAsync(items, _asyncCallback)
            : ReportSync(items, _syncCallback!);
    }



    private static async IAsyncEnumerable<T> ReportSync(IAsyncEnumerable<T> items, Action<T> callback)
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            callback(item);
            yield return item;
        }
    }



    private static async IAsyncEnumerable<T> ReportAsync(IAsyncEnumerable<T> items, Func<T, ValueTask> callback)
    {
        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            await callback(item).ConfigureAwait(continueOnCapturedContext: false);
            yield return item;
        }
    }
}
