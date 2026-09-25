# Per-PR benchmark regression gate

`.github/workflows/pr-benchmarks.yaml` is the **forward-looking** perf signal: it
compares a pull request's benchmarks against its own base commit and blocks a merge
that regresses a hot path. It complements
[`benchmarks.yaml`](../.github/workflows/benchmarks.yaml), which graphs the
*backward-looking* trend on `main` after a merge.

## What it does

1. Triggers only when a PR changes `src/**` or `benchmarks/**`.
2. Runs the BenchmarkDotNet suite (`--job short --memory`) twice: on the PR head, and
   on the PR's base commit (checked out into an isolated `git worktree`).
3. Runs [`scripts/compare-benchmarks.py`](../scripts/compare-benchmarks.py), which joins
   the two runs by benchmark `FullName` and computes the per-benchmark time and
   allocation delta.
4. Posts a **sticky** comment (replaced, not appended, on every push) with a delta table.
5. **Fails the PR** if any benchmark exceeds the threshold — unless the PR carries the
   `perf-impact-acknowledged` label.

## Thresholds

A benchmark must clear **both** a percentage and an absolute floor to fail the gate. A percentage
on its own cannot separate a real regression from runner noise on a fast benchmark.

| Metric | Percentage | Absolute floor | Set in |
| --- | --- | --- | --- |
| Mean time | more than **20%** slower | **and** at least **15000 ns** slower | `TIME_THRESHOLD` / `TIME_FLOOR_NS` env in the workflow |
| Allocated bytes | more than **50%** greater (or newly allocating from zero) | **and** at least **128 B** greater | `ALLOC_THRESHOLD` / `ALLOC_FLOOR_BYTES` env in the workflow |

Both floors are inclusive: a regression of exactly 15000 ns or exactly 128 B does fail.

Time on GitHub-hosted runners is noisy, so a double-digit time delta can be measurement
jitter rather than a real regression. **Allocation deltas are deterministic** and are the
reliable signal — a bump there almost always means a real change on the hot path.

The floors are sized from this repo's own history rather than copied from elsewhere. Across 90
benchmark comparisons on three pull requests that changed no runtime code, the median time delta
was 0.3% and the p90 was 5.1% — but one 47 us benchmark moved +18.5%, an absolute move of only
8.7 us. `TIME_FLOOR_NS` is about 1.7x that worst observed excursion. Allocations are far more
deterministic: the largest delta across the same 90 comparisons was 25 B on a ~3 KB baseline, so
`ALLOC_FLOOR_BYTES` only filters a percentage blow-up on a tiny baseline — 120 B to 200 B is
+67%, which trips the percentage test, but its 80 B delta is under the floor — and it sits far
below any real leak.

Only benchmarks present in *both* runs are gated. Benchmarks added or removed by the PR are
listed in the comment but never trip the gate.

## Overriding an intentional regression

Some changes trade throughput or allocations for correctness or a new feature. When that
trade is deliberate, add the **`perf-impact-acknowledged`** label to the PR. The gate then
reports the regression as a warning instead of failing. Run
[`scripts/Setup-Labels.ps1`](../scripts/Setup-Labels.ps1) once per repo to create the label.

## Local dry run

You can reproduce the comparison locally against any two BenchmarkDotNet result
directories (each containing the `*-report-full-compressed.json` files a run emits):

```bash
python scripts/compare-benchmarks.py \
  --base-dir path/to/base/BenchmarkDotNet.Artifacts/results \
  --head-dir path/to/head/BenchmarkDotNet.Artifacts/results \
  --time-threshold 20 --alloc-threshold 50 \
  --time-floor-ns 15000 --alloc-floor-bytes 128 \
  --out delta.md
```

The script is pure standard-library Python 3 — no jq or other dependency required.
