using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// Buffer sizing and progress-counter behaviour of <see cref="ChunkTransformer{T}"/>.
/// </summary>
public class ChunkTransformerBufferTests
{
    private sealed class RecordingProgress : IProgress<int>
    {
        public List<int> Reports { get; } = new();

        public void Report(int value) => Reports.Add(value);
    }



    // Completes synchronously, so allocations stay on the measuring thread.
    private static async IAsyncEnumerable<int> SyncSource(int count)
    {
        await Task.CompletedTask;
        for (var i = 0; i < count; i++)
        {
            yield return i;
        }
    }



    // Enumerable.Chunk does not exist before .NET 6, so the reference result is built by hand.
    private static List<List<int>> ReferenceChunks(int[] source, int size)
    {
        var chunks = new List<List<int>>();
        for (var i = 0; i < source.Length; i += size)
        {
            chunks.Add(source.Skip(i).Take(size).ToList());
        }

        return chunks;
    }



    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 5)]
    [InlineData(5, 5)]
    [InlineData(10, 5)]
    [InlineData(11, 5)]
    [InlineData(40, 100)]
    [InlineData(100, 100)]
    [InlineData(101, 100)]
    [InlineData(20, 20)]
    [InlineData(21, 20)]
    [InlineData(50, 17)]
    [InlineData(100, 40)]
    [InlineData(1000, 1)]
    public async Task TransformAsync_chunks_match_Enumerable_Chunk(int count, int size)
    {
        var source = Enumerable.Range(0, count).ToArray();
        var sut = new ChunkTransformer<int>(size);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(ReferenceChunks(source, size), result.Select(c => c.ToList()).ToList());
        Assert.All(result.Take(Math.Max(0, result.Count - 1)), c => Assert.Equal(size, c.Count));
    }



#if NET8_0_OR_GREATER
    [Fact]
    public async Task TransformAsync_over_an_empty_source_does_not_allocate_a_chunk_sized_buffer()
    {
        var sut = new ChunkTransformer<long>(50_000_000);
        var before = GC.GetAllocatedBytesForCurrentThread();

        var result = await CollectAsync(sut.TransformAsync(EmptyLongs()));

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Empty(result.SelectMany(c => c));
        Assert.True(allocated < 1_000_000, $"allocated {allocated} bytes");
    }



    [Fact]
    public async Task TransformAsync_over_a_short_source_with_a_large_size_allocates_only_for_the_items_present()
    {
        var sut = new ChunkTransformer<int>(5_000_000);
        var before = GC.GetAllocatedBytesForCurrentThread();

        var result = await CollectAsync(sut.TransformAsync(SyncSource(10)));

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Single(result);
        Assert.Equal(10, result[0].Count);
        Assert.True(allocated < 1_000_000, $"allocated {allocated} bytes");
    }
#endif



    private static async IAsyncEnumerable<long> EmptyLongs()
    {
        await Task.CompletedTask;
        yield break;
    }



    [Fact]
    public async Task ChunkAsync_progress_holds_at_Int32_MaxValue_instead_of_wrapping_negative()
    {
        var progress = new RecordingProgress();

        _ = await CollectAsync
        (
            ChunkTransformer<int>.ChunkAsync(SyncSource(3), size: 1, progress, startCount: int.MaxValue - 1L)
        );

        Assert.Equal(new[] { int.MaxValue, int.MaxValue, int.MaxValue }, progress.Reports);
    }



    [Fact]
    public async Task ChunkAsync_progress_reports_the_running_count_from_zero_by_default()
    {
        var progress = new RecordingProgress();

        _ = await CollectAsync(ChunkTransformer<int>.ChunkAsync(SyncSource(5), size: 2, progress));

        Assert.Equal(new[] { 2, 4, 5 }, progress.Reports);
    }
}
