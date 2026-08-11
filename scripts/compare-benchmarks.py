#!/usr/bin/env python3
"""Compare two BenchmarkDotNet runs (PR base vs head) and emit a delta table.

Reads the ``*-report-full-compressed.json`` files BenchmarkDotNet writes under each
run's ``BenchmarkDotNet.Artifacts/results`` directory, joins benchmarks by their
``FullName``, and produces:

* a markdown comment body (``--out``) with a per-benchmark time/allocation delta table,
* ``regressed=true|false`` on stdout for ``$GITHUB_OUTPUT``.

A benchmark is a regression when its mean time grows by more than ``--time-threshold``
percent OR its allocated bytes grow by more than ``--alloc-threshold`` percent. Only
benchmarks present in BOTH runs are gated; added/removed benchmarks are listed but never
trip the gate.

Used by ``.github/workflows/pr-benchmarks.yaml``. Pure standard library so it runs on any
runner with Python 3 and needs no jq.
"""
from __future__ import annotations

import argparse
import glob
import json
import os
import sys

# A benchmark that newly allocates from a zero baseline has no finite percentage; use a
# large sentinel so it always exceeds any sane allocation threshold and renders as "new".
NEW_ALLOC_SENTINEL = 1_000_000_000.0


def collect(results_dir: str) -> dict[str, dict[str, float]]:
    """Map FullName -> {mean, alloc} across every BDN report under results_dir."""
    out: dict[str, dict[str, float]] = {}
    pattern = os.path.join(results_dir, "**", "*-report-full-compressed.json")
    for path in sorted(glob.glob(pattern, recursive=True)):
        with open(path, encoding="utf-8-sig") as handle:
            doc = json.load(handle)
        for bench in doc.get("Benchmarks", []):
            name = bench.get("FullName")
            if not name:
                continue
            stats = bench.get("Statistics") or {}
            memory = bench.get("Memory") or {}
            out[name] = {
                "mean": float(stats.get("Mean", 0.0)),
                "alloc": float(memory.get("BytesAllocatedPerOperation", 0) or 0),
            }
    return out


def pct(base: float, head: float, is_alloc: bool) -> float:
    if base > 0:
        return (head - base) / base * 100.0
    if is_alloc and head > 0:
        return NEW_ALLOC_SENTINEL
    return 0.0


def fmt_pct(value: float) -> str:
    if value >= NEW_ALLOC_SENTINEL:
        return "new"
    sign = "+" if value > 0 else ""
    return f"{sign}{round(value, 1)}%"


def fmt_ns(value: float) -> str:
    return f"{round(value, 2)} ns"


def fmt_bytes(value: float) -> str:
    return f"{int(value)} B"


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--base-dir", required=True, help="results dir for the base run")
    parser.add_argument("--head-dir", required=True, help="results dir for the PR-head run")
    parser.add_argument("--time-threshold", type=float, required=True, help="max % slower")
    parser.add_argument("--alloc-threshold", type=float, required=True, help="max % more allocations")
    parser.add_argument("--out", required=True, help="markdown comment body output path")
    args = parser.parse_args()

    base = collect(args.base_dir)
    head = collect(args.head_dir)

    common = sorted(set(base) & set(head))
    added = sorted(set(head) - set(base))
    removed = sorted(set(base) - set(head))

    rows = []
    for name in common:
        b, h = base[name], head[name]
        rows.append(
            {
                "key": name,
                "b_mean": b["mean"],
                "h_mean": h["mean"],
                "b_alloc": b["alloc"],
                "h_alloc": h["alloc"],
                "t_pct": pct(b["mean"], h["mean"], is_alloc=False),
                "a_pct": pct(b["alloc"], h["alloc"], is_alloc=True),
            }
        )

    def is_regression(row: dict) -> bool:
        return row["t_pct"] > args.time_threshold or row["a_pct"] > args.alloc_threshold

    regressions = [r for r in rows if is_regression(r)]

    lines = [
        "### 📊 Benchmark delta — PR vs base",
        "",
        (
            f"Threshold: fail if **time > {args.time_threshold:g}%** slower or "
            f"**allocations > {args.alloc_threshold:g}%** greater (per benchmark). "
            "Time on hosted runners is noisy; allocation deltas are the reliable signal."
        ),
        "",
    ]

    if not rows:
        lines.append("_No benchmarks are present in both base and head; nothing to compare._")
    else:
        lines.append("| Benchmark | Time (base → head) | Δ time | Alloc (base → head) | Δ alloc |")
        lines.append("|---|---|---|---|---|")
        # Worst offenders first: allocation regressions dominate, then time.
        for row in sorted(rows, key=lambda r: (-min(r["a_pct"], NEW_ALLOC_SENTINEL), -r["t_pct"])):
            flag = "⚠️ " if is_regression(row) else ""
            lines.append(
                f"| {flag}`{row['key']}` "
                f"| {fmt_ns(row['b_mean'])} → {fmt_ns(row['h_mean'])} | {fmt_pct(row['t_pct'])} "
                f"| {fmt_bytes(row['b_alloc'])} → {fmt_bytes(row['h_alloc'])} | {fmt_pct(row['a_pct'])} |"
            )

    if added:
        lines += ["", "**New benchmarks (not gated):** " + ", ".join(f"`{a}`" for a in added)]
    if removed:
        lines += ["", "**Removed benchmarks:** " + ", ".join(f"`{r}`" for r in removed)]

    lines.append("")
    if regressions:
        lines.append(
            f"⚠️ **{len(regressions)} benchmark(s) exceed the threshold.** "
            "Investigate, or add the `perf-impact-acknowledged` label if the change is intentional."
        )
    else:
        lines.append("✅ No benchmark exceeds the threshold.")

    with open(args.out, "w", encoding="utf-8", newline="\n") as handle:
        handle.write("\n".join(lines) + "\n")

    print(f"regressed={'true' if regressions else 'false'}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
