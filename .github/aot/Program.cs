using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Transformers;

// Roots every public method of the library so the trimmer/AOT compiler must
// analyze the whole reachable graph. A silent no-op is caught by the count
// assertions; a trim/AOT-incompatible construct fails the publish (IL2xxx/IL3xxx)
// or faults the native binary at runtime.

var failures = new List<string>();

async Task Check(string name, int expected, IAsyncEnumerable<object?> sequence)
{
    var count = 0;
    await foreach (var _ in sequence.ConfigureAwait(false))
    {
        count++;
    }

    if (count != expected)
    {
        failures.Add($"{name}: expected {expected} items, got {count}");
    }
    else
    {
        Console.WriteLine($"  ok  {name} ({count})");
    }
}

static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> items, [EnumeratorCancellation] CancellationToken token = default)
{
    foreach (var item in items)
    {
        token.ThrowIfCancellationRequested();
        await Task.Yield();
        yield return item;
    }
}

static IAsyncEnumerable<object?> Box<T>(IAsyncEnumerable<T> source) => Cast(source);

static async IAsyncEnumerable<object?> Cast<T>(IAsyncEnumerable<T> source, [EnumeratorCancellation] CancellationToken token = default)
{
    await foreach (var item in source.WithCancellation(token).ConfigureAwait(false))
    {
        yield return item;
    }
}

var ints = new[] { 1, 2, 3, 4, 5 };
var withDupes = new[] { 1, 1, 2, 3, 3 };
var objects = new object[] { "a", 1, "b", 2 };

Console.WriteLine("Transformers:");
await Check("Where", 2, Box(new WhereTransformer<int>(x => x % 2 == 0).TransformAsync(ToAsync(ints))));
await Check("Where(async)", 2, Box(new WhereTransformer<int>(x => new ValueTask<bool>(x % 2 == 0)).TransformAsync(ToAsync(ints))));
await Check("Select", 5, Box(new SelectTransformer<int, string>(x => x.ToString()).TransformAsync(ToAsync(ints))));
await Check("Select(async)", 5, Box(new SelectTransformer<int, int>(x => new ValueTask<int>(x)).TransformAsync(ToAsync(ints))));
await Check("SelectMany", 10, Box(new SelectManyTransformer<int, int>(x => new[] { x, -x }).TransformAsync(ToAsync(ints))));
await Check("SelectMany(async)", 5, Box(new SelectManyTransformer<int, int>(x => ToAsync(new[] { x })).TransformAsync(ToAsync(ints))));
await Check("Distinct", 3, Box(new DistinctTransformer<int>().TransformAsync(ToAsync(withDupes))));
await Check("Distinct(comparer)", 3, Box(new DistinctTransformer<int>(EqualityComparer<int>.Default).TransformAsync(ToAsync(withDupes))));
await Check("DistinctBy", 2, Box(new DistinctByTransformer<int, int>(x => x % 2).TransformAsync(ToAsync(ints))));
await Check("Take", 2, Box(new TakeTransformer<int>(2).TransformAsync(ToAsync(ints))));
await Check("Skip", 3, Box(new SkipTransformer<int>(2).TransformAsync(ToAsync(ints))));
await Check("TakeWhile", 2, Box(new TakeWhileTransformer<int>(x => x < 3).TransformAsync(ToAsync(ints))));
await Check("TakeWhile(async)", 2, Box(new TakeWhileTransformer<int>(x => new ValueTask<bool>(x < 3)).TransformAsync(ToAsync(ints))));
await Check("SkipWhile", 3, Box(new SkipWhileTransformer<int>(x => x < 3).TransformAsync(ToAsync(ints))));
await Check("SkipWhile(async)", 3, Box(new SkipWhileTransformer<int>(x => new ValueTask<bool>(x < 3)).TransformAsync(ToAsync(ints))));
await Check("Chunk", 3, Box(new ChunkTransformer<int>(2).TransformAsync(ToAsync(ints))));
await Check("Chunk(progress)", 3, Box(new ChunkTransformer<int>(2, new Progress<int>()).TransformAsync(ToAsync(ints))));
await Check("Cast", 4, Box(new CastTransformer<object, object>().TransformAsync(ToAsync(objects))));
await Check("OfType", 2, Box(new OfTypeTransformer<object, string>().TransformAsync(ToAsync(objects))));
await Check("PassThrough", 5, Box(new PassThroughTransformer<int>().TransformAsync(ToAsync(ints))));
await Check("PassThrough(ct)", 5, Box(new PassThroughTransformer<int>().TransformAsync(ToAsync(ints), CancellationToken.None)));
await Check("Buffered", 5, Box(new BufferedTransformer<int>(2).TransformAsync(ToAsync(ints))));
await Check("ProgressReporting", 5, Box(new ProgressReportingTransformer<int>(_ => { }).TransformAsync(ToAsync(ints))));
await Check("ProgressReporting(async)", 5, Box(new ProgressReportingTransformer<int>(_ => default).TransformAsync(ToAsync(ints))));
await Check("ThrottleTransformer", 5, Box(new ThrottleTransformer<int>(TimeSpan.Zero).TransformAsync(ToAsync(ints))));

Console.WriteLine("Composition:");
var chain = new ChainTransformer<int, int, string>(new WhereTransformer<int>(x => x > 1), new SelectTransformer<int, string>(x => x.ToString()));
await Check("ChainTransformer", 4, Box(chain.TransformAsync(ToAsync(ints))));
var chainCt = new ChainTransformerWithCancellation<int, int, int>(new PassThroughTransformer<int>(), new PassThroughTransformer<int>());
await Check("ChainTransformerWithCancellation", 5, Box(chainCt.TransformAsync(ToAsync(ints), CancellationToken.None)));
var then = new WhereTransformer<int>(x => x > 1).Then(new SelectTransformer<int, string>(x => x.ToString()));
await Check("Then", 4, Box(then.TransformAsync(ToAsync(ints))));
var thenCt = new PassThroughTransformer<int>().Then(new PassThroughTransformer<int>());
await Check("Then(cancellation)", 5, Box(thenCt.TransformAsync(ToAsync(ints), CancellationToken.None)));
await Check("Buffered(extension)", 5, Box(ToAsync(ints).Buffered(2)));

Console.WriteLine("Pipeline operators:");
await Check("op Where", 2, Box(EtlPipeline.Create().From(ToAsync(ints)).Where(x => x % 2 == 0).AsAsyncEnumerable()));
await Check("op Where(async)", 2, Box(EtlPipeline.Create().From(ToAsync(ints)).Where(x => new ValueTask<bool>(x % 2 == 0)).AsAsyncEnumerable()));
await Check("op Select", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).Select(x => x.ToString()).AsAsyncEnumerable()));
await Check("op Select(async)", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).Select(x => new ValueTask<int>(x)).AsAsyncEnumerable()));
await Check("op SelectMany", 10, Box(EtlPipeline.Create().From(ToAsync(ints)).SelectMany(x => new[] { x, -x }).AsAsyncEnumerable()));
await Check("op SelectMany(async)", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).SelectMany(x => ToAsync(new[] { x })).AsAsyncEnumerable()));
await Check("op Distinct", 3, Box(EtlPipeline.Create().From(ToAsync(withDupes)).Distinct().AsAsyncEnumerable()));
await Check("op DistinctBy", 2, Box(EtlPipeline.Create().From(ToAsync(ints)).DistinctBy(x => x % 2).AsAsyncEnumerable()));
await Check("op Take", 2, Box(EtlPipeline.Create().From(ToAsync(ints)).Take(2).AsAsyncEnumerable()));
await Check("op Skip", 3, Box(EtlPipeline.Create().From(ToAsync(ints)).Skip(2).AsAsyncEnumerable()));
await Check("op TakeWhile", 2, Box(EtlPipeline.Create().From(ToAsync(ints)).TakeWhile(x => x < 3).AsAsyncEnumerable()));
await Check("op TakeWhile(async)", 2, Box(EtlPipeline.Create().From(ToAsync(ints)).TakeWhile(x => new ValueTask<bool>(x < 3)).AsAsyncEnumerable()));
await Check("op SkipWhile", 3, Box(EtlPipeline.Create().From(ToAsync(ints)).SkipWhile(x => x < 3).AsAsyncEnumerable()));
await Check("op SkipWhile(async)", 3, Box(EtlPipeline.Create().From(ToAsync(ints)).SkipWhile(x => new ValueTask<bool>(x < 3)).AsAsyncEnumerable()));
await Check("op Chunk", 3, Box(EtlPipeline.Create().From(ToAsync(ints)).Chunk(2).AsAsyncEnumerable()));
await Check("op Buffered", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).Buffered(2).AsAsyncEnumerable()));
await Check("op Cast", 4, Box(EtlPipeline.Create().From(ToAsync(objects)).Cast<object, object>().AsAsyncEnumerable()));
await Check("op OfType", 2, Box(EtlPipeline.Create().From(ToAsync(objects)).OfType<object, string>().AsAsyncEnumerable()));

Console.WriteLine("Observability operators (0.4.0):");
await Check("op Tap", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).Tap(_ => { }).AsAsyncEnumerable()));
await Check("op Tap(async)", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).Tap(_ => default).AsAsyncEnumerable()));
await Check("op Log", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).Log(x => x.ToString(), _ => { }).AsAsyncEnumerable()));
await Check("op Throttle", 5, Box(EtlPipeline.Create().From(ToAsync(ints)).Throttle(TimeSpan.Zero).AsAsyncEnumerable()));

if (failures.Count > 0)
{
    Console.Error.WriteLine($"AOT smoke FAILED — {failures.Count} mismatch(es):");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("  " + failure);
    }

    return 1;
}

Console.WriteLine("AOT smoke passed — full public surface ran natively.");
return 0;
