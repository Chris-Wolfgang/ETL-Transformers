# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
