# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Observability pipeline operator `Tap` on `IEtlPipeline<T>` (sync `Action<T>` and async
  `Func<T, ValueTask>`): runs a side effect per item and passes the item through unchanged,
  layered over `Through` (no core changes, no new dependencies). First of the observability
  operator set. ([#174](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/174))
- Property-based fuzzing: a CsCheck suite asserts each transformer is equivalent to its
  `System.Linq` counterpart on randomised input, plus a scheduled `fuzz.yaml` (high iteration
  count, uploads results and auto-files an issue on failure). ([#76](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/76))

- Shadow testing: `samples/ShadowWorkloads` models realistic production traffic (variable
  page sizes, concurrent enumeration, mixed sync/async sources, bursty windows) across the
  public transformers, and a nightly `Shadow Testing` workflow replays them against a
  committed golden baseline, opening a tracking issue and failing on regression beyond
  threshold. Documented in `docs/shadow-testing.md`. ([#77](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/77))

- Security-grade SAST beyond CodeQL: a `semgrep.yaml` workflow runs Semgrep OSS
  (p/csharp, p/security-audit, p/secrets), diff-aware with SARIF upload to code scanning, plus
  `docs/sast.md`. ([#78](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/78))

- ABI-compatibility gate: `EnablePackageValidation` with a pinned
  `PackageValidationBaselineVersion` fails the build on a breaking public-API change versus
  the last published release. ([#83](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/83))

- Concurrency / race-condition testing: a `Tests.Concurrency` project defines interleaving
  scenarios (concurrent enumeration of one transformer, BufferedTransformer producer/consumer,
  cancellation mid-enumeration) run both as an xunit stress gate and as Microsoft Coyote
  systematic-exploration entry points. A weekly `Concurrency (Coyote)` workflow runs the
  stress gate (blocking) and Coyote exploration (non-blocking, traces uploaded). Documented
  in `docs/concurrency-testing.md`. ([#84](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/84))

- Supply-chain hardening: releases now emit a Sigstore-backed SLSA build-provenance
  attestation (`actions/attest-build-provenance`, verifiable via `gh attestation
  verify`) and support secret-gated NuGet author-signing (inert until a code-signing
  certificate is configured; NuGet.org repository signing applies regardless). SECURITY.md
  documents the full consumer verification chain (signature → provenance → SBOM →
  reproducible build). ([#85](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/85))

- Cross-platform/multi-arch differential: a `Cross-Platform Differential` workflow runs
  the test suite on linux-x64, linux-arm64, macos-arm64, and windows-x64, normalizes the
  `.trx` outcomes (`scripts/normalize-test-results.py`), and fails on any divergence
  between platforms — adding ARM64 coverage and catching bugs a per-OS pass/fail gate
  misses. Platform-specific tests opt out via `[Trait("Category","PlatformSpecific")]`.
  Documented in `docs/cross-platform-differential.md`. ([#86](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/86))

- Snapshot / approval testing: a `Tests.Snapshots` project uses Verify to lock stable
  transformer outputs against committed snapshots, plus `docs/snapshot-testing.md`. ([#87](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/87))

- Sustained-load GC/allocation profiling: a scheduled `GC Profiling` workflow drives a
  new `Wolfgang.Etl.Transformers.Profiling` harness under ~10 min of continuous pipeline
  load, captures cross-platform EventPipe data (`dotnet-counters` CSV + `dotnet-gcdump`
  heap snapshot + `dotnet-trace` GC trace), and gates gen2/LOH/allocation-rate against a
  committed baseline via `scripts/gc-profile-report.py`. Documented in `docs/gc-profiling.md`. ([#89](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/89))

- Native-AOT / trim smoke: an `AotConsumer` roots every public call site and an `aot.yaml`
  workflow publishes it with `PublishAot` / `PublishTrimmed` / `IlcTreatWarningsAsErrors`,
  failing on any IL trim/AOT warning. ([#90](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/90))

- Globalization / culture-invariance test matrix: `GlobalizationInvarianceTests` runs the
  suite under en-US, tr-TR, de-DE, zh-CN, ar-SA and ja-JP to catch culture-sensitive
  behaviour (numeric/date formatting, ordinal vs culture-aware comparison), plus
  `docs/globalization.md`. ([#92](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/92))

- Reproducible-build verification: a `Reproducible Build` workflow packs the
  project on Ubuntu and Windows and compares output hashes — same-OS/SDK
  determinism is the guarantee; cross-OS divergence is reported as an advisory
  warning (not a failure) pending a tracked follow-up. Plus `REPRODUCIBLE-BUILD.md`
  documenting the guarantee and how a third party can verify a published package
  against source on the same OS. ([#93](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/93))

- Allocation-budget enforcement: a net10.0 `Tests.Allocation` project asserts the
  library's one intentional zero-allocation hot path
  (`PassThroughTransformer<T>.TransformAsync(IAsyncEnumerable<T>)` returns the
  source by reference, 0 bytes) via `GC.GetAllocatedBytesForCurrentThread`, plus
  `docs/allocation-budget.md` documenting the covered call sites and why streaming
  transformers are deliberately out of scope. ([#94](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/94))

- Per-PR benchmark regression gate: a `PR Benchmarks` workflow runs BenchmarkDotNet
  on the PR head and its base commit, posts a sticky delta-table comment, and fails
  the PR when a benchmark is >20% slower or allocates >50% more (overridable with the
  `perf-impact-acknowledged` label). Backed by `scripts/compare-benchmarks.py` and
  documented in `docs/pr-benchmarks.md`. ([#101](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/101))

- Consumer-side reproducible-build verification: `REPRODUCIBLE-BUILD.md` now documents
  how a third party regenerates and diffs the per-release
  `reproducible-build-manifest.json` (produced by `scripts/generate-repro-manifest.py`),
  files a discrepancy, and publishes an independent verification attestation; a "Verify
  the build" section links it from the README. ([#102](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/102))

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [0.3.0] - 2026-07-21

### Added

- LINQ-flavored pipeline operator extensions on `IEtlPipeline<T>`
  (`EtlPipelineOperatorExtensions`): `Where`, `Select`, `SelectMany` (sync +
  async), `Distinct`, `DistinctBy`, `Take`, `Skip`, `TakeWhile`, `SkipWhile`,
  `Chunk`, `Buffered`, `Cast`, `OfType`. Each is a thin wrapper over the
  matching transformer via the pipeline core's `Through(...)`, lighting up the
  `EtlPipeline.Create().From(...).Where(...).Select(...).To(...).RunAsync()`
  fluent chain once this package is referenced. ([#150](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/150))

### Changed

- Bumped `Wolfgang.Etl.Abstractions` from 0.15.0 to 0.20.0 (ships the
  `EtlPipeline` core the operators build on, plus item-error handling,
  middleware, and time-source abstractions).

## [0.2.1] - 2026-07-06

### Changed

- Dependabot bump: dotnet-dependencies group (7 packages).
## [0.2.0] - 2026-06-26

### Changed

- **Breaking:** `ChunkTransformer<T>` now produces `IReadOnlyList<T>` instead of `T[]` (`ITransformAsync<T, IReadOnlyList<T>>`). Callers that relied on the array contract (indexing a `T[]` variable, passing chunks where an array is required) must adjust to the read-only list.
- Bumped `Wolfgang.Etl.Abstractions` to 0.14.1 and `Microsoft.Bcl.AsyncInterfaces` to 10.0.9.

### Added

- `ChunkTransformer<T>` gains a constructor overload accepting an optional `IProgress<int>?` sink that reports the cumulative item count as chunks are produced.

## [0.1.1] - 2026-06-20

### Changed

- Corrected README Target Frameworks table (removed net4.7/net4.7.1, added .NET Standard 2.0)
- Fixed README and CHANGELOG `Then(...)` overload count: two overloads, not four
- Replaced README Quick Start inline-extension example with a self-contained example using only this package
- Updated CHANGELOG v0.1.0 entry: release date, test count (257 unit + 11 integration), Keep-a-Changelog footer link

### Added

- Integration test project (`Wolfgang.Etl.Transformers.Tests.Integration`) with 11 pipeline composition tests
- Shared `TestHelpers` class in the unit test project (eliminates duplicate `ToAsync`/`CollectAsync` helpers)
- Documentation version picker (`docfx_project/public/version-picker.js` + `versions.json`)
- Canonical `benchmarks.yaml` GitHub Actions workflow (interactive BenchmarkDotNet line chart on gh-pages)

### Fixed

- `ETL-Transformers.slnx`: removed references to 6 files that were never created after template setup

[Unreleased]: https://github.com/Chris-Wolfgang/ETL-Transformers/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/Chris-Wolfgang/ETL-Transformers/compare/v0.2.1...v0.3.0
[0.2.1]: https://github.com/Chris-Wolfgang/ETL-Transformers/compare/v0.2.0...v0.2.1
[0.2.0]: https://github.com/Chris-Wolfgang/ETL-Transformers/compare/v0.1.1...v0.2.0
[0.1.1]: https://github.com/Chris-Wolfgang/ETL-Transformers/compare/v0.1.0...v0.1.1

## [0.1.0] - 2026-06-20

### Added

- **LINQ-style transformers**: `WhereTransformer<T>`, `SelectTransformer<TSource, TDestination>`,
  `SelectManyTransformer<TSource, TDestination>`, `OfTypeTransformer<TSource, TDestination>`,
  `CastTransformer<TSource, TDestination>`, `DistinctTransformer<T>`,
  `DistinctByTransformer<TSource, TKey>`, `TakeTransformer<T>`, `TakeWhileTransformer<T>`,
  `SkipTransformer<T>`, `SkipWhileTransformer<T>`, `ChunkTransformer<T>`
- **Pipeline infrastructure**: `PassThroughTransformer<T>` (identity / tap point),
  `BufferedTransformer<T>` (producer–consumer decoupling via `System.Threading.Channels`),
  `ProgressReportingTransformer<T>` (per-item callback without altering the stream)
- **Composition**: `ChainTransformer<TSource, TIntermediate, TDestination>`,
  `ChainTransformerWithCancellation<TSource, TIntermediate, TDestination>`,
  `TransformerExtensions.Then(...)` (2 overloads), `TransformerExtensions.Buffered(...)`
- Multi-TFM targeting: .NET Framework 4.6.2–4.8.1, .NET Standard 2.0, .NET 5.0–10.0
- 257 unit tests + 11 integration tests with 100% line and method coverage
- BenchmarkDotNet project for baseline performance measurement
- Full DocFX API documentation site

[0.1.0]: https://github.com/Chris-Wolfgang/ETL-Transformers/releases/tag/v0.1.0
