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
/// <example>
/// <code>
///     await EtlPipeline
///         .Create()
///         .From(records)
///         .Where(r =&gt; r.Amount &gt; 0)
///         .Select(r =&gt; r.Id)
///         .To(loader)
///         .RunAsync();
/// </code>
/// </example>
/// </remarks>
public static class EtlPipelineOperatorExtensions
{
    // Each operator guards only `pipeline`: the transformer constructor it wires in validates the
    // delegate (or key) argument under the same parameter name, so a second guard here would be
    // unobservable dead code. `Log` composes through `Tap`, which guards `pipeline` for it.

    /// <summary>Filters the pipeline to items that satisfy a synchronous <paramref name="predicate"/>.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to filter.</param>
    /// <param name="predicate">The predicate an item must satisfy to be kept.</param>
    /// <returns>A pipeline yielding only the items for which <paramref name="predicate"/> returns <see langword="true"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Where<T>(this IEtlPipeline<T> pipeline, Func<T, bool> predicate)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new WhereTransformer<T>(predicate));
    }



    /// <summary>
    /// Filters the pipeline like <c>Where</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="predicate">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Where<T>(this IEtlPipeline<T> pipeline, Func<T, bool> predicate, DelegateTransformerOptions options)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new WhereTransformer<T>(predicate, options));
    }



    /// <summary>
    /// Filters the pipeline like <c>Where</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="predicate">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Where<T>(this IEtlPipeline<T> pipeline, Func<T, ValueTask<bool>> predicate, DelegateTransformerOptions options)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new WhereTransformer<T>(predicate, options));
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
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SelectTransformer<TSource, TDestination>(selector));
    }



    /// <summary>
    /// Projects the pipeline like <c>Select</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="TSource">The source item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The destination item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="selector">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> Select<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, TDestination> selector, DelegateTransformerOptions options)
        where TSource : notnull
        where TDestination : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SelectTransformer<TSource, TDestination>(selector, options));
    }



    /// <summary>
    /// Projects the pipeline like <c>Select</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="TSource">The source item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The destination item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="selector">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> Select<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, ValueTask<TDestination>> selector, DelegateTransformerOptions options)
        where TSource : notnull
        where TDestination : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SelectTransformer<TSource, TDestination>(selector, options));
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
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SelectManyTransformer<TSource, TDestination>(selector));
    }



    /// <summary>
    /// Flattens the pipeline like <c>SelectMany</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="TSource">The source item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The destination item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="selector">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> SelectMany<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, IEnumerable<TDestination>> selector, DelegateTransformerOptions options)
        where TSource : notnull
        where TDestination : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SelectManyTransformer<TSource, TDestination>(selector, options));
    }



    /// <summary>
    /// Flattens the pipeline like <c>SelectMany</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="TSource">The source item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The destination item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="selector">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> SelectMany<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, Func<TSource, IAsyncEnumerable<TDestination>> selector, DelegateTransformerOptions options)
        where TSource : notnull
        where TDestination : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SelectManyTransformer<TSource, TDestination>(selector, options));
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
    public static IEtlPipeline<T> Distinct<T>(this IEtlPipeline<T> pipeline, IEqualityComparer<T>? comparer)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new DistinctTransformer<T>(comparer));
    }



    /// <summary>Removes duplicate items using <see cref="EqualityComparer{T}.Default"/>.</summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to deduplicate.</param>
    /// <returns>A pipeline with duplicate items removed, preserving first-seen order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Distinct<T>(this IEtlPipeline<T> pipeline)
        where T : notnull
    {
        return pipeline.Distinct(comparer: null);
    }



    /// <summary>
    /// De-duplicates the pipeline like <c>Distinct</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="comparer">The equality comparer, or null for the default.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Distinct<T>(this IEtlPipeline<T> pipeline, IEqualityComparer<T>? comparer, DelegateTransformerOptions options)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new DistinctTransformer<T>(comparer, options));
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
    public static IEtlPipeline<TSource> DistinctBy<TSource, TKey>(this IEtlPipeline<TSource> pipeline, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        where TSource : notnull
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new DistinctByTransformer<TSource, TKey>(keySelector, comparer));
    }



    /// <summary>Removes items with duplicate keys using <see cref="EqualityComparer{TKey}.Default"/>.</summary>
    /// <typeparam name="TSource">The item type. Must be non-null.</typeparam>
    /// <typeparam name="TKey">The key type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to deduplicate.</param>
    /// <param name="keySelector">Extracts the key each item is deduplicated by.</param>
    /// <returns>A pipeline keeping the first item seen for each distinct key.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<TSource> DistinctBy<TSource, TKey>(this IEtlPipeline<TSource> pipeline, Func<TSource, TKey> keySelector)
        where TSource : notnull
        where TKey : notnull
    {
        return pipeline.DistinctBy(keySelector, comparer: null);
    }



    /// <summary>
    /// De-duplicates by key the pipeline like <c>DistinctBy</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="TSource">The source item type. Must be non-null.</typeparam>
    /// <typeparam name="TKey">The key type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="keySelector">The caller-supplied delegate.</param>
    /// <param name="comparer">The comparer used to compare keys, or null for the default.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<TSource> DistinctBy<TSource, TKey>(this IEtlPipeline<TSource> pipeline, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer, DelegateTransformerOptions options)
        where TSource : notnull
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new DistinctByTransformer<TSource, TKey>(keySelector, comparer, options));
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
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new TakeWhileTransformer<T>(predicate));
    }



    /// <summary>
    /// Takes items while a predicate holds the pipeline like <c>TakeWhile</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="predicate">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> TakeWhile<T>(this IEtlPipeline<T> pipeline, Func<T, bool> predicate, DelegateTransformerOptions options)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new TakeWhileTransformer<T>(predicate, options));
    }



    /// <summary>
    /// Takes items while a predicate holds the pipeline like <c>TakeWhile</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="predicate">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> TakeWhile<T>(this IEtlPipeline<T> pipeline, Func<T, ValueTask<bool>> predicate, DelegateTransformerOptions options)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new TakeWhileTransformer<T>(predicate, options));
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
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SkipWhileTransformer<T>(predicate));
    }



    /// <summary>
    /// Skips items while a predicate holds the pipeline like <c>SkipWhile</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="predicate">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> SkipWhile<T>(this IEtlPipeline<T> pipeline, Func<T, bool> predicate, DelegateTransformerOptions options)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SkipWhileTransformer<T>(predicate, options));
    }



    /// <summary>
    /// Skips items while a predicate holds the pipeline like <c>SkipWhile</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="predicate">The caller-supplied delegate.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> SkipWhile<T>(this IEtlPipeline<T> pipeline, Func<T, ValueTask<bool>> predicate, DelegateTransformerOptions options)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new SkipWhileTransformer<T>(predicate, options));
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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new ChunkTransformer<T>(size));
    }



    /// <summary>
    /// Groups the pipeline's items into fixed-size chunks like <c>Chunk(size)</c>, additionally reporting the
    /// running count of source items consumed to <paramref name="progress"/> after each chunk.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="size">The maximum number of items per chunk. Must be at least 1.</param>
    /// <param name="progress">The sink for the running consumed-item count, or <see langword="null"/> for none.</param>
    /// <returns>A pipeline of read-only lists, each containing up to <paramref name="size"/> items.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is less than 1.</exception>
    public static IEtlPipeline<IReadOnlyList<T>> Chunk<T>(this IEtlPipeline<T> pipeline, int size, IProgress<int>? progress)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new ChunkTransformer<T>(size, progress));
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
        ArgumentNullException.ThrowIfNull(pipeline);

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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new CastTransformer<TSource, TDestination>());
    }



    /// <summary>
    /// Casts the pipeline like <c>Cast</c> without options, but applies the
    /// <see cref="DelegateTransformerOptions.ErrorPolicy"/> in <paramref name="options"/> to an item whose delegate (or comparer) throws, so
    /// the item can be skipped and counted in <c>EtlPipelineProgress.ErrorItemCount</c> instead of aborting the run.
    /// </summary>
    /// <typeparam name="TSource">The source item type. Must be non-null.</typeparam>
    /// <typeparam name="TDestination">The destination item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to extend.</param>
    /// <param name="options">The per-item error handling options.</param>
    /// <returns>The extended pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="options"/> is <see langword="null"/>, or the delegate argument is <see langword="null"/>.</exception>
    public static IEtlPipeline<TDestination> Cast<TSource, TDestination>(this IEtlPipeline<TSource> pipeline, DelegateTransformerOptions options)
        where TSource : notnull
        where TDestination : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new CastTransformer<TSource, TDestination>(options));
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
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new OfTypeTransformer<TSource, TDestination>());
    }



    /// <summary>
    /// Runs a synchronous side effect on each item and passes the item through unchanged — an
    /// observability "tap" point (logging, metrics, debugging) that does not alter the stream's shape.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to observe.</param>
    /// <param name="onItem">The side effect to run for each item before it flows on.</param>
    /// <returns>A pipeline yielding the same items, in the same order, with <paramref name="onItem"/> invoked per item.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="onItem"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    ///     await EtlPipeline
    ///         .Create()
    ///         .From(records)
    ///         .Tap(r =&gt; Console.WriteLine($"seen {r.Id}"))
    ///         .To(loader)
    ///         .RunAsync();
    /// </code>
    /// </example>
    public static IEtlPipeline<T> Tap<T>(this IEtlPipeline<T> pipeline, Action<T> onItem)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new ProgressReportingTransformer<T>(onItem));
    }



    /// <summary>
    /// Runs an asynchronous side effect on each item and passes the item through unchanged — an
    /// observability "tap" point that does not alter the stream's shape.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to observe.</param>
    /// <param name="onItem">The asynchronous side effect to run for each item before it flows on.</param>
    /// <returns>A pipeline yielding the same items, in the same order, with <paramref name="onItem"/> awaited per item.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> or <paramref name="onItem"/> is <see langword="null"/>.</exception>
    public static IEtlPipeline<T> Tap<T>(this IEtlPipeline<T> pipeline, Func<T, ValueTask> onItem)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new ProgressReportingTransformer<T>(onItem));
    }



    /// <summary>
    /// Writes a log line per item and passes the item through unchanged — a <see cref="Tap{T}(IEtlPipeline{T}, Action{T})"/>
    /// bound to a formatter and a sink. Deliberately dependency-free: it takes a delegate sink rather than
    /// referencing <c>Microsoft.Extensions.Logging</c>, so bridge to an <c>ILogger</c> at the call site if desired.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to observe.</param>
    /// <param name="format">Produces the log message for an item.</param>
    /// <param name="sink">Receives each formatted message (e.g. <c>Console.WriteLine</c> or <c>msg =&gt; logger.LogInformation(msg)</c>).</param>
    /// <returns>A pipeline yielding the same items, in the same order, logging one message per item.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/>, <paramref name="format"/>, or <paramref name="sink"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    ///     await EtlPipeline
    ///         .Create()
    ///         .From(records)
    ///         .Log(r =&gt; $"processing {r.Id}", Console.WriteLine)
    ///         .To(loader)
    ///         .RunAsync();
    /// </code>
    /// </example>
    public static IEtlPipeline<T> Log<T>(this IEtlPipeline<T> pipeline, Func<T, string> format, Action<string> sink)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(format);
        ArgumentNullException.ThrowIfNull(sink);

        return pipeline.Tap(item => sink(format(item)));
    }



    /// <summary>
    /// Paces the pipeline so successive items are yielded at least <paramref name="minInterval"/> apart,
    /// without changing the stream's shape or order — rate-limiting for a downstream sink that must not be
    /// hit too fast. The first item is not delayed; pacing is adaptive (a consumer that was already slow is
    /// not delayed further) and observes the pipeline's cancellation token.
    /// </summary>
    /// <typeparam name="T">The item type. Must be non-null.</typeparam>
    /// <param name="pipeline">The pipeline to pace.</param>
    /// <param name="minInterval">
    /// The minimum time between successive items. <see cref="TimeSpan.Zero"/> is a pass-through. Must not
    /// exceed <see cref="int.MaxValue"/> milliseconds (about 24.8 days).
    /// </param>
    /// <returns>A pipeline yielding the same items, in the same order, paced apart.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minInterval"/> is negative or longer than <see cref="int.MaxValue"/> milliseconds.
    /// </exception>
    /// <example>
    /// <code>
    ///     await EtlPipeline
    ///         .Create()
    ///         .From(records)
    ///         .Throttle(TimeSpan.FromMilliseconds(200))   // at most ~5 items/second
    ///         .To(loader)
    ///         .RunAsync();
    /// </code>
    /// </example>
    public static IEtlPipeline<T> Throttle<T>(this IEtlPipeline<T> pipeline, TimeSpan minInterval)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Through(new ThrottleTransformer<T>(minInterval));
    }
}
