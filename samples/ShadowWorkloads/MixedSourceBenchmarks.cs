using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Wolfgang.Etl.Transformers;

namespace Wolfgang.Etl.Transformers.ShadowWorkloads;

/// <summary>
/// A mixed sync/async source through an async-shaped selector, fan-out, and dedup — the
/// shape of enrichment stages that occasionally await I/O. Exercises Select (async),
/// SelectMany, and Distinct.
/// </summary>
[MemoryDiagnoser]
public class MixedSourceBenchmarks
{
    [Params(50_000)]
    public int ItemCount { get; set; }


    [Benchmark]
    public async Task<long> AsyncSelectorSelectManyDistinct()
    {
        var select = new SelectTransformer<int, int>(i => new ValueTask<int>(i % 1000));
        var selectMany = new SelectManyTransformer<int, int>(i => Enumerable.Range(0, i % 4));
        var distinct = new DistinctTransformer<int>();

        long count = 0;
        var pipeline = distinct.TransformAsync(
            selectMany.TransformAsync(
                select.TransformAsync(Sources.Jittered(ItemCount, asyncEvery: 512))));
        await foreach (var _ in pipeline.ConfigureAwait(false))
        {
            count++;
        }

        return count;
    }
}
