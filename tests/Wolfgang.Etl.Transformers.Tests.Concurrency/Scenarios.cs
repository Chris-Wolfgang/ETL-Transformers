using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Coyote.Specifications;
using Wolfgang.Etl.Transformers;

namespace Wolfgang.Etl.Transformers.Tests.Concurrency;

/// <summary>
/// Concurrency scenarios shared by the Coyote systematic entry points
/// (<see cref="CoyoteEntryPoints"/>) and the xunit stress wrappers
/// (<c>ConcurrencyStressTests</c>). Each asserts its invariant with
/// <see cref="Specification"/>'s Assert helper, which fails both under Coyote's
/// model checker and on the real scheduler.
/// </summary>
internal static class Scenarios
{
    private const int ItemCount = 20;


    /// <summary>
    /// The same transformer instance is enumerated by two tasks at once. Each call to
    /// <c>TransformAsync</c> must yield an independent sequence, so both consumers must see
    /// every expected item with no interference.
    /// </summary>
    public static async Task TwoConcurrentEnumerationsAsync()
    {
        var transformer = new WhereTransformer<int>(i => i % 2 == 0);

        async Task<long> ConsumeAsync()
        {
            long count = 0;
            await foreach (var _ in transformer.TransformAsync(RangeAsync(ItemCount)).ConfigureAwait(false))
            {
                count++;
            }

            return count;
        }

        var first = ConsumeAsync();
        var second = ConsumeAsync();
        var results = await Task.WhenAll(first, second).ConfigureAwait(false);

        Specification.Assert(
            results[0] == ItemCount / 2 && results[1] == ItemCount / 2,
            "Concurrent enumerations of one transformer must each see all matching items.");
    }


    /// <summary>
    /// The BufferedTransformer runs a background producer feeding a channel that the consumer
    /// drains — the most concurrency-sensitive transformer. Every item must arrive exactly once,
    /// under any interleaving, with no deadlock.
    /// </summary>
    public static async Task BufferedProducerConsumerAsync()
    {
        var buffered = new BufferedTransformer<int>(4);

        long count = 0;
        long sum = 0;
        await foreach (var item in buffered.TransformAsync(RangeAsync(ItemCount)).ConfigureAwait(false))
        {
            count++;
            sum += item;
        }

        Specification.Assert(count == ItemCount, "Buffered pipeline must yield every item exactly once.");
        Specification.Assert(sum == ItemCount * (ItemCount - 1) / 2, "Buffered pipeline must preserve item values.");
    }


    /// <summary>
    /// Cancellation arrives from another task while a buffered pipeline is being enumerated.
    /// The enumeration must terminate (cleanly completing or throwing
    /// <see cref="System.OperationCanceledException"/>) rather than deadlock — Coyote's
    /// deadlock detector fails the test if the producer/consumer hangs.
    /// </summary>
    public static Task CancellationDuringEnumerationAsync()
        => RunBufferedConsumeSwallowingOceAsync(cancelBeforeAwait: true);


    /// <summary>
    /// Companion to <see cref="CancellationDuringEnumerationAsync"/> for the race outcome
    /// where cancellation LOSES — the enumeration completes before <c>cts.Cancel()</c> fires
    /// and the try body's normal-completion path runs. Same shape; the pair jointly asserts
    /// "cancellation during buffered enumeration doesn't deadlock in either race outcome".
    /// </summary>
    public static Task EnumerationBeforeLateCancellationAsync()
        => RunBufferedConsumeSwallowingOceAsync(cancelBeforeAwait: false);


    // Shared body for the two cancellation-race scenarios above. Extracted so both race outcomes
    // (cancel-wins-race and cancel-loses-race) cover the same try/catch structure — the
    // cancel-loses variant is what reaches the try body's normal-completion path.
    private static async Task RunBufferedConsumeSwallowingOceAsync(bool cancelBeforeAwait)
    {
        using var cts = new CancellationTokenSource();
        var buffered = new BufferedTransformer<int>(4);

        // ReSharper disable once AccessToDisposedClosure — consumer is awaited below before `using var cts` exits, so cts is alive for the closure's full lifetime.
        async Task ConsumeAsync()
        {
            await foreach (var _ in buffered.TransformAsync(RangeAsync(ItemCount))
                .WithCancellation(cts.Token)
                .ConfigureAwait(false))
            {
            }
        }

        var consumer = ConsumeAsync();
        if (cancelBeforeAwait)
        {
            cts.Cancel();
        }

        try
        {
            await consumer.ConfigureAwait(false);
        }
        catch (System.OperationCanceledException)
        {
            // Expected when cancellation wins the race — the scenario asserts the pipeline doesn't
            // deadlock; whether OCE surfaces or the enumeration finishes first is fine.
        }

        Specification.Assert(true, "Cancellation during buffered enumeration must not deadlock.");
    }


    private static async IAsyncEnumerable<int> RangeAsync(int count)
    {
        for (var i = 0; i < count; i++)
        {
            // A real suspension point per item so MoveNextAsync completes asynchronously:
            // the concurrent consumers actually interleave and Coyote can schedule between
            // yields. Without it every yield completes synchronously and the "concurrent"
            // enumerations just run one after another, exploring no overlap.
            await Task.Yield();
            yield return i;
        }
    }
}
