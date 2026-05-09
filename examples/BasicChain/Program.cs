// ---------------------------------------------------------------------------
// BasicChain Example
// ---------------------------------------------------------------------------
//
// This example demonstrates the most common use of Wolfgang.Etl.Transformers:
// composing two or more transformers into a single ITransformAsync that an
// extractor and loader can be wired up to.
//
// Key concepts covered:
//   - WhereTransformer<T>           filter by predicate
//   - SelectTransformer<TS, TD>     project each item
//   - .Then(...) extension method   chain transformers with full type inference
//   - ITransformAsync<,> as the unifying contract for any composition
//
// The pipeline:
//
//   raw line                          (string)
//      |
//      |  Where:  drop blank/comment lines
//      v
//   raw line                          (string, non-empty)
//      |
//      |  Select: parse "id|name|qty|price" into an Order record
//      v
//   Order                             (Order)
//      |
//      |  Where:  drop orders with quantity <= 0 or price <= 0
//      v
//   Order                             (valid Order)
//
// Each .Then() returns a new ChainTransformer with full type inference.
// The end result is a single ITransformAsync<string, Order> that the
// loader treats as one transformer - it has no idea three separate
// stages live behind it.
// ---------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Etl.Abstractions;
using Wolfgang.Etl.TestKit;
using Wolfgang.Etl.Transformers;

// ---------------------------------------------------------------------------
// Step 1: Sample input data.
//
// In a real ETL the source would be a FixedWidthExtractor / DbClient /
// HTTP source / etc. Here a plain in-memory list keeps the example focused
// on the transformer composition.
// ---------------------------------------------------------------------------

var rawLines = new[]
{
    "ORD-001|widget|2|9.99",
    "",                              // blank line - filtered by stage 1
    "ORD-002|gadget|1|19.99",
    "# this is a comment",           // comment line - filtered by stage 1
    "ORD-003|sprocket|0|4.99",       // qty 0 - filtered by stage 3
    "ORD-004|gizmo|3|14.99",
    "",
    "ORD-005|thingamajig|1|-5.00",   // negative price - filtered by stage 3
    "ORD-006|doohickey|10|0.50",
};

// ---------------------------------------------------------------------------
// Step 2: Build the three transformer stages.
// ---------------------------------------------------------------------------

ITransformAsync<string, string> stripBlanks = new WhereTransformer<string>
(
    line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#", StringComparison.Ordinal)
);

ITransformAsync<string, Order> parse = new SelectTransformer<string, Order>(line =>
{
    var parts = line.Split('|');
    return new Order
    (
        Id: parts[0],
        Name: parts[1],
        Quantity: int.Parse(parts[2], CultureInfo.InvariantCulture),
        Price: decimal.Parse(parts[3], CultureInfo.InvariantCulture)
    );
});

ITransformAsync<Order, Order> keepValid = new WhereTransformer<Order>
(
    order => order.Quantity > 0 && order.Price > 0m
);

// ---------------------------------------------------------------------------
// Step 3: Compose the three stages with .Then(...) into a single transformer.
//
// The C# compiler infers all type parameters from the call:
//   stripBlanks  : ITransformAsync<string, string>
//   parse        : ITransformAsync<string, Order>
//   keepValid    : ITransformAsync<Order, Order>
//   pipeline     : ITransformAsync<string, Order>
// ---------------------------------------------------------------------------

var pipeline = stripBlanks
    .Then(parse)
    .Then(keepValid);

// ---------------------------------------------------------------------------
// Step 4: Wire the pipeline between an extractor and a loader.
//
// TestExtractor and TestLoader come from Wolfgang.Etl.TestKit and stand in
// for the real source/sink in production code.
// ---------------------------------------------------------------------------

var extractor = new TestExtractor<string>(rawLines);
var loader = new TestLoader<Order>(collectItems: true);
var token = CancellationToken.None;

await loader.LoadAsync
(
    pipeline.TransformAsync(extractor.ExtractAsync(token)),
    token
);

// ---------------------------------------------------------------------------
// Step 5: Inspect the results.
// ---------------------------------------------------------------------------

Console.WriteLine("Input lines:");
foreach (var line in rawLines)
{
    Console.WriteLine($"  {(string.IsNullOrEmpty(line) ? "<blank>" : line)}");
}

Console.WriteLine();
Console.WriteLine("Valid orders that flowed through the pipeline:");
Console.WriteLine(new string('-', 60));

foreach (var order in loader.GetCollectedItems()!)
{
    Console.WriteLine
    (
        $"  {order.Id}  {order.Name,-12}  qty={order.Quantity,2}  price={order.Price,7:0.00}"
    );
}

Console.WriteLine(new string('-', 60));
Console.WriteLine
(
    $"In: {rawLines.Length} raw lines  ->  Out: {loader.GetCollectedItems()!.Count} valid orders"
);

// ---------------------------------------------------------------------------
// Order - the projected type produced by the pipeline.
// ---------------------------------------------------------------------------

public sealed record Order(string Id, string Name, int Quantity, decimal Price);
