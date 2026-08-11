using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CsCheck;
using Xunit;

namespace Wolfgang.Etl.Transformers.Tests.Fuzz;

/// <summary>
/// Property-based tests: each transformer must be equivalent to its
/// <see cref="System.Linq"/> reference over randomized inputs. CsCheck generates
/// the cases and shrinks any counter-example to its minimal form. The per-case
/// count is small in the normal test pass and large under fuzz.yaml (via the
/// <c>CsCheck_Iter</c> / <c>CsCheck_Time</c> environment variables).
/// </summary>
public class TransformerEquivalenceProperties
{
    private static readonly Gen<List<int>> IntLists = Gen.Int[-10, 10].List[0, 50];

    [Fact]
    public async Task Where_is_equivalent_to_Linq_Where() =>
        await IntLists.SampleAsync(async list =>
        {
            var expected = list.Where(IsEven).ToList();
            var actual = await Collect(new WhereTransformer<int>(IsEven).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task Select_is_equivalent_to_Linq_Select() =>
        await IntLists.SampleAsync(async list =>
        {
            var expected = list.Select(Times10).ToList();
            var actual = await Collect(new SelectTransformer<int, long>(Times10).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task SelectMany_is_equivalent_to_Linq_SelectMany() =>
        await IntLists.SampleAsync(async list =>
        {
            var expected = list.SelectMany(PlusMinus).ToList();
            var actual = await Collect(new SelectManyTransformer<int, int>(PlusMinus).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task Distinct_is_equivalent_to_Linq_Distinct() =>
        await IntLists.SampleAsync(async list =>
        {
            var expected = list.Distinct().ToList();
            var actual = await Collect(new DistinctTransformer<int>().TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task DistinctBy_is_equivalent_to_Linq_DistinctBy() =>
        await IntLists.SampleAsync(async list =>
        {
            var expected = list.DistinctBy(Mod3).ToList();
            var actual = await Collect(new DistinctByTransformer<int, int>(Mod3).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task Take_is_equivalent_to_Linq_Take() =>
        await ListAndCount().SampleAsync(async pair =>
        {
            var (list, n) = pair;
            var expected = list.Take(n).ToList();
            var actual = await Collect(new TakeTransformer<int>(n).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task Skip_is_equivalent_to_Linq_Skip() =>
        await ListAndCount().SampleAsync(async pair =>
        {
            var (list, n) = pair;
            var expected = list.Skip(n).ToList();
            var actual = await Collect(new SkipTransformer<int>(n).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task TakeWhile_is_equivalent_to_Linq_TakeWhile() =>
        await IntLists.SampleAsync(async list =>
        {
            var expected = list.TakeWhile(IsNonNegative).ToList();
            var actual = await Collect(new TakeWhileTransformer<int>(IsNonNegative).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task SkipWhile_is_equivalent_to_Linq_SkipWhile() =>
        await IntLists.SampleAsync(async list =>
        {
            var expected = list.SkipWhile(IsNonNegative).ToList();
            var actual = await Collect(new SkipWhileTransformer<int>(IsNonNegative).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual);
        });

    [Fact]
    public async Task Chunk_is_equivalent_to_Linq_Chunk() =>
        await ListAndSize().SampleAsync(async pair =>
        {
            var (list, size) = pair;
            var expected = list.Chunk(size).Select(chunk => chunk.ToList()).ToList();
            var actual = await Collect(new ChunkTransformer<int>(size).TransformAsync(ToAsync(list)));
            Assert.Equal(expected, actual.Select(chunk => chunk.ToList()).ToList());
        });

    [Fact]
    public async Task Buffered_preserves_the_sequence() =>
        await ListAndCapacity().SampleAsync(async pair =>
        {
            var (list, capacity) = pair;
            var actual = await Collect(new BufferedTransformer<int>(capacity).TransformAsync(ToAsync(list)));
            Assert.Equal(list, actual);
        });

    [Fact]
    public async Task PassThrough_is_the_identity() =>
        await IntLists.SampleAsync(async list =>
        {
            var actual = await Collect(new PassThroughTransformer<int>().TransformAsync(ToAsync(list)));
            Assert.Equal(list, actual);
        });

    private static Gen<(List<int> List, int Count)> ListAndCount() =>
        from list in IntLists
        from n in Gen.Int[0, 60]
        select (list, n);

    private static Gen<(List<int> List, int Size)> ListAndSize() =>
        from list in IntLists
        from size in Gen.Int[1, 10]
        select (list, size);

    private static Gen<(List<int> List, int Capacity)> ListAndCapacity() =>
        from list in IntLists
        from capacity in Gen.Int[1, 10]
        select (list, capacity);

    private static bool IsEven(int x) => x % 2 == 0;

    private static bool IsNonNegative(int x) => x >= 0;

    private static long Times10(int x) => x * 10L;

    private static int Mod3(int x) => ((x % 3) + 3) % 3;

    private static IEnumerable<int> PlusMinus(int x) => new[] { x, -x };

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
