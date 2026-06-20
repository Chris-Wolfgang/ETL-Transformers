# Introduction

## Overview

**Wolfgang.Etl.Transformers** is a library of generic, composable transformers for ETL pipelines.
It builds on [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions) and
provides the middle tier of a pipeline — the T in ETL — through 18 public types covering filtering,
projection, fan-out, deduplication, batching, buffering, progress reporting, and composition.

## Key Features

- **LINQ-style operations**: `WhereTransformer`, `SelectTransformer`, `SelectManyTransformer`,
  `OfTypeTransformer`, `CastTransformer`, `DistinctTransformer`, `DistinctByTransformer`,
  `TakeTransformer`, `TakeWhileTransformer`, `SkipTransformer`, `SkipWhileTransformer`, `ChunkTransformer`
- **Pipeline infrastructure**: `BufferedTransformer` decouples producer from consumer using
  `System.Threading.Channels` so both stages run concurrently; `ProgressReportingTransformer`
  calls a callback per item without changing the stream
- **Fluent composition**: `.Then(next)` extension composes any two transformers into one,
  chains arbitrarily deep, and automatically picks the cancellation-aware overload when both
  sides implement `ITransformWithCancellationAsync<TSource, TDestination>`
- **Multi-TFM**: targets .NET Framework 4.6.2–4.8.1, .NET Core 3.1, and .NET 5.0–10.0
- **Zero allocations on hot paths**: all transformers yield items via `IAsyncEnumerable<T>` with
  no intermediate buffering beyond `BufferedTransformer`'s explicit channel

## Design Principles

- Every transformer implements `ITransformAsync<TSource, TDestination>` — a single-method
  interface (`TransformAsync(IAsyncEnumerable<TSource>) -> IAsyncEnumerable<TDestination>`)
  that composes naturally with `.Then(...)` and the ETL framework's extractor/loader contracts
- No base class required: transformers are sealed, self-contained classes with no inheritance
  hierarchy beyond the interface they implement
- Sync-blocking APIs are prohibited by `BannedSymbols.txt` — all I/O is async throughout

## Getting Help

- Check the [Getting Started](getting-started.md) guide
- Browse the [API Reference](../api/index.md)
- Visit the [GitHub repository](https://github.com/Chris-Wolfgang/ETL-Transformers)
- Open an issue on [GitHub Issues](https://github.com/Chris-Wolfgang/ETL-Transformers/issues)
