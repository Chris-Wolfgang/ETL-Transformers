using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers.Benchmarks;

/// <summary>
/// Throughput of each transformer that has no benchmark of its own, over the same synchronous
/// 100,000-item source, so <c>pr-benchmarks.yaml</c> can see a regression in any of them.
/// </summary>
/// <remarks>
/// <see cref="WhereBenchmarks"/> and <see cref="BufferedTransformerCapacityBenchmarks"/> already cover
/// <see cref="WhereTransformer{T}"/> and <see cref="BufferedTransformer{T}"/>. Every benchmark here
/// counts what the transformer yields so the JIT cannot drop the loop.
/// </remarks>
[MemoryDiagnoser]
public class OperatorThroughputBenchmarks
{
    private const int ItemCount = 100_000;

    // Every 100th item makes the projection in SelectWithSkipPolicy throw, so the
    // DelegateTransformerOptions skip path runs 1,000 times per invocation.
    private const int FailEvery = 100;

    private static readonly DelegateTransformerOptions SkipOptions = new()
    {
        ErrorPolicy = static _ => ItemErrorAction.Skip,
    };

    private int[] _source = Array.Empty<int>();



    [GlobalSetup]
    public void Setup()
    {
        _source = new int[ItemCount];
        for (var i = 0; i < ItemCount; i++)
        {
            // 1,000 distinct values, so Distinct/DistinctBy do real duplicate detection.
            _source[i] = i % 1_000;
        }
    }



    [Benchmark]
    public Task<int> Select()
    {
        return CountAsync
        (
            new SelectTransformer<int, long>(static i => i * 2L)
                .TransformAsync(ToAsync(_source))
        );
    }



    [Benchmark]
    public Task<int> SelectMany()
    {
        return CountAsync
        (
            new SelectManyTransformer<int, int>(static i => new[] { i, i })
                .TransformAsync(ToAsync(_source))
        );
    }



    [Benchmark]
    public Task<int> Chunk()
    {
        return CountAsync
        (
            new ChunkTransformer<int>(100)
                .TransformAsync(ToAsync(_source))
        );
    }



    [Benchmark]
    public Task<int> Distinct()
    {
        return CountAsync
        (
            new DistinctTransformer<int>()
                .TransformAsync(ToAsync(_source))
        );
    }



    [Benchmark]
    public Task<int> DistinctBy()
    {
        return CountAsync
        (
            new DistinctByTransformer<int, int>(static i => i % 500)
                .TransformAsync(ToAsync(_source))
        );
    }



    // TimeSpan.Zero disables pacing, so this measures the throttle's per-item bookkeeping
    // rather than the delay it would add.
    [Benchmark]
    public Task<int> Throttle()
    {
        return CountAsync
        (
            new ThrottleTransformer<int>(TimeSpan.Zero)
                .TransformAsync(ToAsync(_source))
        );
    }



    [Benchmark]
    public Task<int> SelectWithSkipPolicy()
    {
        return CountAsync
        (
            new SelectTransformer<int, int>(ThrowOnEveryHundredth, SkipOptions)
                .TransformAsync(ToAsync(_source))
        );
    }



    private static int ThrowOnEveryHundredth(int item)
    {
        if (item % FailEvery == 0)
        {
            throw new InvalidOperationException("Benchmark failure for the skip policy.");
        }

        return item;
    }



    private static async Task<int> CountAsync<T>(IAsyncEnumerable<T> items)
    {
        var count = 0;
        await foreach (var _ in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            count++;
        }

        return count;
    }



    // Synchronously-completing async iterator, as in WhereBenchmarks: measures the transformer's
    // overhead, not the source's scheduling.
    private static async IAsyncEnumerable<int> ToAsync(int[] items)
    {
        await Task.CompletedTask.ConfigureAwait(continueOnCapturedContext: false);
        foreach (var item in items)
        {
            yield return item;
        }
    }
}
