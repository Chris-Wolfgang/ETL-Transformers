using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Wolfgang.Etl.Transformers;

namespace Wolfgang.Etl.Transformers.ShadowWorkloads;

/// <summary>
/// Paged extraction with wildly variable page sizes — the shape of SQL/HTTP paging where
/// each page returns an unpredictable count. Exercises Where → Select → Chunk.
/// </summary>
[MemoryDiagnoser]
public class VariableSizeStreamingBenchmarks
{
    private int[] _pageSizes = Array.Empty<int>();


    [Params(50)]
    public int PageCount { get; set; }


    [GlobalSetup]
    public void Setup()
    {
        // Deterministic pseudo-variable sizes (1..~20k) — no RNG, so BDN stays reproducible.
        _pageSizes = new int[PageCount];
        for (var i = 0; i < PageCount; i++)
        {
            _pageSizes[i] = 1 + ((i * 7919) % 20000);
        }
    }


    [Benchmark]
    public async Task<long> WhereSelectChunkOverVariablePages()
    {
        long total = 0;
        foreach (var size in _pageSizes)
        {
            var where = new WhereTransformer<int>(i => (i & 1) == 0);
            var select = new SelectTransformer<int, long>(i => (long)i);
            var chunk = new ChunkTransformer<long>(256);

            var pipeline = chunk.TransformAsync(select.TransformAsync(where.TransformAsync(Sources.Range(size))));
            await foreach (var batch in pipeline.ConfigureAwait(false))
            {
                total += batch.Count;
            }
        }

        return total;
    }
}
