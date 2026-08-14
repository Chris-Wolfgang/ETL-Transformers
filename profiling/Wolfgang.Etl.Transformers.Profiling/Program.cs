using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Wolfgang.Etl.Transformers;

namespace Wolfgang.Etl.Transformers.Profiling;

/// <summary>
/// Sustained-load driver for GC / allocation profiling (issue #89). Runs a representative
/// transformer pipeline in a tight loop for a configurable duration so an external profiler
/// (dotnet-counters / dotnet-trace / dotnet-gcdump) can characterise gen0/1/2 promotion,
/// LOH pressure, and allocation rate under continuous throughput.
/// </summary>
internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var durationSeconds = GetInt(args, "--duration-seconds", 600);
        var batchSize = GetInt(args, "--batch-size", 100_000);
        var scenario = GetString(args, "--scenario", "mixed");

        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"Profiling scenario='{scenario}' duration={durationSeconds}s batch={batchSize} PID={Environment.ProcessId} ServerGC={System.Runtime.GCSettings.IsServerGC}"));

        var stopwatch = Stopwatch.StartNew();
        long itemsProcessed = 0;
        long batches = 0;

        while (stopwatch.Elapsed.TotalSeconds < durationSeconds)
        {
            itemsProcessed += await RunOnceAsync(scenario, batchSize).ConfigureAwait(false);
            batches++;

            if (batches % 50 == 0)
            {
                Console.WriteLine(string.Create(
                    CultureInfo.InvariantCulture,
                    $"[{stopwatch.Elapsed:hh\\:mm\\:ss}] batches={batches} items={itemsProcessed:N0} gen0={GC.CollectionCount(0)} gen1={GC.CollectionCount(1)} gen2={GC.CollectionCount(2)}"));
            }
        }

        stopwatch.Stop();
        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"Done. batches={batches} items={itemsProcessed:N0} elapsed={stopwatch.Elapsed} gen0={GC.CollectionCount(0)} gen1={GC.CollectionCount(1)} gen2={GC.CollectionCount(2)}"));
        return 0;
    }


    private static async Task<long> RunOnceAsync(string scenario, int count)
    {
        long consumed = 0;

        switch (scenario)
        {
            case "buffered":
            {
                var buffered = new BufferedTransformer<int>(500);
                await foreach (var _ in buffered.TransformAsync(SourceAsync(count)).ConfigureAwait(false))
                {
                    consumed++;
                }

                break;
            }

            case "linq":
            {
                var where = new WhereTransformer<int>(i => i % 2 == 0);
                var select = new SelectTransformer<int, int>(i => i * 2);
                var distinct = new DistinctByTransformer<int, int>(i => i % 4096);
                var chunk = new ChunkTransformer<int>(256);

                var pipeline = chunk.TransformAsync(
                    distinct.TransformAsync(
                        select.TransformAsync(
                            where.TransformAsync(SourceAsync(count)))));

                await foreach (var batch in pipeline.ConfigureAwait(false))
                {
                    consumed += batch.Count;
                }

                break;
            }

            default:
            {
                // "mixed": filter -> project (widening) -> buffer -> chunk, the shape a real
                // streaming ETL stage tends to have.
                var where = new WhereTransformer<int>(i => i % 3 != 0);
                var select = new SelectTransformer<int, long>(i => (long)i * i);
                var buffered = new BufferedTransformer<long>(1000);
                var chunk = new ChunkTransformer<long>(512);

                var pipeline = chunk.TransformAsync(
                    buffered.TransformAsync(
                        select.TransformAsync(
                            where.TransformAsync(SourceAsync(count)))));

                await foreach (var batch in pipeline.ConfigureAwait(false))
                {
                    consumed += batch.Count;
                }

                break;
            }
        }

        return consumed;
    }


    // Synchronously-completing async source: maximises throughput so the pressure measured
    // is the transformers' own, not a scheduler's.
    private static async IAsyncEnumerable<int> SourceAsync(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return i;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }


    private static int GetInt(string[] args, string name, int fallback)
    {
        var raw = GetStringOrNull(args, name);
        return raw is not null && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;
    }


    private static string GetString(string[] args, string name, string fallback)
    {
        return GetStringOrNull(args, name) ?? fallback;
    }


    private static string? GetStringOrNull(string[] args, string name)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], name, StringComparison.Ordinal))
            {
                return args[i + 1];
            }
        }

        return null;
    }
}
