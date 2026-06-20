using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;



namespace Wolfgang.Etl.Transformers.Benchmarks;

/// <summary>
/// Sweeps <see cref="BufferedTransformer{T}"/> channel capacity across a range of values to
/// determine whether a sensible default emerges and to quantify the throughput vs. allocation
/// trade-off at each capacity level.
/// </summary>
/// <remarks>
/// <para>
/// The benchmark simulates a realistic pipeline where both the source and the consumer incur
/// measurable per-item latency.  <see cref="BufferedTransformer{T}"/> inserts a
/// <see cref="System.Threading.Channels.Channel{T}"/> between the two stages so the producer
/// and consumer can run concurrently; capacity determines how many items can be buffered before
/// the producer blocks waiting for the consumer to drain the channel.
/// </para>
/// <para>
/// Expected shape of results:
/// <list type="bullet">
///   <item>Capacity 1 — near-sequential; minimal concurrency benefit.</item>
///   <item>Capacity 8–64 — most of the speedup is captured; allocations are modest.</item>
///   <item>Capacity 256–1024 — diminishing returns on throughput; memory cost grows.</item>
///   <item>Capacity 8192 — upper bound; essentially unbounded for typical batch sizes.</item>
/// </list>
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class BufferedTransformerCapacityBenchmarks
{
    /// <summary>Number of items flowing through the pipeline per iteration.</summary>
    [Params(100)]
    public int ItemCount { get; set; }

    /// <summary>
    /// Simulated per-item source delay (µs).  <c>Task.Delay</c> granularity on Windows is
    /// ~15 ms, so delays are implemented with a busy-spin to keep benchmark runs short while
    /// still creating realistic back-pressure.
    /// </summary>
    [Params(50)]
    public int SourceDelayMicroseconds { get; set; }

    /// <summary>Simulated per-item sink delay (µs).</summary>
    [Params(50)]
    public int SinkDelayMicroseconds { get; set; }

    /// <summary>Channel capacity values to benchmark.</summary>
    [Params(1, 8, 64, 256, 1024, 8192)]
    public int Capacity { get; set; }



    [Benchmark(Baseline = true, Description = "No buffer (baseline)")]
    public async Task<int> NoBuffer()
    {
        var count = 0;
        await foreach (var _ in ConsumeAsync(SlowSourceAsync(ItemCount, SourceDelayMicroseconds), SinkDelayMicroseconds)
                           .ConfigureAwait(continueOnCapturedContext: false))
        {
            count++;
        }

        return count;
    }



    [Benchmark(Description = "BufferedTransformer(Capacity)")]
    public async Task<int> WithBuffer()
    {
        var transformer = new BufferedTransformer<int>(Capacity);
        var buffered    = transformer.TransformAsync(SlowSourceAsync(ItemCount, SourceDelayMicroseconds));

        var count = 0;
        await foreach (var _ in ConsumeAsync(buffered, SinkDelayMicroseconds)
                           .ConfigureAwait(continueOnCapturedContext: false))
        {
            count++;
        }

        return count;
    }



    // ──────────────────────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Yields <paramref name="count"/> integers, spinning for <paramref name="delayMicroseconds"/>
    /// microseconds between each item to simulate a latency-bound source.
    /// </summary>
    private static async IAsyncEnumerable<int> SlowSourceAsync(
        int count,
        int delayMicroseconds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask.ConfigureAwait(continueOnCapturedContext: false);

        for (var i = 0; i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SpinWait(delayMicroseconds);
            yield return i;
        }
    }



    /// <summary>
    /// Drains <paramref name="source"/>, spinning for <paramref name="delayMicroseconds"/>
    /// microseconds per item to simulate a latency-bound sink.
    /// </summary>
    private static async IAsyncEnumerable<int> ConsumeAsync(
        IAsyncEnumerable<int> source,
        int delayMicroseconds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
        {
            SpinWait(delayMicroseconds);
            yield return item;
        }
    }



    /// <summary>
    /// Busy-spin for approximately <paramref name="microseconds"/> microseconds.
    /// Uses <see cref="System.Diagnostics.Stopwatch.GetTimestamp"/> for sub-millisecond
    /// resolution; avoids thread-sleep scheduler granularity on Windows.
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void SpinWait(int microseconds)
    {
        if (microseconds <= 0)
        {
            return;
        }

        var frequency = (double)System.Diagnostics.Stopwatch.Frequency;
        var target    = System.Diagnostics.Stopwatch.GetTimestamp()
                        + (long)(microseconds * frequency / 1_000_000.0);

        while (System.Diagnostics.Stopwatch.GetTimestamp() < target)
        {
            System.Threading.Thread.SpinWait(20);
        }
    }
}
