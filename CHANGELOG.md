# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - Upcoming

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
  `TransformerExtensions.Then(...)` (4 overloads), `TransformerExtensions.Buffered(...)`
- Multi-TFM targeting: .NET Framework 4.6.2–4.8.1, .NET Core 3.1, .NET 5.0–10.0
- 231 unit tests with 100% line and method coverage across all 13 target frameworks
- BenchmarkDotNet project for baseline performance measurement
- Full DocFX API documentation site
