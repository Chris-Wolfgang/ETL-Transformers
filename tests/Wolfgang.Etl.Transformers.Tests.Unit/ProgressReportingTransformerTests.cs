using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class ProgressReportingTransformerTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_with_sync_callback_when_callback_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new ProgressReportingTransformer<int>((Action<int>)null!)
        );

        Assert.Equal("callback", ex.ParamName);
    }



    [Fact]
    public void Ctor_with_async_callback_when_callback_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new ProgressReportingTransformer<int>((Func<int, ValueTask>)null!)
        );

        Assert.Equal("callback", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new ProgressReportingTransformer<int>(_ => { });

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- sync callback — pass-through ----------

    [Fact]
    public async Task TransformAsync_sync_callback_yields_all_items_unchanged()
    {
        var sut = new ProgressReportingTransformer<int>(_ => { });
        var source = new[] { 10, 20, 30 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_sync_callback_preserves_order()
    {
        var sut = new ProgressReportingTransformer<int>(_ => { });
        var source = new[] { 3, 1, 4, 1, 5, 9, 2, 6 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_sync_callback_when_source_is_empty_yields_no_items()
    {
        var sut = new ProgressReportingTransformer<int>(_ => { });

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- sync callback — callback invocations ----------

    [Fact]
    public async Task TransformAsync_sync_callback_is_called_once_per_item()
    {
        var seen = new List<int>();
        var sut  = new ProgressReportingTransformer<int>(i => seen.Add(i));
        var source = new[] { 1, 2, 3, 4, 5 };

        await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, seen);
    }



    [Fact]
    public async Task TransformAsync_sync_callback_not_called_when_source_is_empty()
    {
        var callCount = 0;
        var sut = new ProgressReportingTransformer<int>(_ => callCount++);

        await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Equal(0, callCount);
    }



    [Fact]
    public async Task TransformAsync_sync_callback_receives_items_in_source_order()
    {
        var received = new List<int>();
        var sut      = new ProgressReportingTransformer<int>(i => received.Add(i));
        var source   = new[] { 100, 200, 300 };

        await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, received);
    }



    [Fact]
    public async Task TransformAsync_sync_callback_can_be_used_to_count_items()
    {
        var count = 0;
        var sut   = new ProgressReportingTransformer<int>(_ => count++);
        var source = new[] { 1, 2, 3, 4, 5, 6, 7 };

        await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(7, count);
    }



    // ---------- async callback — pass-through ----------

    [Fact]
    public async Task TransformAsync_async_callback_yields_all_items_unchanged()
    {
        var sut    = new ProgressReportingTransformer<int>(_ => new ValueTask());
        var source = new[] { 10, 20, 30 };

        var result = await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, result);
    }



    [Fact]
    public async Task TransformAsync_async_callback_when_source_is_empty_yields_no_items()
    {
        var sut = new ProgressReportingTransformer<int>(_ => new ValueTask());

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- async callback — callback invocations ----------

    [Fact]
    public async Task TransformAsync_async_callback_is_called_once_per_item()
    {
        var seen = new List<int>();
        var sut  = new ProgressReportingTransformer<int>(i =>
        {
            seen.Add(i);
            return new ValueTask();
        });
        var source = new[] { 1, 2, 3 };

        await CollectAsync(sut.TransformAsync(ToAsync(source)));

        Assert.Equal(source, seen);
    }



    // ---------- exception propagation ----------

    [Fact]
    public async Task TransformAsync_when_sync_callback_throws_exception_propagates()
    {
        Action<int> callback = _ => throw new InvalidOperationException("boom");
        var sut = new ProgressReportingTransformer<int>(callback);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("boom", ex.Message);
    }



    [Fact]
    public async Task TransformAsync_when_async_callback_throws_exception_propagates()
    {
        Func<int, ValueTask> callback = _ => throw new InvalidOperationException("async-boom");
        var sut = new ProgressReportingTransformer<int>(callback);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 })))
        );

        Assert.Equal("async-boom", ex.Message);
    }



    // ---------- reference identity ----------

    [Fact]
    public async Task TransformAsync_preserves_reference_identity_of_yielded_items()
    {
        var a = new Box(1);
        var b = new Box(2);

        var sut = new ProgressReportingTransformer<Box>(_ => { });

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { a, b })));

        Assert.Collection
        (
            result,
            item => Assert.Same(a, item),
            item => Assert.Same(b, item)
        );
    }



    // ---------- interface sanity ----------

    [Fact]
    public void ProgressReportingTransformer_implements_ITransformAsync()
    {
        var sut = new ProgressReportingTransformer<int>(_ => { });

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



    // ---------- composability ----------

    [Fact]
    public async Task ProgressReportingTransformer_composes_with_Then()
    {
        var seen = new List<int>();
        ITransformAsync<int, int> reporter  = new ProgressReportingTransformer<int>(i => seen.Add(i));
        ITransformAsync<int, int> doubleIt  = new SelectTransformer<int, int>(i => i * 2);

        // reporter fires on the raw value; doubleIt fires on the doubled value
        var pipeline = reporter.Then(doubleIt);

        var result = await CollectAsync(pipeline.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 2, 4, 6 }, result);
        Assert.Equal(new[] { 1, 2, 3 }, seen);   // raw values, not doubled
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



    private sealed class Box
    {
        public Box(int value)
        {
            Value = value;
        }



        public int Value { get; }
    }
}
