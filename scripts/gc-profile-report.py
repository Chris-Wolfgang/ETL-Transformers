#!/usr/bin/env python3
"""Summarize a dotnet-counters GC CSV into headline metrics and gate against a baseline.

Parses the CSV produced by ``dotnet-counters collect --format csv`` (System.Runtime
meters) over a sustained-load run and derives the metrics that matter under long-running
ETL workloads: gen0/1/2 collection counts, gen2 & LOH heap high-water marks, total bytes
allocated + allocation rate, total GC pause time, ThreadPool queue high-water mark, and
lock-contention count.

Counter names carry the refresh interval in their text (``... / 5 sec``), so metrics are
matched by stable substrings rather than exact names.

Writes the metrics as JSON (``--out``). If ``--baseline`` points at an existing metrics
file, each gated metric is compared and the script exits non-zero when any exceeds
``baseline * (1 + tolerance)``. With no baseline (first run), it is report-only and prints
the measured metrics as the baseline to commit.

Used by ``.github/workflows/gc-profiling.yaml``. Pure standard library.
"""
from __future__ import annotations

import argparse
import csv
import json
import sys
from datetime import datetime

# Metric key -> (name substring, optional generation tag, aggregation)
# aggregation: "sum" for per-interval increments, "max" for snapshot gauges.
SPECS = {
    "gen0_collections": ("dotnet.gc.collections", "generation=gen0", "sum"),
    "gen1_collections": ("dotnet.gc.collections", "generation=gen1", "sum"),
    "gen2_collections": ("dotnet.gc.collections", "generation=gen2", "sum"),
    "total_allocated_bytes": ("dotnet.gc.heap.total_allocated", None, "sum"),
    "gc_pause_seconds": ("dotnet.gc.pause.time", None, "sum"),
    "lock_contentions": ("dotnet.monitor.lock_contentions", None, "sum"),
    "gen2_heap_max_bytes": ("dotnet.gc.last_collection.heap.size", "generation=gen2", "max"),
    "loh_heap_max_bytes": ("dotnet.gc.last_collection.heap.size", "generation=loh", "max"),
    "threadpool_queue_max": ("dotnet.thread_pool.queue.length", None, "max"),
    "working_set_max_bytes": ("dotnet.process.memory.working_set", None, "max"),
}

# Metrics that fail the build when they regress past the baseline. Heap/allocation growth
# and extra collections are real regressions; queue depth and working set are informational.
GATED = {
    "gen2_collections",
    "total_allocated_bytes",
    "allocation_rate_bytes_per_sec",
    "gen2_heap_max_bytes",
    "loh_heap_max_bytes",
}


def parse(csv_path: str) -> dict[str, float]:
    sums: dict[str, float] = {k: 0.0 for k, v in SPECS.items() if v[2] == "sum"}
    maxes: dict[str, float] = {k: 0.0 for k, v in SPECS.items() if v[2] == "max"}
    timestamps: list[datetime] = []

    with open(csv_path, encoding="utf-8-sig", newline="") as handle:
        reader = csv.reader(handle)
        header = next(reader, None)
        for row in reader:
            if len(row) < 5:
                continue
            ts_raw, _provider, name, _ctype, value_raw = row[0], row[1], row[2], row[3], row[4]
            try:
                value = float(value_raw)
            except ValueError:
                continue
            try:
                timestamps.append(datetime.strptime(ts_raw, "%m/%d/%Y %H:%M:%S"))
            except ValueError:
                pass
            for key, (substr, gen, agg) in SPECS.items():
                if substr in name and (gen is None or gen in name):
                    if agg == "sum":
                        sums[key] += value
                    else:
                        maxes[key] = max(maxes[key], value)

    metrics: dict[str, float] = {}
    metrics.update({k: round(v, 3) for k, v in sums.items()})
    metrics.update({k: round(v, 3) for k, v in maxes.items()})

    duration = 0.0
    if len(timestamps) >= 2:
        duration = (max(timestamps) - min(timestamps)).total_seconds()
    metrics["duration_seconds"] = round(duration, 1)
    metrics["allocation_rate_bytes_per_sec"] = round(
        metrics["total_allocated_bytes"] / duration if duration > 0 else 0.0, 1
    )
    return metrics


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--csv", required=True, help="dotnet-counters CSV path")
    parser.add_argument("--out", required=True, help="metrics JSON output path")
    parser.add_argument("--baseline", default="", help="baseline metrics JSON to gate against")
    parser.add_argument("--scenario", default="", help="scenario label recorded in the output")
    parser.add_argument("--tolerance", type=float, default=0.25, help="allowed fractional growth")
    args = parser.parse_args()

    metrics = parse(args.csv)
    if args.scenario:
        metrics["scenario"] = args.scenario

    with open(args.out, "w", encoding="utf-8", newline="\n") as handle:
        json.dump(metrics, handle, indent=2)
        handle.write("\n")

    print(f"=== GC profile metrics ({args.scenario or 'run'}) ===")
    for key, value in metrics.items():
        print(f"  {key}: {value}")

    baseline = None
    if args.baseline:
        try:
            with open(args.baseline, encoding="utf-8") as handle:
                baseline = json.load(handle)
        except FileNotFoundError:
            baseline = None

    if not baseline:
        print("\nNo baseline present - report-only. Commit the metrics above as the baseline.")
        return 0

    print(f"\n=== Regression gate (tolerance {args.tolerance:.0%}) ===")
    regressed = []
    for key in sorted(GATED):
        base = baseline.get(key)
        current = metrics.get(key)
        if base is None or current is None:
            continue
        limit = base * (1 + args.tolerance)
        status = "OK"
        if base > 0 and current > limit:
            status = "REGRESSED"
            regressed.append((key, base, current, limit))
        elif base == 0 and current > 0:
            status = "REGRESSED"
            regressed.append((key, base, current, 0))
        print(f"  {key}: base={base} current={current} limit={round(limit, 1)} -> {status}")

    if regressed:
        print("\n::error::GC profile regressed vs baseline:")
        for key, base, current, limit in regressed:
            print(f"::error::  {key}: {current} exceeds limit {round(limit, 1)} (baseline {base})")
        return 1

    print("\nAll gated metrics within tolerance.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
