# ADR-0001: Async-first enforcement via BannedSymbols.txt

- **Status**: Accepted
- **Date**: 2026-07-20
- **Deciders**: @Chris-Wolfgang

## Context

ETL work is dominated by I/O — reading sources, writing destinations, awaiting
downstream systems. The transformers in this library sit in the middle of an
`IAsyncEnumerable<T>` pipeline whose whole point is non-blocking streaming.

Blocking synchronous APIs (`Task.Wait()`, `Task<T>.Result`,
`GetAwaiter().GetResult()`, `Thread.Sleep`, synchronous `File`/`Stream` I/O,
`Parallel.For`/`ForEach`, blocking `Console` reads) are easy to reach for by
habit, compile without complaint, and then cause thread-pool starvation or
outright deadlock when they appear on an async path — especially under a
single-threaded synchronization context. Code review alone does not reliably
catch them.

## Decision

We enforce the async-first rule mechanically with
`Microsoft.CodeAnalysis.BannedApiAnalyzers` and a repo-level
[`BannedSymbols.txt`](../../BannedSymbols.txt) that bans the blocking APIs
outright, each with a documented async replacement. Violations are build errors
(warnings-as-errors in Release), so a blocking call cannot merge.

## Consequences

- **Positive** — the pipeline stays non-blocking by construction; a whole class
  of deadlock/starvation bugs is impossible to introduce. The reason for each
  ban travels with the ban (the reason column doubles as the fix hint).
- **Negative / accepted trade-off** — the ban list is maintained by hand and
  must be kept in sync across the repo family; genuinely-needed synchronous
  calls (rare) require an explicit, reviewed `#pragma` suppression.
- **Neutral** — the list is deliberately broad (network, serialization, DateTime
  anti-patterns) beyond the strict async concern, acting as a general
  "don't-use-this" gate.

## Alternatives considered

- **Rely on code review / AsyncFixer only** — AsyncFixer catches some patterns
  but not all banned APIs, and review misses them under time pressure. A hard
  build gate is deterministic.
