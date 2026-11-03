using System;
using System.Threading;
using Wolfgang.Etl.Abstractions;



namespace Wolfgang.Etl.Transformers;

/// <summary>
/// The per-item error policy and skip counter shared by every transformer that accepts
/// <see cref="DelegateTransformerOptions"/>.
/// </summary>
/// <remarks>
/// Each such transformer owns one instance, applies the policy through <see cref="Handle"/> when its
/// delegate throws, and surfaces <see cref="Count"/> as <c>IReportsItemErrors.CurrentErrorItemCount</c>.
/// The count is cumulative across every enumeration of the owning transformer and is updated
/// atomically, so concurrent enumerations do not lose increments.
/// </remarks>
internal sealed class ItemErrorCounter
{
    private readonly Func<ItemErrorContext, ItemErrorAction> _errorPolicy;
    private int _count;



    /// <summary>
    /// Initializes a new instance of the <see cref="ItemErrorCounter"/> class.
    /// </summary>
    /// <param name="options">The options whose <see cref="DelegateTransformerOptions.ErrorPolicy"/> is applied.</param>
    internal ItemErrorCounter(DelegateTransformerOptions options)
    {
        _errorPolicy = options.ErrorPolicy;
    }



    /// <summary>
    /// The number of items skipped so far because the policy returned <see cref="ItemErrorAction.Skip"/>.
    /// </summary>
    internal int Count => Volatile.Read(ref _count);



    /// <summary>
    /// Applies the error policy to one failed item and, when it asks to skip, counts the item.
    /// </summary>
    /// <param name="itemNumber">The 1-based number of the failed item.</param>
    /// <param name="exception">The exception the caller-supplied delegate threw.</param>
    /// <returns>The action the policy chose.</returns>
    internal ItemErrorAction Handle(long itemNumber, Exception exception)
    {
        var action = _errorPolicy(new ItemErrorContext(itemNumber, exception));
        if (action == ItemErrorAction.Skip)
        {
            _ = Interlocked.Increment(ref _count);
        }

        return action;
    }
}
