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
}
