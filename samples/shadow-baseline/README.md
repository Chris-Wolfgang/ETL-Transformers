# Shadow-testing golden baseline

The [`Shadow Testing`](../../.github/workflows/shadow.yaml) workflow compares each nightly
run of the [`ShadowWorkloads`](../ShadowWorkloads) samples against the BenchmarkDotNet
reports committed **in this directory**.

## Current baseline

The four `*-report-full-compressed.json` files were captured by the nightly run of
2026-11-08 on `main` @ `6a6e646` (ubuntu-latest, `--job short`), the same runner class and
BenchmarkDotNet job the gate itself uses.

## Why the directory must not be empty

The compare step treats a missing baseline as an error, not as "nothing to compare": a
gate that cannot run fails rather than passing silently. With no `*.json` here the nightly
run fails every day with `No BenchmarkDotNet baseline (*.json)`.

## Seeding / refreshing the baseline

1. Take a green nightly run (or trigger the workflow) and download its `shadow-results`
   artifact.
2. Copy the `*-report-full-compressed.json` files into this directory, replacing the old ones.
3. Commit them. The nightly run fails and files a tracking issue when a workload regresses
   beyond threshold (default: >20% slower or >50% more allocations).

Refresh the baseline deliberately whenever an intentional change shifts the numbers, and
capture it from the same CI runner class the gate runs on: absolute BenchmarkDotNet numbers
are environment-sensitive, and allocation deltas are the reliable signal on hosted runners.
