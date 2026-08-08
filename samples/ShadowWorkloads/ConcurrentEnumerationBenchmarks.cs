using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Wolfgang.Etl.Transformers;

namespace Wolfgang.Etl.Transformers.ShadowWorkloads;

/// <summary>
/// Many pipelines enumerated concurrently — the shape of a server handling parallel
/// requests. Exercises BufferedTransformer under contention plus Where.
/// </summary>
[MemoryDiagnoser]
public class ConcurrentEnumerationBenchmarks
{
    [Params(8)]
    public int Concurrency { get; set; }


    [Params(20_000)]
    public int ItemsPerWorker { get; set; }


    [Benchmark]
    public async Task<long> ConcurrentBufferedPipelines()
    {
        var tasks = new Task<long>[Concurrency];
        for (var worker = 0; worker < Concurrency; worker++)
        {
            tasks[worker] = RunWorkerAsync(ItemsPerWorker);
        }

        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        long total = 0;
        foreach (var result in results)
        {
            total += result;
        }

        return total;
    }


    private static async Task<long> RunWorkerAsync(int count)
    {
        long seen = 0;
        var buffered = new BufferedTransformer<int>(500);
        var where = new WhereTransformer<int>(i => i % 3 != 0);

        var pipeline = where.TransformAsync(buffered.TransformAsync(Sources.Range(count)));
        await foreach (var _ in pipeline.ConfigureAwait(false))
        {
            seen++;
        }

        return seen;
    }
}
