using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class SelectTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_with_sync_selector_when_selector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectTransformer<int, string>((Func<int, string>)null!)
        );

        Assert.Equal("selector", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_async_selector_when_selector_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new SelectTransformer<int, string>((Func<int, ValueTask<string>>)null!)
        );

        Assert.Equal("selector", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        Func<int, string> selector = i => i.ToString(CultureInfo.InvariantCulture);
        var sut = new SelectTransformer<int, string>(selector);

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- sync selector ----------

    [Fact]
    public async Task TransformAsync_sync_selector_projects_each_item()
    {
        Func<int, string> selector = i => i.ToString(CultureInfo.InvariantCulture);
        var sut = new SelectTransformer<int, string>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "1", "2", "3" }, result);
    }



    // ---------- async selector ----------

    [Fact]
    public async Task TransformAsync_async_selector_projects_each_item()
    {
        Func<int, ValueTask<string>> selector =
            i => new ValueTask<string>(i.ToString(CultureInfo.InvariantCulture));
        var sut = new SelectTransformer<int, string>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "1", "2", "3" }, result);
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items_sync_selector()
    {
        Func<int, string> selector = i => i.ToString(CultureInfo.InvariantCulture);
        var sut = new SelectTransformer<int, string>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items_async_selector()
    {
        Func<int, ValueTask<string>> selector =
            i => new ValueTask<string>(i.ToString(CultureInfo.InvariantCulture));
        var sut = new SelectTransformer<int, string>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- exception propagation ----------

    [Fact]
    public async Task TransformAsync_when_sync_selector_throws_exception_propagates()
    {
        Func<int, int> selector = _ => throw new InvalidOperationException("boom");
        var sut = new SelectTransformer<int, int>(selector);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("boom", ex.Message);
    }



    [Fact]
    public async Task TransformAsync_when_async_selector_throws_exception_propagates()
    {
        Func<int, ValueTask<int>> selector = _ => throw new InvalidOperationException("async-boom");
        var sut = new SelectTransformer<int, int>(selector);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("async-boom", ex.Message);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_preserves_order_of_projected_items()
    {
        Func<int, int> selector = i => i * 10;
        var sut = new SelectTransformer<int, int>(selector);
        var source = Enumerable.Range(1, 100).ToArray();

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source.Select(i => i * 10), result);
    }



    // ---------- cross-type projection ----------

    [Fact]
    public async Task TransformAsync_supports_changing_type_from_source_to_destination()
    {
        Func<int, string> selector = i => $"value={i}";
        var sut = new SelectTransformer<int, string>(selector);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 10, 20 })));

        Assert.Equal(new[] { "value=10", "value=20" }, result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void SelectTransformer_implements_ITransformAsync()
    {
        Func<int, int> selector = i => i;
        var sut = new SelectTransformer<int, int>(selector);

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
