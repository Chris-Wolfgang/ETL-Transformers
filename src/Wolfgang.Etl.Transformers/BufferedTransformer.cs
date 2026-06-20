using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that decouples its upstream and downstream stages using a bounded
/// <see cref="Channel{T}"/>, enabling pipeline parallelism: the upstream stage can run ahead
/// while the downstream stage consumes, instead of being lock-stepped by
/// <see cref="IAsyncEnumerable{T}"/>'s pull model.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// With plain <see cref="IAsyncEnumerable{T}"/> chaining, every stage runs on the same logical
/// call path - throughput is bounded by the slowest stage and fast stages idle while waiting.
/// Inserting a <see cref="BufferedTransformer{T}"/> between two stages introduces a producer
/// task that drains the upstream into a bounded buffer; the downstream stage then reads from
/// the buffer. The two sides run concurrently, so total throughput approaches
/// <c>max(stage speeds)</c> rather than <c>min(stage speeds)</c>.
/// </para>
/// <para>
/// The buffer is bounded - once full, the producer task awaits free space, providing
/// backpressure on the source.
/// </para>
/// <para>
/// <b>Cancellation:</b> consumers do not need a transformer-level token. External cancellation
/// supplied via <c>.WithCancellation(token)</c> on the returned sequence is propagated to the
/// internal producer task via a linked <see cref="CancellationTokenSource"/>, so the source
/// enumerator and producer are cleaned up promptly.
/// </para>
/// <para>
/// <b>Error propagation:</b> exceptions thrown by the source enumerator or by writes into the
/// buffer are surfaced to the consumer through the channel - the consumer's <c>await foreach</c>
/// throws the original exception (unwrapped) once any already-buffered items have drained.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - matching the
/// lightweight pattern of the rest of the library. The producer task's lifecycle is managed
/// internally via the iterator's disposal contract.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     extractor
///         .Pipe(new WhereTransformer&lt;Row&gt;(r =&gt; r.IsValid))
///         .Pipe(new BufferedTransformer&lt;Row&gt;(capacity: 500))   // parallelism boundary
///         .Pipe(new SelectTransformer&lt;Row, Record&gt;(Parse))
///         .Pipe(loader);
/// </code>
/// </example>
public sealed class BufferedTransformer<T> : ITransformAsync<T, T>
    where T : notnull
{
    private readonly int _capacity;



    /// <summary>
    /// Initializes a new instance with the given buffer capacity.
    /// </summary>
    /// <param name="capacity">
    /// The maximum number of items that may be buffered between the upstream and downstream
    /// stages. Must be at least 1.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
    public BufferedTransformer(int capacity)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
#else
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Buffer capacity must be greater than 0.");
        }
#endif

        _capacity = capacity;
    }



    /// <summary>
    /// The maximum number of items the internal buffer holds.
    /// </summary>
    public int Capacity => _capacity;



    /// <summary>
    /// Asynchronously yields each item from <paramref name="items"/>, with a bounded buffer
    /// in between that allows the upstream and downstream stages to run concurrently.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence containing the same items as <paramref name="items"/>, in the same order.</returns>
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

        return BufferAsync(items, _capacity);
    }



    private static async IAsyncEnumerable<T> BufferAsync
    (
        IAsyncEnumerable<T> items,
        int capacity,
        [EnumeratorCancellation] CancellationToken externalToken = default
    )
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        var channel = Channel.CreateBounded<T>
        (
            new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.Wait,
            }
        );

        var producer = Task.Run(() => PumpAsync(items, channel, cts.Token), cts.Token);

        try
        {
            await foreach (var item in channel.Reader.ReadAllAsync(cts.Token).ConfigureAwait(continueOnCapturedContext: false))
            {
                yield return item;
            }
        }
        finally
        {
#if NET8_0_OR_GREATER
            await cts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
#else
#pragma warning disable VSTHRD103 // CancelAsync not available pre-net8
            cts.Cancel();
#pragma warning restore VSTHRD103
#endif
#pragma warning disable CA1031 // producer faults were already surfaced via the channel
            try
            {
                await producer.ConfigureAwait(continueOnCapturedContext: false);
            }
            catch
            {
                // Swallow: any real fault was already delivered through the channel.
            }
#pragma warning restore CA1031
        }
    }



    /// <summary>
    /// The producer task body: drains the source into the channel, completing the channel
    /// normally on natural end, or with an exception on fault. Cancellation - whether from
    /// external <c>.WithCancellation</c> or from consumer abandonment - completes the channel
    /// quietly without surfacing as a fault.
    /// </summary>
    private static async Task PumpAsync(IAsyncEnumerable<T> items, Channel<T> channel, CancellationToken token)
    {
        try
        {
            await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
            {
                await channel.Writer.WriteAsync(item, token).ConfigureAwait(continueOnCapturedContext: false);
            }

            channel.Writer.Complete();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            channel.Writer.TryComplete();
        }
#pragma warning disable CA1031 // intentional: surface ANY producer fault to the consumer via the channel
        catch (Exception ex)
        {
            channel.Writer.TryComplete(ex);
        }
#pragma warning restore CA1031
    }
}
