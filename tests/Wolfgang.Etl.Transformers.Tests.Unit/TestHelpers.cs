using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Wolfgang.Etl.Transformers.Tests.Unit;

internal static class TestHelpers
{
    internal static async IAsyncEnumerable<T> ToAsync<T>(
        IEnumerable<T> items,
        [EnumeratorCancellation] System.Threading.CancellationToken token = default)
    {
        foreach (var item in items)
        {
            token.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return item;
        }
    }



    internal static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> items)
    {
        var list = new List<T>();
        await foreach (var item in items)
        {
            list.Add(item);
        }
        return list;
    }



    // Async source that increments PullCount for every element MoveNextAsync actually reaches.
    // Used by the pre-cancelled-token regression tests (#209) to assert that the transformer
    // observes cancellation before pulling anything from the source.
    internal sealed class CountingSource
    {
        private readonly int _itemCount;


        public CountingSource(int itemCount)
        {
            _itemCount = itemCount;
        }


        public int PullCount { get; private set; }


        public async IAsyncEnumerable<int> Enumerate()
        {
            for (var i = 0; i < _itemCount; i++)
            {
                PullCount++;
                yield return i;
                await Task.Yield();
            }
        }
    }
}
