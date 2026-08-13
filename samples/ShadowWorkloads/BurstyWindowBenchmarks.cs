using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Wolfgang.Etl.Transformers;

namespace Wolfgang.Etl.Transformers.ShadowWorkloads;

/// <summary>
/// A bursty source consumed through the windowing/tap operators — the shape of a bounded
/// take with skip/backoff and an observing tap. Exercises SkipWhile, Skip, Take, and
/// ProgressReportingTransformer.
/// </summary>
[MemoryDiagnoser]
public class BurstyWindowBenchmarks
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global — set by BenchmarkDotNet via reflection when it enumerates [Params].
    [Params(50_000)]
    public int ItemCount { get; set; }


    [Benchmark]
    public async Task<long> SkipWhileSkipTakeProgress()
    {
        long observed = 0;
        var skipWhile = new SkipWhileTransformer<int>(i => i < 10);
        var skip = new SkipTransformer<int>(5);
        var take = new TakeTransformer<int>(ItemCount - 100);
        var progress = new ProgressReportingTransformer<int>(_ => { });

        var pipeline = progress.TransformAsync(
            take.TransformAsync(
                skip.TransformAsync(
                    skipWhile.TransformAsync(Sources.Jittered(ItemCount, asyncEvery: 4096)))));
        await foreach (var _ in pipeline.ConfigureAwait(false))
        {
            observed++;
        }

        return observed;
    }
}
