// ---------------------------------------------------------------------------
// BufferedPipeline Example
// ---------------------------------------------------------------------------
//
// This example shows why BufferedTransformer<T> exists.
//
// With plain IAsyncEnumerable<T> chaining, every stage in the pipeline runs
// on the same logical call path: the downstream stage's MoveNextAsync drives
// the upstream stage one item at a time. Throughput is bounded by the
// SLOWEST stage; fast stages idle while waiting.
//
// Inserting a BufferedTransformer<T> between two stages introduces a producer
// task that drains the upstream into a bounded channel. The downstream stage
// then reads from the channel. The two sides run CONCURRENTLY, so total
// throughput approaches MAX(stage speeds) instead of MIN.
//
// To make the difference visible we use:
//
//   - A SOURCE that simulates I/O latency (e.g. fetching pages from an API)
//   - A SINK that simulates I/O latency (e.g. writing rows to a database)
//
// Without a buffer, those two latencies are SERIAL. With a buffer, they
// OVERLAP - while the sink is writing item N the source is already
// fetching item N+1.
//
// We measure both runs and print the wall-clock difference.
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Transformers;

const int itemCount = 20;
const int sourceDelayMs = 25;
const int sinkDelayMs = 25;

// ---------------------------------------------------------------------------
// Run 1: NO BUFFER - source and sink are serialized.
//
// Approximate wall-clock time:  itemCount * (sourceDelayMs + sinkDelayMs)
// ---------------------------------------------------------------------------

Console.WriteLine($"Items: {itemCount}    source delay: {sourceDelayMs} ms    sink delay: {sinkDelayMs} ms");
Console.WriteLine();

Console.WriteLine("Run 1 - no buffer (serial source <-> sink):");

var swSerial = Stopwatch.StartNew();
await ConsumeAsync(SlowSource(itemCount, sourceDelayMs));
swSerial.Stop();

var serialMs = swSerial.ElapsedMilliseconds;
var serialTheoretical = itemCount * (sourceDelayMs + sinkDelayMs);
Console.WriteLine($"  wall-clock: {serialMs} ms   (theoretical serial: {serialTheoretical} ms)");
Console.WriteLine();

// ---------------------------------------------------------------------------
// Run 2: WITH BUFFER - source and sink overlap.
//
// Approximate wall-clock time:  itemCount * MAX(sourceDelayMs, sinkDelayMs)
//                               + small startup cost for the first item
// ---------------------------------------------------------------------------

Console.WriteLine("Run 2 - BufferedTransformer<int>(capacity: 4) between source and sink:");

var buffered = new BufferedTransformer<int>(capacity: 4);

var swBuffered = Stopwatch.StartNew();
await ConsumeAsync(buffered.TransformAsync(SlowSource(itemCount, sourceDelayMs)));
swBuffered.Stop();

var bufferedMs = swBuffered.ElapsedMilliseconds;
var bufferedTheoretical = itemCount * Math.Max(sourceDelayMs, sinkDelayMs);
Console.WriteLine($"  wall-clock: {bufferedMs} ms   (theoretical max-bound: {bufferedTheoretical} ms)");
Console.WriteLine();

// ---------------------------------------------------------------------------
// Comparison
// ---------------------------------------------------------------------------

var speedup = serialMs / (double)Math.Max(1, bufferedMs);
Console.WriteLine(new string('-', 60));
Console.WriteLine($"Speedup with BufferedTransformer: {speedup.ToString("0.00", CultureInfo.InvariantCulture)}x");
Console.WriteLine();
Console.WriteLine("With equal source and sink delays, theoretical max speedup is 2x.");
Console.WriteLine("Real speedup is slightly less because the channel and producer-task");
Console.WriteLine("scheduling add a small constant overhead.");

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

// Simulates an I/O-bound source - each MoveNextAsync waits 'delayMs'
// before yielding the next integer.
static async IAsyncEnumerable<int> SlowSource(int count, int delayMs)
{
    for (var i = 0; i < count; i++)
    {
        await Task.Delay(delayMs).ConfigureAwait(false);
        yield return i;
    }
}

// Simulates an I/O-bound sink - each iteration of the loop body waits
// 'sinkDelayMs' before "writing" the item. We just await the delay; the
// actual item is unused for this measurement.
static async Task ConsumeAsync(IAsyncEnumerable<int> items)
{
    await foreach (var _ in items.ConfigureAwait(false))
    {
        await Task.Delay(sinkDelayMs).ConfigureAwait(false);
    }
}
