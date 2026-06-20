using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class SelectManyTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_with_sync_selector_when_selector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectManyTransformer<int, int>((Func<int, IEnumerable<int>>)null!)
        );

        Assert.Equal("selector", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_async_selector_when_selector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectManyTransformer<int, int>((Func<int, IAsyncEnumerable<int>>)null!)
        );

        Assert.Equal("selector", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        Func<int, IEnumerable<int>> selector = i => new[] { i };
        var sut = new SelectManyTransformer<int, int>(selector);

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- sync selector ----------

    [Fact]
    public async Task TransformAsync_sync_selector_flattens_inner_sequences()
    {
        Func<int, IEnumerable<int>> selector = i => new[] { i, i * 10 };
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 10, 2, 20, 3, 30 }, result);
    }



    [Fact]
    public async Task TransformAsync_sync_selector_yields_nothing_for_empty_inner()
    {
        Func<int, IEnumerable<int>> selector = i => i % 2 == 0 ? new[] { i } : Array.Empty<int>();
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(new[] { 2, 4 }, result);
    }



    [Fact]
    public async Task TransformAsync_sync_selector_with_all_empty_inners_yields_empty_sequence()
    {
        Func<int, IEnumerable<int>> selector = _ => Array.Empty<int>();
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Empty(result);
    }



    // ---------- async selector ----------

    [Fact]
    public async Task TransformAsync_async_selector_flattens_inner_sequences()
    {
        Func<int, IAsyncEnumerable<int>> selector = i => ToAsync(new[] { i, i * 10 });
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 10, 2, 20, 3, 30 }, result);
    }



    [Fact]
    public async Task TransformAsync_async_selector_yields_nothing_for_empty_inner()
    {
        Func<int, IAsyncEnumerable<int>> selector =
            i => i % 2 == 0 ? ToAsync(new[] { i }) : ToAsync(Array.Empty<int>());
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3, 4 })));

        Assert.Equal(new[] { 2, 4 }, result);
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items_sync_selector()
    {
        Func<int, IEnumerable<int>> selector = i => new[] { i };
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items_async_selector()
    {
        Func<int, IAsyncEnumerable<int>> selector = i => ToAsync(new[] { i });
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- depth-first ordering ----------

    [Fact]
    public async Task TransformAsync_emits_inner_items_depth_first()
    {
        Func<int, IEnumerable<string>> selector =
            i => new[] { $"{i}-a", $"{i}-b", $"{i}-c" };
        var sut = new SelectManyTransformer<int, string>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2 })));

        Assert.Equal
        (
            new[] { "1-a", "1-b", "1-c", "2-a", "2-b", "2-c" },
            result
        );
    }



    // ---------- exception propagation ----------

    [Fact]
    public async Task TransformAsync_when_sync_selector_throws_exception_propagates()
    {
        Func<int, IEnumerable<int>> selector = _ => throw new InvalidOperationException("boom");
        var sut = new SelectManyTransformer<int, int>(selector);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("boom", ex.Message);
    }



    [Fact]
    public async Task TransformAsync_when_async_selector_throws_exception_propagates()
    {
        Func<int, IAsyncEnumerable<int>> selector =
            _ => throw new InvalidOperationException("async-boom");
        var sut = new SelectManyTransformer<int, int>(selector);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("async-boom", ex.Message);
    }



    [Fact]
    public async Task TransformAsync_when_inner_sync_iteration_throws_exception_propagates()
    {
        Func<int, IEnumerable<int>> selector = _ => ThrowingEnumerable();
        var sut = new SelectManyTransformer<int, int>(selector);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("inner-boom", ex.Message);

        static IEnumerable<int> ThrowingEnumerable()
        {
            yield return 1;
            throw new InvalidOperationException("inner-boom");
        }
    }



    // ---------- cross-type projection ----------

    [Fact]
    public async Task TransformAsync_supports_changing_type_from_source_to_destination()
    {
        Func<string, IEnumerable<char>> selector = s => s.ToCharArray();
        var sut = new SelectManyTransformer<string, char>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { "ab", "cd" })));

        Assert.Equal(new[] { 'a', 'b', 'c', 'd' }, result);
    }



    // ---------- order preservation across many input items ----------

    [Fact]
    public async Task TransformAsync_preserves_global_ordering()
    {
        Func<int, IEnumerable<int>> selector = i => Enumerable.Range(i * 10, 3);
        var sut = new SelectManyTransformer<int, int>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 0, 1, 2 })));

        Assert.Equal(new[] { 0, 1, 2, 10, 11, 12, 20, 21, 22 }, result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void SelectManyTransformer_implements_ITransformAsync()
    {
        Func<int, IEnumerable<int>> selector = i => new[] { i };
        var sut = new SelectManyTransformer<int, int>(selector);

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



    // ---------- helpers ----------

    private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }



    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> items)
    {
        var list = new List<T>();
        await foreach (var item in items)
        {
            list.Add(item);
        }
        return list;
    }
}
