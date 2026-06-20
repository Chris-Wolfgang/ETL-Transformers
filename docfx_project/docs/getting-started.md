# Getting Started

This guide will help you quickly get up and running with Wolfgang.Etl.Transformers.

## Prerequisites

- .NET 5.0 or later (any TFM from .NET Framework 4.6.2 to .NET 10.0 is supported at runtime)
- .NET 10.0 SDK to build from source and run the full TFM matrix

## Installation

### Via .NET CLI

```bash
dotnet add package Wolfgang.Etl.Transformers
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Etl.Transformers
```

## Quick Start

### Basic filter + project pipeline

```csharp
using Wolfgang.Etl.Transformers;

ITransformAsync<string, InvoiceRow> pipeline =
    new SelectTransformer<string, Order>(ParseOrder)
        .Then(new WhereTransformer<Order>(o => o.IsValid))
        .Then(new SelectTransformer<Order, InvoiceRow>(ToInvoice));

await foreach (var row in pipeline.TransformAsync(rawLines))
{
    await loader.LoadAsync(row, token);
}
```

### Buffered producer–consumer

Insert `BufferedTransformer<T>` (or `.Buffered(n)`) to decouple a slow producer from a slow
consumer so both run concurrently:

```csharp
ITransformAsync<Order, InvoiceRow> pipeline =
    new SelectTransformer<Order, Order>(Normalize)
        .Then(new BufferedTransformer<Order>(capacity: 500))
        .Then(new SelectTransformer<Order, InvoiceRow>(ToInvoice));
```

### Progress reporting

Tap the stream at any point without changing the data:

```csharp
long count = 0;
var reporter = new ProgressReportingTransformer<Order>(_ =>
    progress.Report(Interlocked.Increment(ref count)));

ITransformAsync<string, InvoiceRow> pipeline =
    new SelectTransformer<string, Order>(ParseOrder)
        .Then(reporter)
        .Then(new SelectTransformer<Order, InvoiceRow>(ToInvoice));
```

## Next Steps

- Explore the [API Reference](../api/index.md) for detailed documentation on every transformer
- Check out example projects in the [GitHub repository](https://github.com/Chris-Wolfgang/ETL-Transformers/tree/main/examples)
- Read [CONTRIBUTING.md](https://github.com/Chris-Wolfgang/ETL-Transformers/blob/main/CONTRIBUTING.md) if you want to contribute

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/ETL-Transformers)
- [NuGet Package](https://www.nuget.org/packages/Wolfgang.Etl.Transformers)
- [Report an Issue](https://github.com/Chris-Wolfgang/ETL-Transformers/issues)
