# Introduction

Welcome to **Wolfgang.Etl.Transformers**.

## Overview

A collection of generic, broadly reusable transformers for use in ETL pipelines built on [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions). Each transformer is a small `sealed` class implementing `ITransformAsync<,>` (or `ITransformWithCancellationAsync<,>`) directly &mdash; no base-class inheritance, no per-item allocations, no surprises. Compose any number of them into a single pipeline with the `.Then(...)` extension.

## Key features

The library is grouped into three categories:

### LINQ-style transformers

| Transformer | Mirrors |
|---|---|
| `WhereTransformer<T>` | `Enumerable.Where` |
| `SelectTransformer<TSource, TDestination>` | `Enumerable.Select` |
| `SelectManyTransformer<TSource, TDestination>` | `Enumerable.SelectMany` |
| `OfTypeTransformer<TSource, TDestination>` | `Enumerable.OfType` |
| `CastTransformer<TSource, TDestination>` | `Enumerable.Cast` |
| `TakeTransformer<T>` | `Enumerable.Take(int)` |
| `TakeWhileTransformer<T>` | `Enumerable.TakeWhile` |
| `SkipTransformer<T>` | `Enumerable.Skip(int)` |
| `SkipWhileTransformer<T>` | `Enumerable.SkipWhile` |
| `DistinctTransformer<T>` | `Enumerable.Distinct` |
| `DistinctByTransformer<TSource, TKey>` | `Enumerable.DistinctBy` |
| `ChunkTransformer<T>` | `Enumerable.Chunk(int)` |

### Pipeline infrastructure

| Transformer | Purpose |
|---|---|
| `PassThroughTransformer<T>` | Yields each item unchanged. Useful as a placeholder, DI default, or test seam. |
| `BufferedTransformer<T>` | Decouples upstream and downstream stages via a bounded channel for pipeline parallelism — throughput approaches `max(stage speeds)` instead of `min`. |

### Composition

| Type | Purpose |
|---|---|
| `ChainTransformer<TSource, TIntermediate, TDestination>` | Combines two transformers into one. |
| `ChainTransformerWithCancellation<TSource, TIntermediate, TDestination>` | Same as above, but for two `ITransformWithCancellationAsync<,>` transformers — propagates a single `CancellationToken` to both stages. |
| `TransformerExtensions.Then(...)` | Extension on `ITransformAsync<,>` (and a more-specific overload on `ITransformWithCancellationAsync<,>`) that composes two transformers with full type inference. Chain N stages by chaining `.Then(...).Then(...)`. |

## Design notes

- **Lightweight by design.** Every transformer implements `ITransformAsync<,>` (or `ITransformWithCancellationAsync<,>`) directly &mdash; none inherit from `TransformerBase<,,>`. Benchmarks measured a 14&ndash;27% throughput cost from the base class and motivated the lightweight design library-wide.
- **`sealed` everywhere.** No virtual dispatch in the hot loop.
- **Sync + async lambdas.** Transformers that take a predicate or selector ship two constructors &mdash; sync `Func<...>` and async `Func<..., ValueTask<...>>` &mdash; with separate private iterator methods, so the hot loop never branches on which kind of lambda was supplied.
- **No progress, no Skip/Max counters.** Compose with dedicated transformers when those concerns are needed.

## Getting help

If you need help with Wolfgang.Etl.Transformers, please:

- Check the [Getting Started](getting-started.md) guide
- Review the [API Reference](../api/index.md) (auto-generated from source XML comments)
- Visit the [GitHub repository](https://github.com/Chris-Wolfgang/ETL-Transformers)
- Open an issue on [GitHub Issues](https://github.com/Chris-Wolfgang/ETL-Transformers/issues)
