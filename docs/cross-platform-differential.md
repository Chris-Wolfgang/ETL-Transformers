# Cross-platform / multi-arch differential

`pr.yaml` already runs the test suite on Linux, Windows, and macOS, but it only checks
that the suite *passes* on each. The
[`cross-platform-differential.yaml`](../.github/workflows/cross-platform-differential.yaml)
workflow goes further: it verifies the suite passes **the same way** on every OS *and*
architecture, and it adds ARM64 coverage that `pr.yaml` lacks.

## What it does

1. Runs the unit test project (`net10.0`, the one framework available on all four runners)
   on:
   - **linux-x64** — `ubuntu-latest`
   - **linux-arm64** — `ubuntu-24.04-arm`
   - **macos-arm64** — `macos-latest` (Apple Silicon)
   - **windows-x64** — `windows-latest`
2. Each job uploads its raw `.trx`.
3. A compare job normalizes every run to a stable `outcome<TAB>testName` form
   ([`scripts/normalize-test-results.py`](../scripts/normalize-test-results.py)) and diffs
   each platform against `linux-x64`.
4. **Any divergence fails the workflow** with the offending diff.

It triggers on PRs that change `src/**` or `tests/**`, and on manual dispatch.

## What a divergence looks like

A test that passes on x64 but fails on ARM64, or a theory whose parameters render
differently under a culture-sensitive conversion (e.g. a number or date formatted with the
current culture), produces a differing line and fails the run. These are exactly the subtle
bugs a per-OS pass/fail gate misses.

## Platform-specific tests

If a test is *legitimately* platform-specific (path separators, file-system case
sensitivity, an OS-only API), tag it so the differential excludes it:

```csharp
[Fact]
[Trait("Category", "PlatformSpecific")]
public void Path_handling_uses_the_OS_separator() { /* ... */ }
```

The workflow runs with `--filter "Category!=PlatformSpecific"`, so tagged tests still run in
`pr.yaml` but never trip the differential.
