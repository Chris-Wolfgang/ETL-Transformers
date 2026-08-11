# Concurrency / race-condition testing

Per-PR tests run one logical thread. Production use of an async library hits real schedule
interleavings — concurrent consumers, cancellation arriving mid-`await`, a background
producer racing its consumer. This project stress-tests those paths two ways.

## The scenarios

[`tests/Wolfgang.Etl.Transformers.Tests.Concurrency`](../tests/Wolfgang.Etl.Transformers.Tests.Concurrency)
defines the concurrency scenarios once (in `Scenarios.cs`), each asserting an invariant with
`Microsoft.Coyote.Specifications.Specification.Assert`:

| Scenario | Interleaving it stresses |
| --- | --- |
| `TwoConcurrentEnumerationsAsync` | One transformer instance enumerated by two tasks at once — each `TransformAsync` must yield an independent sequence. |
| `BufferedProducerConsumerAsync` | `BufferedTransformer`'s background producer racing the consumer over a bounded channel — every item once, no deadlock. |
| `CancellationDuringEnumerationAsync` | Cancellation arriving from another task mid-enumeration — must terminate, not hang. |

## Two layers of testing

The [`Concurrency (Coyote)`](../.github/workflows/concurrency.yaml) workflow runs weekly (and
on manual dispatch):

1. **Blocking stress gate.** `ConcurrencyStressTests` (xunit, `[Trait("Category","Concurrency")]`)
   runs each scenario many times on the real scheduler. A race that reproduces reasonably
   often fails the job. These also run in normal `dotnet test`.
2. **Coyote systematic exploration (non-blocking).** [Microsoft Coyote](https://github.com/microsoft/coyote)
   rewrites the assembly (`coyote rewrite`) and explores thousands of schedule interleavings
   per scenario (`coyote test -m <method> -i 10000`), surfacing races/deadlocks a million CI
   runs wouldn't.

### Why Coyote is non-blocking

Coyote 1.7 controls `Task`/`Channel` scheduling by rewriting IL, but its support for
`IAsyncEnumerable` / `await foreach`-heavy code — which is the entire shape of this library —
is still rough (async-iterator task-state transitions, framework-assembly version loading).
That produces exploratory findings that are often tool artifacts rather than real races, so
the workflow uploads Coyote's traces for triage instead of failing the build on them. The
**blocking** signal is the stress gate. Revisit gating on Coyote as its async-stream support
matures.

### net8.0

The project targets `net8.0`: the Coyote CLI runs on the .NET 8 runtime and cannot load a
net10 assembly. `rewrite.coyote.json` lists the test assembly and the library so both are
rewritten into Coyote's controlled runtime.

## Running locally

```bash
# Stress gate
dotnet test tests/Wolfgang.Etl.Transformers.Tests.Concurrency -c Release --filter "Category=Concurrency"

# Coyote exploration
dotnet tool install -g Microsoft.Coyote.CLI
cd tests/Wolfgang.Etl.Transformers.Tests.Concurrency
dotnet build -c Release
coyote rewrite rewrite.coyote.json
coyote test bin/Release/net8.0/Wolfgang.Etl.Transformers.Tests.Concurrency.dll \
  -m Wolfgang.Etl.Transformers.Tests.Concurrency.CoyoteEntryPoints.ConcurrentEnumerations -i 10000
```

## 24-hour soak (future — needs self-hosted infra)

The issue also calls for a monthly 24-hour soak that hammers the public API and watches for
handle/thread/memory leaks via `dotnet-counters`. That requires a **self-hosted runner** (a
24h job is impractical on hosted runners), which this repository does not yet have. When one
is provisioned, add a `soak.yaml` that loops the stress scenarios for 24h and snapshots
`dotnet-counters` (the [`gc-profiling`](gc-profiling.md) harness and report script are a ready
starting point for the leak-regression check).
