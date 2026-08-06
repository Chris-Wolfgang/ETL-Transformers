using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static VerifyXunit.Verifier;

namespace Wolfgang.Etl.Transformers.Tests.Snapshots;

/// <summary>
/// Snapshot (approval) tests over representative complex-shaped transformer
/// output. The library is mostly a pure-data pass-through (no text/JSON/XML/CSV
/// rendering), so snapshot coverage is intentionally narrow — see
/// <c>docs/snapshot-testing.md</c>. These lock the shape of a record projection,
/// a chunk grouping, and an end-to-end pipeline so accidental drift is caught by
/// a diff against the committed <c>.verified.txt</c>.
/// </summary>
public class TransformerSnapshotTests
{
    private sealed record Person(int Id, string Name, string City);

    private static readonly Person[] People =
    {
        new(1, "Ada", "London"),
        new(2, "Alan", "London"),
        new(3, "Grace", "New York"),
        new(4, "Edsger", "Rotterdam"),
        new(5, "Barbara", "New York"),
    };

    [Fact]
    public async Task Select_projection_to_records()
    {
        var result = await Collect
        (
            new SelectTransformer<Person, object>(p => new { p.Id, Upper = p.Name.ToUpperInvariant() })
                .TransformAsync(ToAsync(People))
        );

        await Verify(result);
    }

    [Fact]
    public async Task Chunk_grouping_shape()
    {
        var result = await Collect(new ChunkTransformer<Person>(2).TransformAsync(ToAsync(People)));

        await Verify(result);
    }

    [Fact]
    public async Task Pipeline_filter_dedup_project()
    {
        var byCity = new DistinctByTransformer<Person, string>(p => p.City);
        var project = new SelectTransformer<Person, object>(p => new { p.City, Representative = p.Name });

        var result = await Collect(project.TransformAsync(byCity.TransformAsync(ToAsync(People))));

        await Verify(result);
    }

    private static async IAsyncEnumerable<T> ToAsync<T>(
        IEnumerable<T> items,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        foreach (var item in items)
        {
            token.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return item;
        }
    }

    private static async Task<List<T>> Collect<T>(IAsyncEnumerable<T> items)
    {
        var list = new List<T>();
        await foreach (var item in items.ConfigureAwait(false))
        {
            list.Add(item);
        }

        return list;
    }
}
