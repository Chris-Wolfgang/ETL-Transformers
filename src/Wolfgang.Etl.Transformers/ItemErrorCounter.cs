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
/// The count is per run: it is reset when an enumeration begins while no other enumeration of the
/// owning transformer is active, so re-running a pipeline reports only that run's skips. Enumerations
/// that overlap in time share one count, and increments are atomic so none are lost.
/// </remarks>
internal sealed class ItemErrorCounter
{
    private readonly Func<ItemErrorContext, ItemErrorAction> _errorPolicy;
    private int _count;
    private int _active;



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


    /// <summary>
    /// Marks the start of one enumeration. When no other enumeration is active the count restarts at
    /// zero, giving <c>IReportsItemErrors.CurrentErrorItemCount</c> its "so far in the current run" meaning.
    /// </summary>
    /// <returns>A scope that marks the enumeration as finished when disposed.</returns>
    internal Run BeginRun()
    {
        if (Interlocked.Increment(ref _active) == 1)
        {
            Volatile.Write(ref _count, 0);
        }

        return new Run(this);
    }



    /// <summary>The scope of one enumeration, returned by <see cref="BeginRun"/>.</summary>
    internal readonly struct Run : IDisposable, IEquatable<Run>
    {
        private readonly ItemErrorCounter _owner;



        internal Run(ItemErrorCounter owner) => _owner = owner;



        /// <inheritdoc />
        public void Dispose() => _ = Interlocked.Decrement(ref _owner._active);



        /// <inheritdoc />
        public bool Equals(Run other) => ReferenceEquals(_owner, other._owner);



        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Run other && Equals(other);



        /// <inheritdoc />
        public override int GetHashCode() => _owner is null ? 0 : _owner.GetHashCode();
    }
}
