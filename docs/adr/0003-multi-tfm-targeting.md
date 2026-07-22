# ADR-0003: Broad multi-target-framework matrix (net462 → net10.0)

- **Status**: Accepted
- **Date**: 2026-07-20
- **Deciders**: @Chris-Wolfgang

## Context

The library targets a wide framework matrix:
`net462;net472;net48;net481;netstandard2.0;net5.0;net6.0;net7.0;net8.0;net9.0;net10.0`.

ETL glue code lives in long-lived line-of-business systems, many of which are
still on .NET Framework 4.x and cannot move to modern .NET on the timeline of a
dependency upgrade. Restricting to `net8.0+` would make the library unusable for
exactly the integration-heavy codebases that most need reusable transformers.

## Decision

We target the full matrix down to `net462`, with `netstandard2.0` as the broad
floor and explicit modern TFMs above it. `SuppressTfmSupportBuildWarnings` is
set for the older targets, and `TargetFrameworks` is kept on a **single line**
in the csproj because CI greps it.

## Consequences

- **Positive** — the library drops into .NET Framework 4.x apps and modern .NET
  apps alike without a shim; consumers pick the best TFM their app resolves to.
- **Negative / accepted trade-off** — more build/test surface (every TFM is
  built and tested), occasional per-TFM polyfills and `#if` guards, and API
  choices constrained to what `netstandard2.0` can express unless guarded.
- **Neutral** — the async streaming surface relies on
  `Microsoft.Bcl.AsyncInterfaces` to make `IAsyncEnumerable<T>` available on the
  down-level targets.

## Alternatives considered

- **`net8.0`+ only** — simpler build, smaller test matrix, but excludes the
  .NET Framework consumers this library is meant to serve.
- **`netstandard2.0` only** — one TFM, but forgoes modern-runtime optimizations
  and newer BCL surface available on `net6.0+`.
