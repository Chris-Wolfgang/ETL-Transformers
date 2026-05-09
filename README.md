# Wolfgang.Etl.Transformers

A collection of generic, broadly reusable transformers for use in ETL pipelines built on [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions).

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Multi--Targeted-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/ETL-Transformers)

---

## 📦 Installation

```bash
dotnet add package Wolfgang.Etl.Transformers
```

**NuGet Package:** Coming soon to NuGet.org

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 📚 Documentation

- **GitHub Repository:** [https://github.com/Chris-Wolfgang/ETL-Transformers](https://github.com/Chris-Wolfgang/ETL-Transformers)
- **API Documentation:** https://Chris-Wolfgang.github.io/ETL-Transformers/
- **Formatting Guide:** [README-FORMATTING.md](README-FORMATTING.md)
- **Contributing Guide:** [CONTRIBUTING.md](CONTRIBUTING.md)

---

## 🚀 Quick Start

Compose any number of transformers into a single pipeline with `.Then(...)`:

```csharp
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.Transformers;

// Three small, single-purpose transformers...
ITransformAsync<string, string> stripBlanks = new WhereTransformer<string>
(
    line => !string.IsNullOrWhiteSpace(line)
);

ITransformAsync<string, Order> parse = new SelectTransformer<string, Order>
(
    line => Order.Parse(line)
);

ITransformAsync<Order, Order> keepValid = new WhereTransformer<Order>
(
    o => o.Quantity > 0 && o.Price > 0m
);

// ...composed into one with full type inference at every .Then(...).
var pipeline = stripBlanks.Then(parse).Then(keepValid);
// pipeline : ITransformAsync<string, Order>

// Wire it between an extractor and a loader (TestKit doubles shown for brevity).
await loader.LoadAsync
(
    pipeline.TransformAsync(extractor.ExtractAsync(token)),
    token
);
```

Each transformer is a small `sealed` class implementing `ITransformAsync<,>` directly. No base-class inheritance, no per-item allocations, no surprises.

---

## ✨ Features

| Transformer | Purpose |
|---|---|
| **`PassThroughTransformer<T>`** | Yields each item unchanged. Useful as a placeholder, DI default, or test seam. Implements `ITransformWithCancellationAsync<T, T>`. |
| **`SelectTransformer<TSource, TDestination>`** | LINQ `Select` — projects each item via a sync or async selector. |
| **`WhereTransformer<T>`** | LINQ `Where` — filters items by sync or async predicate. |
| **`SelectManyTransformer<TSource, TDestination>`** | LINQ `SelectMany` — flattens a one-to-many projection (sync `IEnumerable<T>` or async `IAsyncEnumerable<T>` selector). |
| **`OfTypeTransformer<TSource, TDestination>`** | LINQ `OfType` — silently filters items that are not of the destination type. |
| **`CastTransformer<TSource, TDestination>`** | LINQ `Cast` — casts every item; throws `InvalidCastException` on mismatch. |
| **`TakeTransformer<T>`** | LINQ `Take(int)` — yields up to N items, then stops without enumerating further. |
| **`TakeWhileTransformer<T>`** | LINQ `TakeWhile` — yields items until the predicate first returns false. |
| **`SkipTransformer<T>`** | LINQ `Skip(int)` — discards the first N items. Returns the source directly when `count <= 0`. |
| **`SkipWhileTransformer<T>`** | LINQ `SkipWhile` — skips items while the predicate is true; predicate stops being called once it first returns false. |
| **`DistinctTransformer<T>`** | LINQ `Distinct` — yields each unique item, optional `IEqualityComparer<T>`. |
| **`DistinctByTransformer<TSource, TKey>`** | LINQ `DistinctBy` — yields each item with a unique key as projected by a sync key selector. |
| **`ChunkTransformer<T>`** | LINQ `Chunk(int)` — batches the input into right-sized `T[]` arrays. Last chunk may be smaller. |
| **`BufferedTransformer<T>`** | Decouples upstream and downstream stages via a bounded `System.Threading.Channels.Channel<T>` — enables pipeline parallelism, throughput approaches `max(stage speeds)` instead of `min`. |
| **`ChainTransformer<TSource, TIntermediate, TDestination>`** | Composes two transformers into one. Build any-N pipelines via the `.Then(...)` extension. |
| **`ChainTransformerWithCancellation<TSource, TIntermediate, TDestination>`** | Same as `ChainTransformer`, but for two `ITransformWithCancellationAsync<,>` transformers — propagates a single `CancellationToken` to both stages. |
| **`TransformerExtensions.Then(...)`** | Extension on `ITransformAsync<,>` (and a more-specific overload on `ITransformWithCancellationAsync<,>`) that composes two transformers into a single one with full type inference. Chain N stages by chaining `.Then(...).Then(...)`. |

### Design notes

- **Lightweight by design.** Every transformer implements `ITransformAsync<,>` (or `ITransformWithCancellationAsync<,>`) directly — none inherit from `TransformerBase<,,>`. Benchmarks in [`benchmarks/`](benchmarks/) measured a 14–27% throughput cost from the base class and motivated the lightweight design across the library.
- **`sealed` everywhere.** No virtual dispatch in the hot loop.
- **Sync + async lambdas.** Transformers that take a predicate or selector ship two constructors — sync `Func<...>` and async `Func<..., ValueTask<...>>` — with separate private iterator methods, so the hot loop never branches on which kind of lambda was supplied.
- **No progress, no Skip/Max counters.** Compose with dedicated transformers (`SkipTransformer`, `TakeTransformer`, future `ProgressReportingTransformer`) when those concerns are needed.

**Examples:**

The [`examples/`](examples/) folder contains runnable console projects for the most common scenarios:

| Example | Description |
|---|---|
| [BasicChain](examples/BasicChain) | Three transformers (`Where → Select → Where`) composed via `.Then(...)` into a single `ITransformAsync<string, Order>`. |
| [LinqOps](examples/LinqOps) | Tour of the LINQ-style transformers: `Where → Distinct → Select → Take → Chunk` in one chain. |
| [BufferedPipeline](examples/BufferedPipeline) | Demonstrates *why* `BufferedTransformer<T>` exists. Slow source + slow sink with and without a buffer; prints wall-clock comparison and observed speedup. |

---

## 🎯 Target Frameworks

The source library targets the same TFM set as `Wolfgang.Etl.Abstractions` 0.12.0:

| Framework | Versions |
|-----------|----------|
| .NET Framework | .NET 4.6.2, .NET 4.7.2, .NET 4.8, .NET 4.8.1 |
| .NET Standard | .NET Standard 2.0 |
| .NET | .NET 5.0, .NET 6.0, .NET 7.0, .NET 8.0, .NET 9.0, .NET 10.0 |

---

## 🔍 Code Quality & Static Analysis

This project enforces **strict code quality standards** through **7 specialized analyzers** and custom async-first rules:

### Analyzers in Use

1. **Microsoft.CodeAnalysis.NetAnalyzers** - Built-in .NET analyzers for correctness and performance
2. **Roslynator.Analyzers** - Advanced refactoring and code quality rules
3. **AsyncFixer** - Async/await best practices and anti-pattern detection
4. **Microsoft.VisualStudio.Threading.Analyzers** - Thread safety and async patterns
5. **Microsoft.CodeAnalysis.BannedApiAnalyzers** - Prevents usage of banned synchronous APIs
6. **Meziantou.Analyzer** - Comprehensive code quality rules
7. **SonarAnalyzer.CSharp** - Industry-standard code analysis

### Async-First Enforcement

This library uses **`BannedSymbols.txt`** to prohibit synchronous APIs and enforce async-first patterns:

**Blocked APIs Include:**
- ❌ `Task.Wait()`, `Task.Result` - Use `await` instead
- ❌ `Thread.Sleep()` - Use `await Task.Delay()` instead
- ❌ Synchronous file I/O (`File.ReadAllText`) - Use async versions
- ❌ Synchronous stream operations - Use `ReadAsync()`, `WriteAsync()`
- ❌ `Parallel.For/ForEach` - Use `Task.WhenAll()` or `Parallel.ForEachAsync()`
- ❌ Obsolete APIs (`WebClient`, `BinaryFormatter`)

**Why?** To ensure all code is **truly async** and **non-blocking** for optimal performance in async contexts.

---

## 🛠️ Building from Source

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- Optional: [PowerShell Core](https://github.com/PowerShell/PowerShell) for formatting scripts

### Build Steps

```bash
# Clone the repository
git clone https://github.com/Chris-Wolfgang/ETL-Transformers.git
cd ETL-Transformers

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Run code formatting (PowerShell Core)
pwsh ./format.ps1
```

### Code Formatting

This project uses `.editorconfig` and `dotnet format`:

```bash
# Format code
dotnet format

# Verify formatting (as CI does)
dotnet format --verify-no-changes
```

See [README-FORMATTING.md](README-FORMATTING.md) for detailed formatting guidelines.

### Building Documentation

This project uses [DocFX](https://dotnet.github.io/docfx/) to generate API documentation:

```bash
# Install DocFX (one-time setup)
dotnet tool install -g docfx

# Generate API metadata and build documentation
cd docfx_project
docfx metadata  # Extract API metadata from source code
docfx build     # Build HTML documentation

# Documentation is generated in the docs/ folder at the repository root
```

The documentation is automatically built and deployed to GitHub Pages when changes are pushed to the `main` branch.

**Local Preview:**
```bash
# Serve documentation locally (with live reload)
cd docfx_project
docfx build --serve

# Open http://localhost:8080 in your browser
```

**Documentation Structure:**
- `docfx_project/` - DocFX configuration and source files
- `docs/` - Generated HTML documentation (published to GitHub Pages)
- `docfx_project/index.md` - Main landing page content
- `docfx_project/docs/` - Additional documentation articles
- `docfx_project/api/` - Auto-generated API reference YAML files

---

## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Code quality standards
- Build and test instructions
- Pull request guidelines
- Analyzer configuration details

---


## 🙏 Acknowledgments

- **[Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions)** — provides the `ITransformAsync<,>` and `ITransformWithCancellationAsync<,>` interfaces (plus `TransformerBase<,,>`, which the lightweight transformers in this library deliberately do not inherit from). Sets the `Microsoft.Bcl.AsyncInterfaces` floor for older TFMs.
- **[Wolfgang.Etl.TestKit](https://github.com/Chris-Wolfgang/ETL-TestKit)** — `TestExtractor<T>` and `TestLoader<T>` used by the example projects as in-memory pipeline endpoints.
- **[System.Threading.Channels](https://www.nuget.org/packages/System.Threading.Channels)** — bounded channel powering `BufferedTransformer<T>` for pipeline parallelism.
- **[BenchmarkDotNet](https://benchmarkdotnet.org/)** — used in [`benchmarks/`](benchmarks/) to compare the lightweight `WhereTransformer` against a `TransformerBase`-backed alternative; the resulting 14–27% throughput delta motivated the lightweight design across the library.

