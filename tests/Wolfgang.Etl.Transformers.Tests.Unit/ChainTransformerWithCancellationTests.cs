using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

public class ChainTransformerWithCancellationTests
{
    // ---------- construction ----------

    [Fact]
    public void Ctor_when_first_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new ChainTransformerWithCancellation<int, int, int>
            (
                null!,
                new PassThroughTransformer<int>()
            )
        );

        Assert.Equal("first", ex.ParamName);
    }



    [Fact]
    public void Ctor_when_second_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => new ChainTransformerWithCancellation<int, int, int>
            (
                new PassThroughTransformer<int>(),
                null!
            )
        );

        Assert.Equal("second", ex.ParamName);
    }



    // ---------- null input ----------

    [Fact]
    public void TransformAsync_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new ChainTransformerWithCancellation<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!)
        );

        Assert.Equal("items", ex.ParamName);
    }



    [Fact]
    public void TransformAsync_with_cancellation_when_items_is_null_throws_ArgumentNullException()
    {
        var sut = new ChainTransformerWithCancellation<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

        var ex = Assert.Throws<ArgumentNullException>
        (
            () => sut.TransformAsync(null!, CancellationToken.None)
        );

        Assert.Equal("items", ex.ParamName);
    }



    // ---------- pass-through fidelity ----------

    [Fact]
    public async Task TransformAsync_no_token_yields_each_item_through_both_stages()
    {
        var first = new RecordingPassThroughTransformer();
        var second = new RecordingPassThroughTransformer();
        var sut = new ChainTransformerWithCancellation<int, int, int>(first, second);

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Equal(3, first.ItemsSeen);
        Assert.Equal(3, second.ItemsSeen);
    }



    [Fact]
    public async Task TransformAsync_with_token_yields_each_item_through_both_stages()
    {
        var first = new RecordingPassThroughTransformer();
        var second = new RecordingPassThroughTransformer();
        var sut = new ChainTransformerWithCancellation<int, int, int>(first, second);

        var result = await CollectAsync
        (
            sut.TransformAsync(ToAsync(new[] { 1, 2, 3 }), CancellationToken.None)
        );

        Assert.Equal(new[] { 1, 2, 3 }, result);
        Assert.Equal(3, first.ItemsSeen);
        Assert.Equal(3, second.ItemsSeen);
    }



    // ---------- token propagates to both stages ----------

    [Fact]
    public async Task TransformAsync_with_token_propagates_token_to_first_stage()
    {
        var first = new TokenCapturingPassThrough();
        var second = new TokenCapturingPassThrough();
        var sut = new ChainTransformerWithCancellation<int, int, int>(first, second);
        using var cts = new CancellationTokenSource();

        await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 }), cts.Token));

        Assert.Equal(cts.Token, first.LastSeenToken);
    }



    [Fact]
    public async Task TransformAsync_with_token_propagates_token_to_second_stage()
    {
        var first = new TokenCapturingPassThrough();
        var second = new TokenCapturingPassThrough();
        var sut = new ChainTransformerWithCancellation<int, int, int>(first, second);
        using var cts = new CancellationTokenSource();

        await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1 }), cts.Token));

        Assert.Equal(cts.Token, second.LastSeenToken);
    }



    [Fact]
    public async Task TransformAsync_with_pre_cancelled_token_throws_OperationCanceledException()
    {
        // Use real PassThroughTransformer which actually observes the token via
        // ThrowIfCancellationRequested - the test fixtures above only capture/forward,
        // so they would not raise on a pre-cancelled token by themselves.
        var sut = new ChainTransformerWithCancellation<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            async () =>
            {
                await foreach (var _ in sut.TransformAsync(ToAsync(new[] { 1, 2, 3 }), cts.Token))
                {
                }
            }
        );
    }



    // ---------- empty source ----------

    [Fact]
    public async Task TransformAsync_when_source_is_empty_yields_no_items()
    {
        var sut = new ChainTransformerWithCancellation<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(Array.Empty<int>())));

        Assert.Empty(result);
    }



    // ---------- order preservation ----------

    [Fact]
    public async Task TransformAsync_with_token_preserves_order()
    {
        var sut = new ChainTransformerWithCancellation<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );
        var source = Enumerable.Range(1, 50).ToArray();

        var result = await CollectAsync
        (
            sut.TransformAsync(ToAsync(source), CancellationToken.None)
        );

        Assert.Equal(source, result);
    }



    // ---------- interface sanity ----------

    [Fact]
    public void ChainTransformerWithCancellation_implements_ITransformWithCancellationAsync()
    {
        var sut = new ChainTransformerWithCancellation<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

        Assert.IsAssignableFrom<ITransformWithCancellationAsync<int, int>>(sut);
    }



    [Fact]
    public void ChainTransformerWithCancellation_also_implements_ITransformAsync()
    {
        var sut = new ChainTransformerWithCancellation<int, int, int>
        (
            new PassThroughTransformer<int>(),
            new PassThroughTransformer<int>()
        );

        Assert.IsAssignableFrom<ITransformAsync<int, int>>(sut);
    }



    // test fixtures

    /// <summary>
    /// Pass-through that records how many items it saw - used to verify both stages run.
    /// </summary>
    private sealed class RecordingPassThroughTransformer : ITransformWithCancellationAsync<int, int>
    {
        public int ItemsSeen { get; private set; }



        public IAsyncEnumerable<int> TransformAsync(IAsyncEnumerable<int> items)
        {
            return Iterate(items, CancellationToken.None);
        }



        public IAsyncEnumerable<int> TransformAsync(IAsyncEnumerable<int> items, CancellationToken token)
        {
            return Iterate(items, token);
        }



        private async IAsyncEnumerable<int> Iterate(IAsyncEnumerable<int> items, [EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
            {
                ItemsSeen++;
                yield return item;
            }
        }
    }



    /// <summary>
    /// Pass-through that captures the cancellation token it was given - used to verify
    /// the chain forwards the token to both stages.
    /// </summary>
    private sealed class TokenCapturingPassThrough : ITransformWithCancellationAsync<int, int>
    {
        public CancellationToken LastSeenToken { get; private set; }



        public IAsyncEnumerable<int> TransformAsync(IAsyncEnumerable<int> items)
        {
            LastSeenToken = CancellationToken.None;
            return Iterate(items, CancellationToken.None);
        }



        public IAsyncEnumerable<int> TransformAsync(IAsyncEnumerable<int> items, CancellationToken token)
        {
            LastSeenToken = token;
            return Iterate(items, token);
        }



        private async IAsyncEnumerable<int> Iterate(IAsyncEnumerable<int> items, [EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var item in items.WithCancellation(token).ConfigureAwait(continueOnCapturedContext: false))
            {
                yield return item;
            }
        }
    }
}
