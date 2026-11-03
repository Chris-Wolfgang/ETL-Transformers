using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;



namespace Wolfgang.Etl.Transformers.Tests.Shared;

/// <summary>
/// Async-sequence helpers shared by the Integration, Fuzz and Snapshots test projects. Each project
/// compiles this file in through a linked <c>&lt;Compile Include="..\Shared\AsyncSequence.cs" /&gt;</c>,
/// so the three copies cannot drift apart.
/// </summary>
internal static class AsyncSequence
{
    /// <summary>
    /// Yields <paramref name="items"/> as an asynchronous sequence, yielding the thread before each item.
    /// </summary>
    internal static async IAsyncEnumerable<T> ToAsync<T>
    (
        IEnumerable<T> items,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        foreach (var item in items)
        {
            token.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return item;
        }
    }



    /// <summary>
    /// Drains <paramref name="items"/> into a list.
    /// </summary>
    internal static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> items)
    {
        var list = new List<T>();
        await foreach (var item in items.ConfigureAwait(false))
        {
            list.Add(item);
        }

        return list;
    }
}
