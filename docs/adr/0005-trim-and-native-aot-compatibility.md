# ADR-0005: Declare the library trim- and Native-AOT-compatible, and prove it twice

- **Status**: Accepted
- **Date**: 2026-11-17
- **Deciders**: @Chris-Wolfgang

## Context

The library targets `net8.0` and later alongside the older frameworks. A consumer
who publishes with `PublishTrimmed` or `PublishAot` gets `IL2xxx` / `IL3xxx`
warnings for every referenced library that does not declare itself compatible, and a
library that reflects over types the trimmer cannot see, or generates code at run
time, fails in the published app rather than at build time.

The transformers are generic classes over `IAsyncEnumerable<T>`. The shipped code
uses no reflection and no run-time code generation, and carries no trim or AOT
annotation or suppression.

Three pieces were already in place when this ADR was written, added at different
times: a published smoke consumer (July 2026), the compatibility declaration
(September 2026), and a Native AOT run of the unit suite (November 2026). None had a
record of why it is there or what it covers.

## Decision

We declare compatibility, enforce the declaration at build time, and verify it with
two native publishes.

1. **Declaration.** `Directory.Build.props` sets `IsAotCompatible` and `IsTrimmable`
   for the projects under `src/`, on the target frameworks compatible with `net8.0`.
2. **Enforcement.** `.editorconfig` keeps the `Trimming`, `AOT` and `SingleFile`
   analyzer categories at `warning`. Release builds treat warnings as errors, so a
   reflection or dynamic-code use in shipped code fails the build.
3. **Smoke consumer.** `.github/aot/AotConsumer.csproj` is a console app that roots
   the whole public transformer surface. It is published with `PublishAot` and
   `PublishTrimmed`, treats ILC warnings as errors, and the native binary is run.
4. **Unit suite under AOT.** `tests/Wolfgang.Etl.Transformers.Tests.Aot` links the
   sources of the unit test project and compiles them with `xunit.v3.aot` into a
   Native AOT executable, which is then run.

Both publishes are jobs in `.github/workflows/aot.yaml`. They target `linux-x64`,
run on every pull request, and fail on any `IL2xxx` / `IL3xxx` warning.

## Consequences

- **Positive** — a consumer can trim or AOT-publish an app that uses the library
  without warnings from it, and a regression is caught on the pull request instead
  of in a consumer's published app.
- **Positive** — the unit suite runs against the trimmed, natively compiled library,
  so behaviour is checked there and not only that the publish succeeds.
- **Negative / accepted trade-off** — shipped code that needs reflection or dynamic
  code must be redesigned or annotated, and an annotation on a public member is a
  public-surface change that needs its own decision.
- **Negative / accepted trade-off** — two more CI jobs on every pull request, and a
  second test project to keep building. A unit test that cannot run under AOT has to
  be excluded from the linked sources or rewritten.
- **Neutral** — nothing is claimed for the target frameworks below `net8.0`, and the
  native link and run are verified on `linux-x64` only.
- **Neutral** — `aot.yaml` runs on pull requests and on manual dispatch, not on a
  push to the default branch.

## Alternatives considered

- **Analyzers only** — the analyzers see this assembly's source, not what the
  trimmer and the native compiler do with the whole closure. Only a publish followed
  by a run shows that the result works.
- **Smoke consumer only** — it proves that the surface links and that the calls it
  makes work. The linked unit suite covers the behaviour the smoke consumer does not
  call.
- **Declaration without enforcement** — with the analyzer categories at their
  default severity the declaration would be a claim nothing checks.

*Recorded after the fact: the declaration and both checks were already in force when
this ADR was written.*
