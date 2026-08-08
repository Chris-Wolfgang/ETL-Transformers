using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wolfgang.Etl.Transformers.ShadowWorkloads;

/// <summary>
/// Async sequence shapes that model realistic production traffic for the shadow workloads.
/// </summary>
internal static class Sources
{
    /// <summary>A synchronously-completing source of <paramref name="count"/> integers.</summary>
    public static async IAsyncEnumerable<int> Range(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return i;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }


    /// <summary>
    /// A mostly-synchronous source that forces a real asynchronous hop every
    /// <paramref name="asyncEvery"/> items — modelling a source that buffers locally but
    /// periodically waits on I/O (paging, retry/backoff).
    /// </summary>
    public static async IAsyncEnumerable<int> Jittered(int count, int asyncEvery)
    {
        for (var i = 0; i < count; i++)
        {
            if (asyncEvery > 0 && i % asyncEvery == 0)
            {
                await Task.Yield();
            }

            yield return i;
        }
    }
}
