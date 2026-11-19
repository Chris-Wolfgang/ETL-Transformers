# ADR-0006: Record architecture decisions

- **Status**: Accepted
- **Date**: 2026-11-17
- **Deciders**: @Chris-Wolfgang

## Context

`Wolfgang.Etl.Transformers` makes several choices that the code alone does not
explain: blocking APIs are banned by an analyzer, record types are constrained to
`notnull`, the library targets frameworks back to `net462`, and the assembly version
follows the minor version instead of the package version. Without the reason next to
it, each of these reads as an accident that a later change might "tidy up".

The repository has kept such records in `docs/adr` since ADR-0001, and
[`index.md`](index.md) describes how. The practice itself was never recorded as a
decision, which is why this record carries a later number than the decisions it
describes.

## Decision

We record each significant, non-obvious design decision as an Architecture Decision
Record in `docs/adr`, following the rules already stated in [`index.md`](index.md):

- one Markdown file per decision, `NNNN-title.md` with the next free number, copied
  from [`TEMPLATE.md`](TEMPLATE.md), with a row in the index;
- an ADR lands in the pull request that introduces the decision and is part of that
  review;
- an ADR is never deleted. A decision that changes gets a new ADR, and the old one is
  marked `Superseded by ADR-XXXX`.

## Consequences

- **Positive** — a contributor can find out why something is the way it is without
  reading the history, and reversing a decision is explicit.
- **Negative / accepted trade-off** — a decision worth recording costs a few minutes
  to write up, and the index has to be kept in step with the files by hand.
- **Neutral** — a record written after the fact, as this one and ADR-0005 are,
  reconstructs the reasoning and can miss part of it.

## Alternatives considered

- **Leave the reasoning in commit messages and pull requests** — it is far from the
  code and has to be dug out of history, if it was written down at all.
