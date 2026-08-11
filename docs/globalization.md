# Globalization / culture invariance

`Wolfgang.Etl.Transformers` is **culture-invariant**: no public method's behavior
depends on `CultureInfo.CurrentCulture` / `CurrentUICulture`.

## Why

The transformers operate on generic `T` and perform no formatting or parsing of
strings, dates, or numbers. The only comparison points are equality and keying:

- `DistinctTransformer<T>` and `DistinctByTransformer<TSource,TKey>` default to
  `EqualityComparer<T>.Default`, which is **ordinal** for `string` (and
  structural for other types) — not culture-sensitive.
- When a caller supplies an `IEqualityComparer<T>`/`IEqualityComparer<TKey>`, the
  comparison semantics are the caller's choice. Passing a culture-sensitive
  comparer (e.g. `StringComparer.CurrentCultureIgnoreCase`) makes *that call*
  culture-sensitive by the caller's intent — the library does not impose it.

## Allowlist of intentionally culture-sensitive public methods

**None.** No public method in this library is intended to vary by culture.

## Enforcement

[`GlobalizationInvarianceTests`](../tests/Wolfgang.Etl.Transformers.Tests.Unit/Globalization/GlobalizationInvarianceTests.cs)
runs a representative set of operations under `en-US`, `tr-TR`, `de-DE`, `zh-CN`,
`ar-SA`, and `ja-JP` (swapping both `CurrentCulture` and `CurrentUICulture` and
restoring them after each test), asserting identical, ordinal results in every
culture. If a future change introduces culture-sensitive behavior, add it to the
allowlist above and cover it explicitly.
