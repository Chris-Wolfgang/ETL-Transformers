using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;



namespace Wolfgang.Etl.Transformers.Benchmarks;

/// <summary>
/// Compares the lightweight <see cref="WhereTransformer{T}"/> (direct <c>ITransformAsync</c>
/// implementation) against <see cref="WhereTransformerWithBase{T}"/> (inherits
/// <see cref="Wolfgang.Etl.Abstractions.TransformerBase{TSource, TDestination, TProgress}"/>)
/// to quantify the overhead of the shared base class on a pure-filter workload.
/// </summary>
[MemoryDiagnoser]
public class WhereBenchmarks
{
    private int[] _source = Array.Empty<int>();
    private Func<int, bool> _predicate = _ => true;



    [Params(1_000, 100_000, 1_000_000)]
    public int ItemCount { get; set; }



    [Params(0.1, 0.5, 0.9)]
    public double PassRate { get; set; }



    [GlobalSetup]
    public void Setup()
    {
        _source = new int[ItemCount];
        for (var i = 0; i < ItemCount; i++)
        {
            _source[i] = i;
        }

        // Keep a consistent modulo threshold so JIT/branch-predictor behaviour is stable.
        // PassRate of 0.1 -> item % 10 == 0 (keep 10%)
        // PassRate of 0.5 -> item % 2  == 0 (keep 50%)
        // PassRate of 0.9 -> item % 10 != 0 (keep 90%)
        _predicate = PassRate switch
        {
            0.1 => i => i % 10 == 0,
            0.5 => i => i % 2 == 0,
            0.9 => i => i % 10 != 0,
            _   => _ => true,
        };
    }



    [Benchmark(Baseline = true)]
    public async Task<int> Lightweight()
    {
        var sut = new WhereTransformer<int>(_predicate);
        var count = 0;
        await foreach (var _ in sut.TransformAsync(ToAsync(_source)).ConfigureAwait(continueOnCapturedContext: false))
        {
            count++;
        }
        return count;
    }



    [Benchmark]
    public async Task<int> WithBase()
    {
        var sut = new WhereTransformerWithBase<int>(_predicate);
        var count = 0;
        await foreach (var _ in sut.TransformAsync(ToAsync(_source)).ConfigureAwait(continueOnCapturedContext: false))
        {
            count++;
        }
        return count;
    }



    // Synchronously-completing async iterator. `await Task.CompletedTask` satisfies the
    // async-method contract without forcing a scheduler hop per item - this way we measure
    // the transformer's overhead, not the source's scheduling.
    private static async IAsyncEnumerable<int> ToAsync(int[] items)
    {
        await Task.CompletedTask.ConfigureAwait(continueOnCapturedContext: false);
        foreach (var item in items)
        {
            yield return item;
        }
    }
}
