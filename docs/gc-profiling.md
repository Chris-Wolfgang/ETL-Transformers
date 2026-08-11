# Sustained-load GC / allocation profiling

BenchmarkDotNet (`benchmarks/`) measures single-method micro-performance. The
[`GC Profiling`](../.github/workflows/gc-profiling.yaml) workflow answers a different
question: **how does the library behave under continuous, long-running load?** — the gen0/1/2
promotion, LOH pressure, and allocation-rate characteristics that only appear in a
server/ETL process that runs for hours.

## Components

| Piece | Role |
| --- | --- |
| [`profiling/Wolfgang.Etl.Transformers.Profiling`](../profiling/Wolfgang.Etl.Transformers.Profiling) | Console harness that drives a representative transformer pipeline in a tight loop for a configurable duration (`--duration-seconds`, `--scenario mixed\|linq\|buffered`). Server GC enabled. |
| `dotnet-counters` | Collects `System.Runtime` GC counters to CSV over the whole run. |
| `dotnet-gcdump` | Heap snapshot near the end — inspect **top object types on the heap** offline. |
| `dotnet-trace` (`gc-verbose`) | Short GC trace — inspect **top allocation sites** (allocation-tick events) offline. |
| [`scripts/gc-profile-report.py`](../scripts/gc-profile-report.py) | Summarizes the counters CSV into headline metrics and gates against a baseline. |

EventPipe tools are used instead of Windows-only ETW/PerfView so the whole thing runs on a
hosted Linux runner. The `.gcdump` and `.nettrace` artifacts open in Visual Studio /
PerfView / `dotnet-trace report` for the "top allocation sites / top object types" drill-down.

## Metrics captured

`gen0/1/2 collections`, `gen2` & `LOH` heap high-water marks, `total allocated bytes` +
`allocation rate (B/s)`, `GC pause seconds`, `ThreadPool queue` high-water mark, and lock
contentions. The **gated** metrics (fail the build on regression past tolerance, default
25%) are: gen2 collections, total allocated bytes, allocation rate, gen2 heap max, LOH heap
max.

## The baseline

The gate compares against `profiling/gc-baseline.<scenario>.json`. **No baseline is committed
initially** — the first run is report-only and prints its metrics. Commit that run's
`gc-metrics.<scenario>.json` (from the workflow artifact) as
`profiling/gc-baseline.<scenario>.json` to arm the gate. Refresh it deliberately when an
intentional change shifts the numbers.

Baselines are runner-specific: capture them from the same CI environment the gate runs in,
not from a local machine (Server GC, core count, and OS all affect the absolute numbers).

## Running locally

```bash
dotnet build profiling/Wolfgang.Etl.Transformers.Profiling -c Release
exe=profiling/Wolfgang.Etl.Transformers.Profiling/bin/Release/net10.0/Wolfgang.Etl.Transformers.Profiling
"$exe" --duration-seconds 60 --scenario mixed &
dotnet-counters collect -p $! --format csv -o counters.csv --counters System.Runtime --duration 00:01:00
python3 scripts/gc-profile-report.py --csv counters.csv --out metrics.json --scenario mixed
```
