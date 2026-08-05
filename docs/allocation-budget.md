# Allocation budget

Some public call sites are deliberately allocation-free. This document lists them,
explains what is *not* covered and why, and describes how the guarantee is enforced.

## Enforced zero-allocation hot paths

| Call site | Guarantee | Enforced by |
| --- | --- | --- |
| `PassThroughTransformer<T>.TransformAsync(IAsyncEnumerable<T>)` (single-argument overload) | Returns the source sequence **unchanged and by reference**, allocating **0 bytes** — no wrapping iterator. | `AllocationBudgetTests.PassThroughTransformer_single_arg_TransformAsync_allocates_zero_bytes` (asserts a byte delta of exactly `0`) and `..._returns_same_instance` (asserts reference identity). |

This is the library's one intentional zero-allocation hot path. Because it performs
no work, `PassThroughTransformer<T>` implements `ITransformWithCancellationAsync<T, T>`
directly rather than deriving from `TransformerBase<,,>`, and its single-argument
overload short-circuits to `return items;`.

## Deliberately out of scope

Every other transformer in this package is a **streaming** transformer: it wraps the
source in a C# async iterator (`async IAsyncEnumerable<T>`) so that items flow lazily
with cancellation support. Constructing that iterator allocates exactly one state-machine
object **per enumeration** — this is inherent to `IAsyncEnumerable<T>` and is not a
regression to be eliminated. The cancellation-aware
`PassThroughTransformer<T>.TransformAsync(IAsyncEnumerable<T>, CancellationToken)`
overload is the clearest example, and
`AllocationBudgetTests.PassThroughTransformer_cancellation_overload_allocates` asserts
it *does* allocate — locking in the documented distinction between the two overloads.

Per-**item** steady-state allocation is intentionally not asserted here: for streaming
transformers it depends on the caller-supplied `selector` / `predicate` delegates and on
the source sequence's own `MoveNextAsync` completion behaviour, neither of which the
transformer controls. Measuring it would test the caller's code, not this library's.

## Measurement methodology

The tests use `GC.GetAllocatedBytesForCurrentThread()`, a per-thread cumulative counter:

- **Same-thread measurement.** Each measured section is fully synchronous (no `await`),
  so the before/after reads happen on the same thread. An `await` that resumes on a
  different thread would read a *different* thread's counter and produce a meaningless
  delta — this is the standard async caveat for this API.
- **Warm-up.** A warm-up loop runs first so JIT/tiered-compilation allocations are not
  counted against the measured window.
- **No collection needed.** The counter is cumulative and unaffected by garbage
  collections, so no `GC.Collect()` dance is required.

## Regression behaviour

If a change makes the zero-alloc overload allocate, the assertion fails with the actual
byte delta and the offending test name, pointing straight at the regressed call site.

The tests live in `tests/Wolfgang.Etl.Transformers.Tests.Allocation` and target `net10.0`
only: the contract is a runtime property, and pinning one modern runtime keeps the
allocation numbers stable and deterministic.
