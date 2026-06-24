using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// A transformer that batches the input sequence into fixed-size groups and yields each batch as
/// a single <see cref="IReadOnlyList{T}"/> output item.
/// </summary>
/// <typeparam name="T">The type of items flowing through the transformer. Must be non-null.</typeparam>
/// <remarks>
/// <para>
/// <see cref="ChunkTransformer{T}"/> is the transformer equivalent of LINQ's
/// <c>Enumerable.Chunk</c> (introduced in .NET 6): it groups consecutive input items into
/// <see cref="Size"/>-element arrays. The last chunk may be smaller than <see cref="Size"/>
/// if the input length is not an exact multiple of <see cref="Size"/>.
/// </para>
/// <para>
/// Each yielded chunk is backed by a freshly-allocated <typeparamref name="T"/>[] (exposed as
/// <see cref="IReadOnlyList{T}"/>) - chunks do not share backing storage with the transformer or
/// with each other, so consumers can retain each chunk without affecting subsequent output.
/// </para>
/// <para>
/// Useful for batching writes to a downstream loader (e.g. <c>SqlBulkCopy</c>) without
/// materializing the entire stream.
/// </para>
/// <para>
/// Implements only <see cref="ITransformAsync{TSource, TDestination}"/> - no progress, no
/// cancellation, no Skip/Max - to keep the hot loop minimal.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // batch rows into groups of 1000 for bulk loading
///     var batches = new ChunkTransformer&lt;Row&gt;(size: 1000);
/// </code>
/// </example>
public sealed class ChunkTransformer<T> : ITransformAsync<T, IReadOnlyList<T>>
    where T : notnull
{
    private readonly int _size;



    /// <summary>
    /// Initializes a new instance with the given chunk size.
    /// </summary>
    /// <param name="size">The maximum number of items in each yielded chunk. Must be at least 1.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is less than 1.</exception>
    public ChunkTransformer(int size)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
#else
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Chunk size must be greater than 0.");
        }
#endif

        _size = size;
    }



    /// <summary>
    /// Asynchronously yields successive chunks of <see cref="Size"/> consecutive items from
    /// <paramref name="items"/>. The last chunk may contain fewer items if the source length
    /// is not a multiple of <see cref="Size"/>.
    /// </summary>
    /// <param name="items">The asynchronous source sequence.</param>
    /// <returns>An asynchronous sequence of read-only lists, each containing up to <see cref="Size"/> items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public IAsyncEnumerable<IReadOnlyList<T>> TransformAsync(IAsyncEnumerable<T> items)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(items);
#else
#pragma warning disable RCS1140 // Roslynator does not associate throw inside #else block with method XML doc
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
#pragma warning restore RCS1140
#endif

        return ChunkAsync(items, _size);
    }



    /// <summary>
    /// The maximum number of items in each yielded chunk.
    /// </summary>
    public int Size => _size;



    private static async IAsyncEnumerable<IReadOnlyList<T>> ChunkAsync(IAsyncEnumerable<T> items, int size)
    {
        var buffer = new T[size];
        var index = 0;

        await foreach (var item in items.ConfigureAwait(continueOnCapturedContext: false))
        {
            buffer[index++] = item;

            if (index == size)
            {
                yield return buffer;
                buffer = new T[size];
                index = 0;
            }
        }

        if (index > 0)
        {
            // partial final chunk - copy to a right-sized array so the consumer
            // sees only the items that were actually present
            var partial = new T[index];
            Array.Copy(buffer, partial, index);
            yield return partial;
        }
    }
}
