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

| Metric | Default fail threshold | Set in |
| --- | --- | --- |
| Mean time | more than **20%** slower | `TIME_THRESHOLD` env in the workflow |
| Allocated bytes | more than **50%** greater (or newly allocating from zero) | `ALLOC_THRESHOLD` env in the workflow |

Time on GitHub-hosted runners is noisy, so a double-digit time delta can be measurement
jitter rather than a real regression. **Allocation deltas are deterministic** and are the
reliable signal — a bump there almost always means a real change on the hot path.

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
  --out delta.md
```

The script is pure standard-library Python 3 — no jq or other dependency required.
