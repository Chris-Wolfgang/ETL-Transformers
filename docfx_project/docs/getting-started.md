# Getting Started

This guide will help you quickly get up and running with **Wolfgang.Etl.Transformers**.

## Prerequisites

- A .NET SDK that targets one of the supported frameworks: .NET Framework 4.6.2 / 4.7.2 / 4.8 / 4.8.1, .NET Standard 2.0, or .NET 5.0 &ndash; .NET 10.0
- An `ITransformAsync<,>` (or `ITransformWithCancellationAsync<,>`) source &mdash; typically an `ExtractorBase<,>` from a sibling library such as [Wolfgang.Etl.FixedWidth](https://github.com/Chris-Wolfgang/ETL-FixedWidth), or [Wolfgang.Etl.TestKit](https://github.com/Chris-Wolfgang/ETL-TestKit)'s `TestExtractor<T>` for getting started

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Etl.Transformers
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Etl.Transformers
```

## Quick Start

Compose three small transformers into a single pipeline with `.Then(...)`:

```csharp
using System.Threading;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.TestKit;
using Wolfgang.Etl.Transformers;

// Hypothetical domain type. Replace with your own.
public sealed record Order(int Quantity, decimal Price)
{
    public static Order Parse(string line)
    {
        var parts = line.Split(',');
        return new Order(int.Parse(parts[0]), decimal.Parse(parts[1]));
    }
}

// The input you would normally read from a file, database, or HTTP stream.
string[] rawLines =
{
    "2,9.99",
    "",
    "1,4.50",
    "0,12.00",
};

// 1. Build three small, single-purpose transformers.
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

// 2. Compose them with .Then(...) - all type parameters inferred.
var pipeline = stripBlanks.Then(parse).Then(keepValid);
// pipeline : ITransformAsync<string, Order>

// 3. Wire the pipeline between an extractor and a loader (TestKit doubles
//    here for brevity; in production these would be your real source/sink).
var extractor = new TestExtractor<string>(rawLines);
var loader    = new TestLoader<Order>(collectItems: true);
var token     = CancellationToken.None;

await loader.LoadAsync
(
    pipeline.TransformAsync(extractor.ExtractAsync(token)),
    token
);
```

For a fully runnable version of this snippet (and two more), see the [`examples/` folder](https://github.com/Chris-Wolfgang/ETL-Transformers/tree/main/examples) in the repository.

## Common patterns

### Adding cancellation

Every transformer that supports cancellation implements `ITransformWithCancellationAsync<,>`. When you compose two cancellation-aware transformers with `.Then(...)`, the resulting chain implements `ITransformWithCancellationAsync<,>` automatically &mdash; the C# compiler picks the more-specific overload of `Then(...)`. A token passed to the chain propagates to both stages.

### Adding pipeline parallelism

If your pipeline has a slow source and a slow sink (e.g. database reads feeding database writes), insert a `BufferedTransformer<T>` between them. The producer and consumer then run concurrently &mdash; throughput approaches `max(stage speeds)` instead of `min`:

```csharp
var pipeline = parseRaw
    .Then(normalize)
    .Then(new BufferedTransformer<Row>(capacity: 500))   // parallelism boundary
    .Then(formatForLoad);
```

## Next Steps

- Read the [Introduction](introduction.md) for the full transformer catalog and design notes
- Explore the [API Reference](../api/index.md) for every type, method, and overload (auto-generated from source XML)
- Browse the [examples on GitHub](https://github.com/Chris-Wolfgang/ETL-Transformers/tree/main/examples) for runnable end-to-end pipelines

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/ETL-Transformers)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/ETL-Transformers/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/ETL-Transformers/issues)
