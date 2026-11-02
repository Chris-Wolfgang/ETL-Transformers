using System;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// Configuration for the transformers that invoke a caller-supplied delegate, controlling what
/// happens when that delegate throws for a single item.
/// </summary>
/// <remarks>
/// <para>
/// Per ADR-0009, stage configuration is supplied as a record passed to the constructor rather than
/// set through mutable properties, so a transformer is configured once and not reconfigured during a
/// run. This record is shared by the transformers that accept it, because the one setting they
/// need is the same. Every transformer that invokes a caller-supplied delegate or comparer accepts
/// it: <see cref="CastTransformer{TSource, TDestination}"/>, <see cref="DistinctTransformer{T}"/>,
/// <see cref="DistinctByTransformer{TSource, TKey}"/>,
/// <see cref="SelectTransformer{TSource, TDestination}"/>,
/// <see cref="SelectManyTransformer{TSource, TDestination}"/>, <see cref="SkipWhileTransformer{T}"/>,
/// <see cref="TakeWhileTransformer{T}"/> and <see cref="WhereTransformer{T}"/>. It is left unsealed
/// so a transformer that later grows its own settings can derive from it.
/// </para>
/// <para>
/// It deliberately does <b>not</b> derive from
/// <see cref="TransformerOptions"/>. That record also carries
/// <c>SkipItemCount</c>, <c>MaximumItemCount</c> and <c>ReportingInterval</c>, which the
/// transformers in this library do not implement - they carry no counters by design - and which are
/// already expressed here by the dedicated <see cref="SkipTransformer{T}"/> and
/// <see cref="TakeTransformer{T}"/> stages. Inheriting them would advertise settings that are
/// silently ignored and give the same concept two spellings.
/// </para>
/// <para>
/// The pass-through, windowing, pacing, type-filtering and composition transformers
/// (<see cref="PassThroughTransformer{T}"/>, <see cref="TakeTransformer{T}"/>,
/// <see cref="SkipTransformer{T}"/>, <see cref="ChunkTransformer{T}"/>,
/// <see cref="BufferedTransformer{T}"/>, <see cref="ProgressReportingTransformer{T}"/>,
/// <see cref="ThrottleTransformer{T}"/>, <see cref="OfTypeTransformer{TSource, TDestination}"/>,
/// <see cref="ChainTransformer{TSource, TIntermediate, TDestination}"/> and
/// <see cref="ChainTransformerWithCancellation{TSource, TIntermediate, TDestination}"/>) invoke no
/// caller delegate, so they have nothing to apply a policy to and do not accept this record.
/// </para>
/// </remarks>
/// <example>
/// <code>
///     // Drop the items whose projection throws, and count them.
///     var options = new DelegateTransformerOptions { ErrorPolicy = _ => ItemErrorAction.Skip };
///     var transformer = new SelectTransformer&lt;string, int&gt;(int.Parse, options);
/// </code>
/// </example>
public record DelegateTransformerOptions
{
    /// <summary>
    /// The shared instance used by the constructors that take no options, so the default path does
    /// not allocate a record per transformer.
    /// </summary>
    /// <remarks>
    /// Safe to share: the record is immutable - every property is <see langword="init"/>-only - and
    /// <see cref="ErrorPolicy"/> defaults to a cached static lambda.
    /// </remarks>
    internal static DelegateTransformerOptions Default { get; } = new();



    /// <summary>
    /// The policy applied when the caller-supplied delegate throws while handling one item.
    /// </summary>
    /// <value>
    /// A function receiving an <see cref="ItemErrorContext"/> describing the failure and returning
    /// <see cref="ItemErrorAction.Skip"/> to drop the item and continue, or
    /// <see cref="ItemErrorAction.Abort"/> to re-throw. Defaults to a policy that always returns
    /// <see cref="ItemErrorAction.Abort"/>, which is the behaviour of a transformer constructed
    /// without this record: the exception propagates to the caller unchanged.
    /// </value>
    /// <remarks>
    /// Ready-made policies - including skip-and-log and skip-to-dead-letter forms - are available
    /// from <c>ItemErrorPolicy</c> in the <c>Wolfgang.Etl.ErrorPolicies</c> package. This library
    /// does not reference that package; the policy is just a delegate.
    /// </remarks>
    /// <exception cref="ArgumentNullException">The value being assigned is <see langword="null"/>.</exception>
    public Func<ItemErrorContext, ItemErrorAction> ErrorPolicy
    {
        get;
        init => field = value ?? throw new ArgumentNullException(nameof(value));
    } = static _ => ItemErrorAction.Abort;
}
