---
_layout: landing
---

# Wolfgang.Etl.Transformers Documentation

Welcome to the Wolfgang.Etl.Transformers documentation. This site contains comprehensive guides, API reference, and examples to help you get started.

## Quick Links

- [Getting Started](docs/getting-started.md) - Learn the basics
- [API Reference](xref:Wolfgang.Etl.Transformers) - Complete API documentation
- [GitHub Repository](https://github.com/Chris-Wolfgang/ETL-Transformers) - View source code

## About Wolfgang.Etl.Transformers

A collection of generic, broadly reusable transformers for use in ETL pipelines built on [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions).

The library ships a focused set of transformers covering the LINQ-style operators (`Where`, `Select`, `SelectMany`, `Distinct`, `Take`, `Skip`, `Chunk`, etc.), a `BufferedTransformer<T>` for pipeline parallelism, and `ChainTransformer` + `.Then(...)` extension methods for composing any number of transformers into one. Every transformer implements `ITransformAsync<,>` directly &mdash; no base-class inheritance &mdash; for minimal per-item overhead. See [Introduction](docs/introduction.md) for the complete list grouped by category.

## Installation

```bash
dotnet add package Wolfgang.Etl.Transformers
```

## Documentation Sections

### 📖 [Documentation](docs/getting-started.md)
Step-by-step guides and tutorials to help you use Wolfgang.Etl.Transformers effectively.

### 📚 [API Reference](xref:Wolfgang.Etl.Transformers)
Complete API documentation automatically generated from source code XML comments.

## Additional Resources

- [Contributing Guidelines](https://github.com/Chris-Wolfgang/ETL-Transformers/blob/main/CONTRIBUTING.md)
- [Code of Conduct](https://github.com/Chris-Wolfgang/ETL-Transformers/blob/main/CODE_OF_CONDUCT.md)
- [License](https://github.com/Chris-Wolfgang/ETL-Transformers/blob/main/LICENSE)

---

*Documentation built with [DocFX](https://dotnet.github.io/docfx/)*

