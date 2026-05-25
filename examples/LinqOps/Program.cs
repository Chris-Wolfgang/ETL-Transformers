// ---------------------------------------------------------------------------
// LinqOps Example
// ---------------------------------------------------------------------------
//
// This example tours the LINQ-style transformers in Wolfgang.Etl.Transformers.
// Each is a standalone ITransformAsync<,> that mirrors a familiar LINQ
// operator. They compose with .Then(...) into a single transformer.
//
// The pipeline:
//
//   raw integers (with duplicates)
//      |
//      |  Where:    keep evens
//      v
//   even integers
//      |
//      |  Distinct: drop repeats
//      v
//   distinct evens
//      |
//      |  Select:   project to a richer record
//      v
//   Bucket records
//      |
//      |  Take:     first N (windowing)
//      v
//   first N Bucket records
//      |
//      |  Chunk:    group into arrays of size 3
//      v
//   Bucket[]   (one or more arrays, last may be partial)
//
// Each stage is a standalone transformer. The chain composes them with
// full type inference at every .Then(...).
// ---------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.TestKit;
using Wolfgang.Etl.Transformers;

// ---------------------------------------------------------------------------
// Source: 1..30, with a few intentional duplicates so Distinct has work to do.
// ---------------------------------------------------------------------------

var source = Enumerable.Range(1, 30)
    .Concat(new[] { 4, 8, 12, 16, 20 })   // duplicates to be deduped
    .ToArray();

// ---------------------------------------------------------------------------
// Build the chain.
//
// Notes on each transformer:
//
//   WhereTransformer<T>        : ITransformAsync<T, T>
//                                Sync OR async predicate via two ctors.
//
//   DistinctTransformer<T>     : ITransformAsync<T, T>
//                                Default- or custom-comparer ctor.
//                                Memory grows with unique-item count.
//
//   SelectTransformer<TS, TD>  : ITransformAsync<TS, TD>
//                                Sync OR async selector via two ctors.
//
//   TakeTransformer<T>         : ITransformAsync<T, T>
//                                Negative/zero count yields empty without
//                                enumerating the source.
//
//   ChunkTransformer<T>        : ITransformAsync<T, T[]>
//                                Last chunk may be smaller than 'size'.
//                                Each yielded chunk is a fresh array.
// ---------------------------------------------------------------------------

var pipeline = new WhereTransformer<int>(i => i % 2 == 0)
    .Then(new DistinctTransformer<int>())
    .Then(new SelectTransformer<int, Bucket>(i => new Bucket(i, $"item-{i:D3}")))
    .Then(new TakeTransformer<Bucket>(count: 8))
    .Then(new ChunkTransformer<Bucket>(size: 3));

// ---------------------------------------------------------------------------
// Wire the pipeline between TestKit endpoints.
// ---------------------------------------------------------------------------

var extractor = new TestExtractor<int>(source);
var loader = new TestLoader<Bucket[]>(collectItems: true);
var token = CancellationToken.None;

await loader.LoadAsync
(
    pipeline.TransformAsync(extractor.ExtractAsync(token)),
    token
);

// ---------------------------------------------------------------------------
// Show what each stage did.
// ---------------------------------------------------------------------------

Console.WriteLine("Source ({0} items, with duplicates):", source.Length);
Console.WriteLine("  " + string.Join(", ", source.Select(i => i.ToString(CultureInfo.InvariantCulture))));
Console.WriteLine();

Console.WriteLine("After Where(even) + Distinct + Select(Bucket) + Take(8) + Chunk(3):");
Console.WriteLine(new string('-', 70));

var chunkIndex = 0;
foreach (var chunk in loader.GetCollectedItems()!)
{
    chunkIndex++;
    var rendered = string.Join(", ", chunk.Select(b => $"({b.Number},{b.Label})"));
    Console.WriteLine($"  chunk #{chunkIndex} ({chunk.Length} item{(chunk.Length == 1 ? string.Empty : "s")}):  {rendered}");
}

Console.WriteLine(new string('-', 70));
var chunks = loader.GetCollectedItems()!;
Console.WriteLine($"Total chunks emitted: {chunks.Count}");
Console.WriteLine($"Total Bucket items:   {chunks.Sum(c => c.Length)}");

// ---------------------------------------------------------------------------
// Bucket - intermediate record type produced by the Select stage.
// ---------------------------------------------------------------------------

public sealed record Bucket(int Number, string Label);
