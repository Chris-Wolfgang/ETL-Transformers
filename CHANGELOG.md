# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

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

- Bumped `Wolfgang.Etl.Abstractions` from 0.15.0 to 0.16.0 (ships the
  `EtlPipeline` core the operators build on).

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
