# Copilot Coding Agent Instructions

## Repository Summary

**Wolfgang.Etl.Transformers** is a shipped NuGet library of generic, composable transformers for ETL
pipelines built on [Wolfgang.Etl.Abstractions](https://github.com/Chris-Wolfgang/ETL-Abstractions):
LINQ-style operators (`Where`, `Select`, `SelectMany`, `Distinct`, `Chunk`, ...), pipeline
infrastructure (`BufferedTransformer`, `ProgressReportingTransformer`, `ThrottleTransformer`),
composition (`ChainTransformer`, `.Then(...)`), `IEtlPipeline<T>` operator extensions, and a per-item
error policy (`DelegateTransformerOptions`). The repository was created from `repo-template` and keeps
its CI, security and maintenance tooling, but it is a working library, not a template.

**Repository Type**: Library (NuGet package `Wolfgang.Etl.Transformers`)
**Target Platforms** (src): `net462;net472;net48;net481;netstandard2.0;net5.0;net6.0;net7.0;net8.0;net9.0;net10.0`
(read `<TargetFrameworks>` in `src/Wolfgang.Etl.Transformers/Wolfgang.Etl.Transformers.csproj`; there is no `netcoreapp3.1` target)
**Primary Language**: C#

## Build and Validation Instructions

### Prerequisites
- .NET 10.0 SDK (builds every TFM; the test matrix also needs the .NET 5.0-9.0 runtimes, and
  .NET Framework 4.6.2-4.8.1 developer packs on Windows)
- PowerShell 7+ for the scripts under `scripts/`
- The tools pinned in `.config/dotnet-tools.json` (`dotnet tool restore`; `scripts/build-pr.ps1` does this)

### Local gate (run before every push)
```powershell
pwsh ./scripts/build-pr.ps1
```
It mirrors the Windows stage of `.github/workflows/pr.yaml`: Release restore and build, every test
project on every target framework, the coverage gate, DevSkim and gitleaks. `-SkipTests`,
`-SkipCoverage` and `-SkipSecurity` shorten it while iterating.

### Critical Build Requirements
- **Code Coverage**: at least **95% line coverage** for production assemblies (`src/`) and **100%**
  for test assemblies (`tests/`). `pr.yaml` sets `CODECOV_MINIMUM: 95` and `CODECOV_TEST_MINIMUM: 100`;
  `release.yaml` re-checks `CODECOV_MINIMUM: 95`.
- **Warnings are errors**: fix analyzer findings rather than suppressing them.
- **Public API**: every public addition needs a `PublicAPI.Unshipped.txt` entry (RS0016/RS0017).
- **Changelog**: a PR that changes `src/` adds one fragment under `changelog/unreleased/` (see
  `changelog/unreleased/README.md`) or carries the `no-changelog` label; `CHANGELOG.md` is assembled
  at release time by `scripts/changelog.ps1`.
- **Security Scanning**: DevSkim and gitleaks must report nothing.

### Common Issues and Workarounds
- **Duration**: the full gate runs every TFM of eight test projects and takes a while; use the skip
  switches while iterating, then run it in full once before pushing.
- **Coverage Threshold Failures**: below 95% (src) or 100% (tests) fails the build by design.
- **DevSkim False Positives**: review the DevSkim output; do not suppress inline.

## Project Layout and Architecture

```
root/
├── ETL-Transformers.slnx
├── src/Wolfgang.Etl.Transformers/           # the library (one public type per file)
├── tests/
│   ├── Wolfgang.Etl.Transformers.Tests.Unit/          # main unit tests, full TFM matrix
│   ├── Wolfgang.Etl.Transformers.Tests.Integration/   # pipeline tests, full TFM matrix
│   ├── ...Tests.Allocation / .Concurrency / .DocExamples / .Fuzz / .Snapshots / .SourceLink
├── benchmarks/Wolfgang.Etl.Transformers.Benchmarks/   # BenchmarkDotNet (gated by pr-benchmarks.yaml)
├── profiling/                               # GC profiling harness
├── examples/                                # BasicChain, BufferedPipeline, LinqOps
├── samples/                                 # shadow-testing workloads
├── docfx_project/                           # DocFX site (published to gh-pages)
├── docs/                                    # ADRs and per-topic guides
├── changelog/unreleased/                    # changelog fragments
└── scripts/                                 # build-pr.ps1, changelog.ps1, format.ps1, ...
```

### Key Configuration Files
- **`.editorconfig`**, **`Directory.Build.props`**, **`Directory.Build.targets`**, **`BannedSymbols.txt`**:
  code style, analyzers and banned (sync-blocking) APIs
- **`coverlet.runsettings`**: coverage collection settings
- **`CONTRIBUTING.md`**: contribution guidelines and the analyzer list
- **`docs/WORKFLOW_SECURITY.md`**: how the PR workflow and the protected-files guard keep CI honest

### GitHub Integration
- **`pr.yaml`**: runs on `pull_request` to `main` (and on push to `main`): gitleaks, project detection,
  the changelog-fragment check, ReSharper InspectCode, Stage 1 Linux tests + coverage gate,
  Stage 2 Windows tests (.NET 5.0-10.0 and .NET Framework 4.6.2-4.8.1), Stage 3 macOS tests, DevSkim
- **`protected-files.yaml`**: fails a PR that mixes protected configuration files with other changes
- **`release.yaml`**: runs when a GitHub Release is published; see `docs/RELEASE-WORKFLOW-SETUP.md`
- Further scheduled / on-demand workflows: CodeQL, Semgrep, Scorecard, AOT, fuzzing, Stryker,
  benchmarks, SBOM, SourceLink, reproducible build, license audit and others under `.github/workflows/`
- **Issue Templates**, **PR Template**, **CODEOWNERS** (`@Chris-Wolfgang`), **Dependabot**

### Branch Protection Configuration
Branch protection is a repository ruleset on `main`. `scripts/Fix-BranchRuleset.ps1` audits and
repairs it with the GitHub CLI (`gh auth login` first, then `pwsh -File ./scripts/Fix-BranchRuleset.ps1`).
Rulesets are changed by the maintainer only.

## Maintenance Framework

Every repo derived from this template uses a per-repo **Maintenance** tracking framework to organize ongoing improvement work (security, performance, testing, cleanup, docs, API, CI/CD). The framework has three pieces:

1. **One parent `Maintenance: <repo>` issue per repo** (labeled `maintenance`). Living "improvement menu" — stays open forever. Lists candidate work by category. Never close.
2. **Sub-issues labeled `maintenance-task` + a `maintenance - <category>` label.** These are the actual tracked work. Spawn lazily — create one when there's actionable work, close when done.
3. **A cross-repo GitHub Projects v2 board** that auto-aggregates every `maintenance-task` issue across all repos for filtered views (by repo, by category, by status).

### Categories (one per sub-issue)

| Label | Covers |
|---|---|
| `maintenance - security` | SAST/analyzer scans, finding fixes, dependency vulnerability audit |
| `maintenance - performance` | Profile, benchmark, optimize, validate gains |
| `maintenance - testing` | Coverage %, integration/smoke/mutation tests, fixtures, CI test-step additions |
| `maintenance - cleanup` | Refactor for reuse, quality, efficiency |
| `maintenance - docs` | XML doc coverage, README, CHANGELOG, samples |
| `maintenance - API` | Public/internal surface audit, breaking-change vigilance |
| `maintenance - CI/CD` | Docker, CI workflow, build/publish pipeline, packaging |

### For Copilot agents working in this repo

When you **discover work** that fits one of the categories — a security scanner finding, a slow hot path, missing test coverage, code smelling for refactor, outdated docs, an inadvertent breaking API change, a flaky CI step — **don't just comment or fix it silently.** Instead:

1. Open a `maintenance-task` sub-issue using the **"Maintenance task" issue template** (`.github/ISSUE_TEMPLATE/maintenance-task.yaml`).
2. Pick the matching category in the dropdown.
3. Fill in the Scope and Acceptance fields with concrete observable outcomes.
4. After creation, add the corresponding `maintenance - <category>` label (the form pre-fills only `maintenance-task`; the category must be added manually).
5. If you're submitting a PR that addresses the issue, include `Fixes #<sub-issue-number>` in the PR body so the project board auto-marks the item as Done.

This produces a paper trail that ties findings → tracked work → PRs, and rolls up into the cross-repo Maintenance view so unfinished improvements aren't forgotten when the agent session ends.

**Reserve plain (non-`maintenance - ` prefixed) issues for repo-specific decisions** that aren't part of the fleet-wide pattern — e.g., "Drop net6.0 from TFM matrix," "Add new feature X," one-off bug fixes.

### Provisioning Maintenance on a new repo

When a new repo is created from this template:
1. Run `scripts/Setup-Labels.ps1` to provision the 9 Maintenance labels.
2. Run `scripts/Setup-Maintenance.ps1 -MaintenanceProjectUrl '<url>'` to create the parent Maintenance issue. (Pass the cross-repo project URL — ask the user if you don't have it.)
3. The auto-add workflow in the Maintenance project will pick up new `maintenance-task` sub-issues automatically.

## Agent Guidelines

### Trust These Instructions
These instructions describe this repository's current build, gates and layout. **Only search for
additional information if these instructions are incomplete or found to be incorrect.**

### When Working in This Repository
1. **Code Style**: follow `.editorconfig` and the conventions in `CONTRIBUTING.md` (Allman braces,
   file-scoped namespaces)
2. **Adding Dependencies**: use `dotnet add package`; watch for transitive cascades (for example
   `Microsoft.Bcl.*`) across the TFM matrix
3. **Async only**: no synchronous I/O or blocking waits; `BannedSymbols.txt` enforces it
4. **Testing**: new behaviour gets tests in `tests/Wolfgang.Etl.Transformers.Tests.Unit/`; test
   names follow `MethodUnderTest_when_condition_expected_result`
5. **Coverage**: keep src at 95%+ and test assemblies at 100%
6. **Security**: review DevSkim findings and address security concerns
7. **Maintenance framework**: when you find improvement work that fits a category, open a `maintenance-task` sub-issue rather than fixing silently (see "Maintenance Framework" section above)

### Validation Steps
Before submitting changes:
1. Run `pwsh ./scripts/build-pr.ps1` (build, all tests on every TFM, coverage gate, DevSkim, gitleaks)
2. Add a changelog fragment if `src/` changed, and run `pwsh ./scripts/changelog.ps1 check` after committing
3. Update `PublicAPI.Unshipped.txt` for any public API change
4. Ensure all GitHub Actions checks pass
