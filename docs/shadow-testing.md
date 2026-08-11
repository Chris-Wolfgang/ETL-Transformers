# Shadow testing

Unit and property tests prove the behavioural contract under *synthetic* input. Shadow
testing replays **realistic production traffic shapes** against each new commit and compares
latency/allocation against a golden baseline, catching performance and allocation
regressions that only surface under real usage.

## The sample workloads

[`samples/ShadowWorkloads`](../samples/ShadowWorkloads) is a BenchmarkDotNet consumer that
models how the transformers are actually used. Each scenario doubles as runnable "real usage"
documentation:

| Scenario | Shape it models | Transformers exercised |
| --- | --- | --- |
| `VariableSizeStreamingBenchmarks` | SQL/HTTP paging with wildly variable page sizes | `Where`, `Select`, `Chunk` |
| `ConcurrentEnumerationBenchmarks` | A server enumerating many pipelines in parallel | `Buffered`, `Where` |
| `MixedSourceBenchmarks` | Enrichment stage over a mixed sync/async source | `Select` (async), `SelectMany`, `Distinct` |
| `BurstyWindowBenchmarks` | Bounded take with skip/backoff and an observing tap | `SkipWhile`, `Skip`, `Take`, `ProgressReporting` |

Run them locally with:

```bash
dotnet run -c Release --project samples/ShadowWorkloads -- --filter "*" --job short --memory
```

## The nightly workflow

[`shadow.yaml`](../.github/workflows/shadow.yaml) runs nightly (and on manual dispatch):

1. Runs every workload with BenchmarkDotNet (`--memory` for GC/allocation stats).
2. Compares the results against the committed golden baseline
   ([`samples/shadow-baseline`](../samples/shadow-baseline)) via
   [`scripts/compare-benchmarks.py`](../scripts/compare-benchmarks.py).
3. If any workload regresses beyond threshold (default **>20% slower** or **>50% more
   allocations**, configurable per run via workflow inputs), it **opens or updates a tracking
   issue** and **fails the run**.

Until a baseline is committed, the run is report-only. See
[`samples/shadow-baseline/README.md`](../samples/shadow-baseline/README.md) for how to seed
and refresh it.

## Relationship to the other perf workflows

| Workflow | When | What |
| --- | --- | --- |
| `benchmarks.yaml` | push to `main` | Curated micro-perf trend chart |
| `pr-benchmarks.yaml` | per PR | Gate PR head vs its base |
| **`shadow.yaml`** | nightly | Replay realistic consumer workloads vs a golden baseline |
| `gc-profiling.yaml` | weekly | Sustained-load GC/allocation profile |
