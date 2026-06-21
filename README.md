# Wolfgang.Etl.Transformers

A collection of generic, composable transformers for ETL pipelines built on [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions).

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Etl.Transformers.svg)](https://www.nuget.org/packages/Wolfgang.Etl.Transformers)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/ETL-Transformers)

---

## Installation

```bash
dotnet add package Wolfgang.Etl.Transformers
```

---

## Quick Start

```csharp
using Wolfgang.Etl.Transformers;

// Build a pipeline: parse → filter → project → buffer
var pipeline = new SelectTransformer<string, Order>(ParseOrder)
    .Then(new WhereTransformer<Order>(o => o.IsValid))
    .Then(new SelectTransformer<Order, InvoiceRow>(ToInvoice));

await foreach (var row in pipeline.TransformAsync(rawLines))
{
    await loader.LoadAsync(row);
}

// Or buffer inline between pipeline stages:
var buffered = rawLines.Buffered(capacity: 500);
var filtered = new WhereTransformer<Order>(o => o.IsValid);
var projected = new SelectTransformer<Order, InvoiceRow>(ToInvoice);

await foreach (var row in projected.TransformAsync(filtered.TransformAsync(buffered)))
{
    await loader.LoadAsync(row);
}
```

---

## Transformers

### LINQ-style

| Transformer | Description |
|-------------|-------------|
| `WhereTransformer<T>` | Filters items by a sync or async predicate |
| `SelectTransformer<TSource, TDestination>` | Projects each item with a sync or async selector |
| `SelectManyTransformer<TSource, TDestination>` | Fan-out: maps each item to a sequence (sync or async) |
| `OfTypeTransformer<TSource, TDestination>` | Passes only items that are assignable to `TDestination` |
| `CastTransformer<TSource, TDestination>` | Casts each item; throws on incompatible types |
| `DistinctTransformer<T>` | Deduplicates using the default or a supplied `IEqualityComparer<T>` |
| `DistinctByTransformer<TSource, TKey>` | Deduplicates by a key selector |
| `TakeTransformer<T>` | Yields only the first N items |
| `TakeWhileTransformer<T>` | Yields items while a predicate holds |
| `SkipTransformer<T>` | Skips the first N items |
| `SkipWhileTransformer<T>` | Skips items while a predicate holds |
| `ChunkTransformer<T>` | Batches items into fixed-size arrays |

### Pipeline infrastructure

| Transformer | Description |
|-------------|-------------|
| `PassThroughTransformer<T>` | Identity pass-through; also implements `ITransformWithCancellationAsync<T, T>` |
| `BufferedTransformer<T>` | Decouples producer from consumer via a `System.Threading.Channels` buffer |
| `ProgressReportingTransformer<T>` | Calls a sync or async callback per item without altering the stream |

### Composition

| Type / Method | Description |
|---------------|-------------|
| `ChainTransformer<TSource, TIntermediate, TDestination>` | Composes two `ITransformAsync` transformers into one |
| `ChainTransformerWithCancellation<TSource, TIntermediate, TDestination>` | Same as above but propagates `CancellationToken` through both stages |
| `TransformerExtensions.Then(...)` | Fluent composition — two overloads: one for `ITransformAsync` pairs, one for `ITransformWithCancellationAsync` pairs |
| `TransformerExtensions.Buffered(...)` | Inline buffer insertion — sugar for `new BufferedTransformer<T>(n).TransformAsync(source)` |

---

## Target Frameworks

| Framework | Versions |
|-----------|----------|
| .NET Framework | 4.6.2, 4.7.2, 4.8, 4.8.1 |
| .NET Standard | 2.0 |
| .NET | 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 |

---

## Code Quality & Static Analysis

This project enforces **strict code quality standards** through **7 specialized analyzers** and custom async-first rules:

### Analyzers in Use

1. **Microsoft.CodeAnalysis.NetAnalyzers** — Correctness, performance, and security rules
2. **Roslynator.Analyzers** — 500+ refactoring and code quality rules
3. **AsyncFixer** — Async/await best practices and anti-pattern detection
4. **Microsoft.VisualStudio.Threading.Analyzers** — Thread safety and async patterns
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** — Blocks synchronous APIs listed in `BannedSymbols.txt`
6. **Meziantou.Analyzer** — Comprehensive code quality checks
7. **SonarAnalyzer.CSharp** — Industry-standard code analysis

### Async-First Enforcement

This library prohibits synchronous blocking calls via `BannedSymbols.txt`:

```csharp
// ❌ Banned
task.Wait();
task.Result;
File.ReadAllText(path);
Thread.Sleep(1000);

// ✅ Required
await task;
await File.ReadAllTextAsync(path);
await Task.Delay(1000);
```

---

## Building from Source

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later (required to build the full TFM matrix)
- Optional: [PowerShell Core](https://github.com/PowerShell/PowerShell) for formatting scripts

### Build Steps

```bash
# Clone the repository
git clone https://github.com/Chris-Wolfgang/ETL-Transformers.git
cd ETL-Transformers

# Restore dependencies
dotnet restore

# Build (Release enforces all analyzers as errors)
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Format code
pwsh ./scripts/format.ps1
```

### Building Documentation

This project uses [DocFX](https://dotnet.github.io/docfx/) for API documentation:

```bash
dotnet tool install -g docfx
cd docfx_project
docfx build --serve
# Open http://localhost:8080
```

Documentation is automatically built and deployed to GitHub Pages when a GitHub Release is published.

**Documentation:** [https://Chris-Wolfgang.github.io/ETL-Transformers/](https://Chris-Wolfgang.github.io/ETL-Transformers/)

---

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for code quality standards, build instructions, and pull request guidelines.

---

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

Built on [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions) — the base class library providing `ExtractorBase<TSource, TProgress>`, `LoaderBase<TDestination, TProgress>`, and `TransformerBase<TSource, TDestination, TProgress>`.
