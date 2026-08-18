using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// Paces an asynchronous sequence so that successive items are yielded no closer together than a
/// minimum interval, without changing the stream's shape or order. Useful for rate-limiting a
/// downstream sink (an API, a database) that must not be hit too fast.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// The pacing is <b>adaptive</b>: the delay before an item is the remaining time until the minimum
/// interval since the previous item has elapsed, so a consumer that was already slow is not delayed
/// further. The first item is never delayed. All waits observe the enumeration's
/// <see cref="CancellationToken"/> (supply it via <c>.WithCancellation(token)</c>).
/// </para>
/// </remarks>
/// <example>
/// <code>
///     var throttle = new ThrottleTransformer&lt;Request&gt;(TimeSpan.FromMilliseconds(200));
///     await foreach (var request in throttle.TransformAsync(source))
///     {
///         // at most ~5 items/second
///     }
/// </code>
/// </example>
public sealed class ThrottleTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly TimeSpan _minInterval;
    private readonly Func<TimeSpan, CancellationToken, Task> _delayAsync;
    private readonly Func<long> _getTimestamp;


    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottleTransformer{T}"/> class.
    /// </summary>
    /// <param name="minInterval">
    /// The minimum time between successive yielded items. <see cref="TimeSpan.Zero"/> disables pacing
    /// (a pass-through).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minInterval"/> is negative.</exception>
    public ThrottleTransformer(TimeSpan minInterval)
        : this(minInterval, static (delay, token) => Task.Delay(delay, token), Stopwatch.GetTimestamp)
    {
    }


    // Test seam: inject a delay and a monotonic timestamp source so pacing can be verified
    // deterministically without real time passing.
    internal ThrottleTransformer(TimeSpan minInterval, Func<TimeSpan, CancellationToken, Task> delayAsync, Func<long> getTimestamp)
    {
        if (minInterval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(minInterval), minInterval, "The minimum interval must not be negative.");
        }

        _minInterval = minInterval;
        _delayAsync = delayAsync ?? throw new ArgumentNullException(nameof(delayAsync));
        _getTimestamp = getTimestamp ?? throw new ArgumentNullException(nameof(getTimestamp));
    }


    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/> in order, pacing successive items
    /// to at least the configured minimum interval apart.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence with the same items, in the same order, paced apart.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<T> TransformAsync(IAsyncEnumerable<T> items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
#pragma warning disable RCS1140
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#pragma warning restore RCS1140
#endif
        return TransformAsyncCore(items);
    }


    private async IAsyncEnumerable<T> TransformAsyncCore
    (
        IAsyncEnumerable<T> items,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        // Observe cancellation BEFORE MoveNextAsync — see issue #209.
        // Calling WithCancellation on the source below only OFFERS the token to the source's
        // enumerator; a plain sequence-backed source still yields its first element regardless.
        // Without this pre-check one item is pulled from a non-replayable source when the caller
        // hands us an already-cancelled token.
        token.ThrowIfCancellationRequested();

        var hasPrevious = false;
        long previousTimestamp = 0;

        await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
        {
            if (hasPrevious && _minInterval > TimeSpan.Zero)
            {
                var elapsed = TimeSpan.FromSeconds((_getTimestamp() - previousTimestamp) / (double)Stopwatch.Frequency);
                var wait = _minInterval - elapsed;
                if (wait > TimeSpan.Zero)
                {
                    await _delayAsync(wait, token).ConfigureAwait(continueOnCapturedContext: false);
                }
            }

            // Record the delivery time BEFORE yielding. `yield return` suspends here until the
            // consumer requests the next item, so recording afterwards would start the interval
            // clock only when the consumer comes back — excluding the consumer's own processing
            // time and imposing a full extra delay even when the consumer already spent longer
            // than the interval. Measuring from delivery makes the pacing adaptive: successive
            // items are at least _minInterval apart, with no wait when the consumer is already
            // the slower party.
            previousTimestamp = _getTimestamp();
            hasPrevious = true;

            yield return item;
        }
    }
}
