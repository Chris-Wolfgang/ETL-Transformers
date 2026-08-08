# Shadow-testing golden baseline

The [`Shadow Testing`](../../.github/workflows/shadow.yaml) workflow compares each nightly
run of the [`ShadowWorkloads`](../ShadowWorkloads) samples against the BenchmarkDotNet
reports committed **in this directory**.

## Seeding / refreshing the baseline

This directory ships empty on purpose — with no baseline, the nightly run is **report-only**
(nothing to compare against, so nothing regresses). To arm the gate:

1. Trigger the workflow (or take a green nightly run) and download its `shadow-results`
   artifact.
2. Copy the `*-report-full-compressed.json` files into this directory.
3. Commit them. From then on the nightly run fails and files a tracking issue when a
   workload regresses beyond threshold (default: >20% slower or >50% more allocations).

Refresh the baseline deliberately whenever an intentional change shifts the numbers —
ideally capturing it from the same CI runner class the gate runs on, since absolute
BenchmarkDotNet numbers are environment-sensitive.
