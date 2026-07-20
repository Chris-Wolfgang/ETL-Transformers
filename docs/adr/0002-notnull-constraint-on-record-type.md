# ADR-0002: `where T : notnull` on transformer record types

- **Status**: Accepted
- **Date**: 2026-07-20
- **Deciders**: @Chris-Wolfgang

## Context

Every transformer flows a stream of records (`T`, `TSource`, `TDestination`)
through `IAsyncEnumerable`. The base types in `Wolfgang.Etl.Abstractions`
constrain their record type parameters to `notnull`, and the transformers here
build on that base.

A `null` record in an ETL stream is almost always a defect: it is ambiguous
(end-of-stream? skipped row? genuine value?), and it forces every downstream
transformer and loader to null-check the item on the hot path. Allowing `T` to
be a nullable reference type would push that ambiguity through the whole
pipeline.

## Decision

All transformer type parameters carry `where T : notnull` (inherited from the
Abstractions base classes). Records in the stream are non-null by contract;
"no value" is represented by the stream simply not yielding an item, not by
yielding `null`.

## Consequences

- **Positive** — downstream stages never have to defend against `null` items;
  the nullable-reference-type analysis stays clean end-to-end; the "no value"
  case has one unambiguous representation (yield nothing).
- **Negative / accepted trade-off** — consumers cannot use a nullable reference
  type (`string?`) or `Nullable<T>` directly as the record type; they wrap the
  optionality in the value shape or filter nulls out at the extractor edge.
- **Neutral** — matches the constraint the rest of the ETL family already
  imposes, so records compose across libraries without constraint mismatches.

## Alternatives considered

- **Leave `T` unconstrained** — would compile, but reintroduces null ambiguity
  and per-stage null checks, and would diverge from the Abstractions contract.
