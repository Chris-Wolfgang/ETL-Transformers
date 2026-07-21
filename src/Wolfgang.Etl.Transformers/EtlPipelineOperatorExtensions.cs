using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// LINQ-flavored operator extension methods on <see cref="IEtlPipeline{T}"/>. Each operator is a
/// thin wrapper that appends one of this package's transformers to the pipeline via the core's
/// <see cref="IEtlPipeline{T}.Through{TDestination}(ITransformAsync{T, TDestination})"/> primitive —
/// no iteration logic is re-implemented here.
/// </summary>
/// <remarks>
/// <para>
/// The pipeline core in <c>Wolfgang.Etl.Abstractions</c> is deliberately minimal and does not depend
/// on this package, so the operator surface lives here. Add a <c>using Wolfgang.Etl.Transformers;</c>
/// to light these up between the source (<c>From(...)</c>) and sink (<c>To(...)</c>) stages of a
/// pipeline:
/// </para>
/// <code>
///     await EtlPipeline
///         .Create()
///         .From(records)
///         .Where(r =&gt; r.Amount &gt; 0)
///         .Select(r =&gt; r.Id)
///         .To(loader)
///         .RunAsync();
/// </code>
/// </remarks>
public static class EtlPipelineOperatorExtensions
{
    /// <summary>Filters the pipeline to items that satisfy a synchronous <paramref name="predicate"/>.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to filter.</param>
    /// <param name="predicate">The predicate an item must satisfy to be kept.</param>
    /// <returns>A pipeline yielding only the items for which <paramref name="predicate"/> returns <see langword="true"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Where<T>(this IEtlPipeline<T> pipeline, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(predicate, nameof(predicate));

        return pipeline.Through(new WhereTransformer<T>(predicate));
    }



    /// <summary>Filters the pipeline to items that satisfy an asynchronous <paramref name="predicate"/>.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to filter.</param>
    /// <param name="predicate">The asynchronous predicate an item must satisfy to be kept.</param>
    /// <returns>A pipeline yielding only the items for which <paramref name="predicate"/> returns <see langword="true"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Where<T>(this IEtlPipeline<T> pipeline, Func<T, ValueTask<bool>> predicate)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(predicate, nameof(predicate));

        return pipeline.Through(new WhereTransformer<T>(predicate));
    }



    /// <summary>Projects each item with a synchronous <paramref name="selector"/>.</summary>
    /// <typeparam name="TSource">The input item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The projected item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to project.</param>
    /// <param name="selector">The projection applied to each item.</param>
    /// <returns>A pipeline of the projected items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="selector"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> Select<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, TDestination> selector)
        where TSource : notnull
        where TDestination : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(selector, nameof(selector));

        return pipeline.Through(new SelectTransformer<TSource, TDestination>(selector));
    }



    /// <summary>Projects each item with an asynchronous <paramref name="selector"/>.</summary>
    /// <typeparam name="TSource">The input item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The projected item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to project.</param>
    /// <param name="selector">The asynchronous projection applied to each item.</param>
    /// <returns>A pipeline of the projected items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="selector"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> Select<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, ValueTask<TDestination>> selector)
        where TSource : notnull
        where TDestination : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(selector, nameof(selector));

        return pipeline.Through(new SelectTransformer<TSource, TDestination>(selector));
    }



    /// <summary>Fans each item out to a synchronous sequence and flattens the results.</summary>
    /// <typeparam name="TSource">The input item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The flattened item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to fan out.</param>
    /// <param name="selector">Maps each item to a sequence of results.</param>
    /// <returns>A pipeline of the flattened items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="selector"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> SelectMany<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, IEnumerable<TDestination>> selector)
        where TSource : notnull
        where TDestination : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(selector, nameof(selector));

        return pipeline.Through(new SelectManyTransformer<TSource, TDestination>(selector));
    }



    /// <summary>Fans each item out to an asynchronous sequence and flattens the results.</summary>
    /// <typeparam name="TSource">The input item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The flattened item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to fan out.</param>
    /// <param name="selector">Maps each item to an asynchronous sequence of results.</param>
    /// <returns>A pipeline of the flattened items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="selector"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> SelectMany<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, IAsyncEnumerable<TDestination>> selector)
        where TSource : notnull
        where TDestination : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(selector, nameof(selector));

        return pipeline.Through(new SelectManyTransformer<TSource, TDestination>(selector));
    }



    /// <summary>Removes duplicate items, optionally using a supplied <paramref name="comparer"/>.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to deduplicate.</param>
    /// <param name="comparer">
    /// The comparer used to determine equality, or <see langword="null"/> to use
    /// <see cref="EqualityComparer{T}.Default"/>.
    /// </param>
    /// <returns>A pipeline with duplicate items removed, preserving first-seen order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Distinct<T>(this IEtlPipeline<T> pipeline, IEqualityComparer<T>? comparer = null)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));

        return pipeline.Through(new DistinctTransformer<T>(comparer));
    }



    /// <summary>Removes items with duplicate keys, optionally using a supplied key <paramref name="comparer"/>.</summary>
    /// <typeparam name="TSource">The item type. Must be non-null.</typeparam>
    /// <typeparam name="TKey">The key type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to deduplicate.</param>
    /// <param name="keySelector">Extracts the key each item is deduplicated by.</param>
    /// <param name="comparer">
    /// The comparer used to determine key equality, or <see langword="null"/> to use
    /// <see cref="EqualityComparer{TKey}.Default"/>.
    /// </param>
    /// <returns>A pipeline keeping the first item seen for each distinct key.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TSource> DistinctBy<TSource, TKey>(this IEtlPipeline<TSource> pipeline, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
        where TSource : notnull
        where TKey : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(keySelector, nameof(keySelector));

        return pipeline.Through(new DistinctByTransformer<TSource, TKey>(keySelector, comparer));
    }



    /// <summary>Yields only the first <paramref name="count"/> items, then stops.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to truncate.</param>
    /// <param name="count">The maximum number of items to yield. Must be non-negative.</param>
    /// <returns>A pipeline yielding at most <paramref name="count"/> items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Take<T>(this IEtlPipeline<T> pipeline, int count)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));

        return pipeline.Through(new TakeTransformer<T>(count));
    }



    /// <summary>Skips the first <paramref name="count"/> items, then yields the rest.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to skip within.</param>
    /// <param name="count">The number of leading items to skip. Must be non-negative.</param>
    /// <returns>A pipeline yielding the items after the first <paramref name="count"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Skip<T>(this IEtlPipeline<T> pipeline, int count)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));

        return pipeline.Through(new SkipTransformer<T>(count));
    }



    /// <summary>Yields items while a synchronous <paramref name="predicate"/> holds, then stops.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to truncate.</param>
    /// <param name="predicate">Yielding continues while this returns <see langword="true"/>.</param>
    /// <returns>A pipeline yielding the leading run of items satisfying <paramref name="predicate"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> TakeWhile<T>(this IEtlPipeline<T> pipeline, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(predicate, nameof(predicate));

        return pipeline.Through(new TakeWhileTransformer<T>(predicate));
    }



    /// <summary>Yields items while an asynchronous <paramref name="predicate"/> holds, then stops.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to truncate.</param>
    /// <param name="predicate">Yielding continues while this returns <see langword="true"/>.</param>
    /// <returns>A pipeline yielding the leading run of items satisfying <paramref name="predicate"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> TakeWhile<T>(this IEtlPipeline<T> pipeline, Func<T, ValueTask<bool>> predicate)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(predicate, nameof(predicate));

        return pipeline.Through(new TakeWhileTransformer<T>(predicate));
    }



    /// <summary>Skips items while a synchronous <paramref name="predicate"/> holds, then yields the rest.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to skip within.</param>
    /// <param name="predicate">Skipping continues while this returns <see langword="true"/>.</param>
    /// <returns>A pipeline yielding everything from the first item that fails <paramref name="predicate"/> onward.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> SkipWhile<T>(this IEtlPipeline<T> pipeline, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(predicate, nameof(predicate));

        return pipeline.Through(new SkipWhileTransformer<T>(predicate));
    }



    /// <summary>Skips items while an asynchronous <paramref name="predicate"/> holds, then yields the rest.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to skip within.</param>
    /// <param name="predicate">Skipping continues while this returns <see langword="true"/>.</param>
    /// <returns>A pipeline yielding everything from the first item that fails <paramref name="predicate"/> onward.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> SkipWhile<T>(this IEtlPipeline<T> pipeline, Func<T, ValueTask<bool>> predicate)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));
        ThrowIfNull(predicate, nameof(predicate));

        return pipeline.Through(new SkipWhileTransformer<T>(predicate));
    }



    /// <summary>Batches items into fixed-size chunks.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to batch.</param>
    /// <param name="size">The number of items per chunk. Must be at least 1.</param>
    /// <returns>A pipeline of chunks; the final chunk may hold fewer than <paramref name="size"/> items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<IReadOnlyList<T>> Chunk<T>(this IEtlPipeline<T> pipeline, int size)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));

        return pipeline.Through(new ChunkTransformer<T>(size));
    }



    /// <summary>
    /// Inserts a decoupling buffer so the upstream producer and downstream consumer run concurrently.
    /// The item type is unchanged; this batches nothing.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to buffer.</param>
    /// <param name="capacity">The maximum number of items held in the buffer. Must be at least 1.</param>
    /// <returns>A pipeline of the same items, produced concurrently with consumption.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Buffered<T>(this IEtlPipeline<T> pipeline, int capacity)
        where T : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));

        return pipeline.Through(new BufferedTransformer<T>(capacity));
    }



    /// <summary>Casts each item to <typeparamref name="TDestination"/>, throwing on an incompatible item.</summary>
    /// <typeparam name="TSource">The input item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The target type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to cast.</param>
    /// <returns>A pipeline of the cast items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> Cast<TSource, TDestination>(this IEtlPipeline<TSource> pipeline)
        where TSource : notnull
        where TDestination : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));

        return pipeline.Through(new CastTransformer<TSource, TDestination>());
    }



    /// <summary>Passes through only the items assignable to <typeparamref name="TDestination"/>.</summary>
    /// <typeparam name="TSource">The input item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The type to filter to. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to filter by type.</param>
    /// <returns>A pipeline of the items that are of type <typeparamref name="TDestination"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> OfType<TSource, TDestination>(this IEtlPipeline<TSource> pipeline)
        where TSource : notnull
        where TDestination : notnull
    {
        ThrowIfNull(pipeline, nameof(pipeline));

        return pipeline.Through(new OfTypeTransformer<TSource, TDestination>());
    }



    private static void ThrowIfNull(object? argument, string paramName)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
