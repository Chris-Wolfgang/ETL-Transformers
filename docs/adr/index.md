# Architecture Decision Records

Short records of the non-obvious design decisions in
`Wolfgang.Etl.Transformers` — the context, the choice, and the trade-offs
accepted — so the reasoning survives past the PR that introduced it.

New ADRs land **alongside the PR** that introduces the corresponding decision
and are part of that review. Copy [`TEMPLATE.md`](TEMPLATE.md) to the next
`NNNN-title.md`, and add a row below. Never delete an ADR — supersede it (mark
the old one `Superseded by ADR-XXXX` and add the replacement).

| ADR | Title | Status |
| --- | --- | --- |
| [0001](0001-async-first-banned-symbols.md) | Async-first enforcement via BannedSymbols.txt | Accepted |
| [0002](0002-notnull-constraint-on-record-type.md) | `where T : notnull` on transformer record types | Accepted |
| [0003](0003-multi-tfm-targeting.md) | Broad multi-TFM matrix (net462 → net10.0) | Accepted |
| [0004](0004-assemblyversion-pinned-to-minor.md) | Pin `AssemblyVersion` to `major.minor.0.0` | Accepted |
