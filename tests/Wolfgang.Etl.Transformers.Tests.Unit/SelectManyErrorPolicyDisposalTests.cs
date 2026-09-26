using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// The corner cases of <see cref="SelectManyTransformer{TSource, TDestination}"/>'s error policy:
/// a selector that throws before it returns a sequence at all, and a returned sequence whose
/// disposal throws.
/// </summary>
public class SelectManyErrorPolicyDisposalTests
{
    private static DelegateTransformerOptions SkipAll() =>
        new() { ErrorPolicy = _ => ItemErrorAction.Skip };



    [Fact]
    public async Task Async_selector_that_throws_before_returning_the_sequence_is_skipped()
    {
        // Distinct from a throw during enumeration: this lambda is not an iterator, so it throws
        // when it is called rather than on the first MoveNextAsync.
        var sut = new SelectManyTransformer<int, string>
        (
            (Func<int, IAsyncEnumerable<string>>)(i =>
                i == 2 ? throw new InvalidOperationException() : ToAsync(new[] { $"v{i}" })),
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1", "v3" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task Sync_selector_that_throws_before_returning_the_sequence_is_skipped()
    {
        var sut = new SelectManyTransformer<int, string>
        (
            (Func<int, IEnumerable<string>>)(i =>
                i == 2 ? throw new InvalidOperationException() : new[] { $"v{i}" }),
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1", "v3" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task Disposal_failure_after_a_skipped_item_does_not_end_the_run()
    {
        // The item already failed and was counted; a cleanup error must not defeat the Skip.
        var sut = new SelectManyTransformer<int, string>
        (
            i => new ThrowingSequence(i, throwOnMoveNext: i == 2, throwOnDispose: i == 2),
            SkipAll()
        );

        var result = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        Assert.Equal(new[] { "v1", "v3" }, result);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task Disposal_failure_on_a_healthy_item_still_propagates()
    {
        // Nothing was handled for this item, so a cleanup error is not an item-level data error
        // and is not silently swallowed.
        var sut = new SelectManyTransformer<int, string>
        (
            i => new ThrowingSequence(i, throwOnMoveNext: false, throwOnDispose: i == 2),
            SkipAll()
        );

        await Assert.ThrowsAsync<ObjectDisposedException>
        (
            async () => await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })))
        );

        Assert.Equal(0, sut.CurrentErrorItemCount);
    }



    [Fact]
    public async Task Disposal_failure_does_not_count_the_item_a_second_time()
    {
        var policyCalls = 0;
        var options = new DelegateTransformerOptions
        {
            ErrorPolicy = _ =>
            {
                policyCalls++;
                return ItemErrorAction.Skip;
            }
        };
        var sut = new SelectManyTransformer<int, string>
        (
            i => new ThrowingSequence(i, throwOnMoveNext: i == 2, throwOnDispose: i == 2),
            options
        );

        _ = await CollectAsync(sut.TransformAsync(ToAsync(new[] { 1, 2, 3 })));

        // One failure, one policy consultation, one count - the disposal error is not re-reported.
        Assert.Equal(1, policyCalls);
        Assert.Equal(1, sut.CurrentErrorItemCount);
    }



    [Fact]
    public void ThrowingSequence_non_generic_members_behave_like_the_generic_ones()
    {
        // The transformer only uses the generic surface; this keeps the helper's non-generic
        // interface members honest rather than leaving them unexercised.
        IEnumerable sequence = new ThrowingSequence(7, throwOnMoveNext: false, throwOnDispose: false);
        var enumerator = sequence.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("v7", enumerator.Current);
        Assert.Throws<NotSupportedException>(() => enumerator.Reset());
    }



    /// <summary>
    /// A sequence whose enumerator can be told to throw from MoveNext, from Dispose, or both.
    /// </summary>
    private sealed class ThrowingSequence : IEnumerable<string>
    {
        private readonly int _value;
        private readonly bool _throwOnMoveNext;
        private readonly bool _throwOnDispose;


        public ThrowingSequence(int value, bool throwOnMoveNext, bool throwOnDispose)
        {
            _value = value;
            _throwOnMoveNext = throwOnMoveNext;
            _throwOnDispose = throwOnDispose;
        }


        public IEnumerator<string> GetEnumerator() =>
            new ThrowingEnumerator(_value, _throwOnMoveNext, _throwOnDispose);


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }



    private sealed class ThrowingEnumerator : IEnumerator<string>
    {
        private readonly int _value;
        private readonly bool _throwOnMoveNext;
        private readonly bool _throwOnDispose;
        private bool _moved;


        public ThrowingEnumerator(int value, bool throwOnMoveNext, bool throwOnDispose)
        {
            _value = value;
            _throwOnMoveNext = throwOnMoveNext;
            _throwOnDispose = throwOnDispose;
        }


        public string Current { get; private set; } = string.Empty;


        object IEnumerator.Current => Current;


        public bool MoveNext()
        {
            if (_throwOnMoveNext)
            {
                throw new InvalidOperationException();
            }

            if (_moved)
            {
                return false;
            }

            _moved = true;
            Current = $"v{_value}";
            return true;
        }


        public void Reset() => throw new NotSupportedException();


        public void Dispose()
        {
            if (_throwOnDispose)
            {
                throw new ObjectDisposedException(nameof(ThrowingEnumerator));
            }
        }
    }
}
