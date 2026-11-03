# Workflow Security

## Overview

This document describes how the GitHub Actions workflows keep pull-request validation trustworthy,
focusing on the PR validation workflow (`.github/workflows/pr.yaml`) and the protected-files guard
(`.github/workflows/protected-files.yaml`). The header comment of `pr.yaml` is the short version of
the same model.

## Security Architecture

### 1. PR validation runs on `pull_request`

`pr.yaml` triggers on `pull_request` to `main` (and on `push` to `main`, which re-runs the same gates
on the merged result):

```yaml
on:
  pull_request:
    branches:
      - main
  push:
    branches:
      - main
```

A `pull_request` run builds and tests the PR's own code with the PR's own copies of this workflow and
of every analyzer and configuration file, so the PR is checked against the configuration it would
merge. GitHub gives such a run a read-only `GITHUB_TOKEN` and, for forks, no secrets. Nothing in
`pr.yaml` uses a secret, and the workflow declares `permissions: contents: read`.

The only write scope in `pr.yaml` is `security-events: write` on the `inspectcode-upload` job, which
never checks out PR code: the `inspectcode` job runs ReSharper InspectCode with read-only
permissions and hands its SARIF to `inspectcode-upload` as an artifact. The upload runs only for
pushes to `main` and same-repository PRs not opened by Dependabot (their tokens cannot write Code
Scanning); fork and Dependabot PRs still get the InspectCode gate itself.

### 2. Gate integrity: the protected-files guard

Because `pr.yaml` uses the PR's own configuration, a PR could otherwise weaken a gate and pass it in
the same change. `protected-files.yaml` closes that gap. It is the one workflow that still runs on
`pull_request_target`, and it may, because it **never checks out or executes anything from the PR**:
it runs from `main` (so a PR cannot edit it), lists the PR's changed files through the API, and
classifies the PR:

| PR changes | Result |
|---|---|
| no protected file | pass; `pr.yaml` validated everything |
| only protected files | pass with a notice; a maintainer reviews that diff on its own |
| protected files **and** anything else | **fail**; split the PR |

**Protected files** (the `case` list and pattern in `protected-files.yaml` are authoritative):
- `.editorconfig` (any directory), `*.globalconfig`, `*.ruleset`, `*.DotSettings` (case-insensitive)
- `Directory.Build.props`, `Directory.Build.targets`, `BannedSymbols.txt`
- `coverlet.runsettings`, `.config/dotnet-tools.json`, `.gitleaks.toml`
- the CI scripts the workflows run: `scripts/build-pr.ps1`, `scripts/changelog.ps1`,
  `scripts/tfm-parity.ps1`, `scripts/third-party-notices.ps1`
- `.github/workflows/*.yml` / `*.yaml`
- `.github/license-audit/*.json` (read from `main` by `license-audit.yaml`)

Dependabot PRs are exempt: its bumps to protected files (for example `Directory.Build.props`) are
legitimate and its identity is GitHub-controlled.

### 3. Credential Protection

**Mechanism**: `persist-credentials: false`

Every checkout step sets `persist-credentials: false` so the checkout token is not written to git
config. Actions are pinned to a full commit SHA with the version in a trailing comment:

```yaml
- name: Checkout code
  uses: actions/checkout@3d3c42e5aac5ba805825da76410c181273ba90b1  # v7.0.1
  with:
    persist-credentials: false
```

On a `pull_request` event this checks out the PR merge ref. **Note**: `persist-credentials: false`
keeps the token out of git config; it does NOT stop a step from using `GITHUB_TOKEN` if a workflow
explicitly exposes it.

### 4. Minimal Permissions

Workflows default to

```yaml
permissions:
  contents: read
```

and individual jobs declare a write scope only where they need one (for example
`security-events: write` for a SARIF upload, or `id-token: write` for NuGet trusted publishing and
build-provenance attestation in `release.yaml`).

## Attack Scenarios Prevented

### Scenario 1: Malicious Workflow Modification
**Attack**: a PR edits `.github/workflows/pr.yaml` to disable a check and adds code that would fail it.
**Prevention**: the workflow file is protected; a PR mixing it with other changes fails the
protected-files guard, and a workflow-only PR is reviewed on its own.
**Status**: ✅ Protected

### Scenario 2: Configuration File Tampering
**Attack**: a PR edits `.editorconfig` or `Directory.Build.props` to turn analyzers off.
**Prevention**: same guard; the configuration change cannot ride along with the code it would let through.
**Status**: ✅ Protected

### Scenario 3: Credential Theft
**Attack**: PR code tries to read GitHub credentials.
**Prevention**: `pull_request` runs get a read-only token and no secrets; `persist-credentials: false`;
minimal permissions; the one write-scoped job never runs PR code.
**Status**: ✅ Protected

### Scenario 4: Code Analysis Bypass
**Attack**: a PR edits `BannedSymbols.txt` or a `.ruleset` to allow a dangerous API it also uses.
**Prevention**: same guard.
**Status**: ✅ Protected

## Validation

Manual checks when reviewing a change to this model:

1. Open a PR that changes `.editorconfig` together with a `.cs` file and confirm the
   "Protected Files Guard" check fails and names the file.
2. Open a PR that changes only `.editorconfig` and confirm the guard passes with the
   configuration-only notice.

## Maintenance

### Making Changes to Protected Configuration Files

1. Put the configuration change in its **own** PR (no other files). The guard passes it with a notice;
   `pr.yaml` runs with the new configuration, so the PR shows exactly what the change does.
2. Review the configuration diff itself before merging: it decides what every later PR is checked against.
3. Then open the code change that depends on it as a separate PR.

If you need to relax a rule and add code that needs the relaxed rule, that is two PRs: the
configuration first, then the code.

### Adding New Protected Configuration Files

1. Add the path to the `case` list (exact names) or the `grep -qiE` pattern (globs) in
   `.github/workflows/protected-files.yaml`.
2. Update the protected-file list in this document.

## References

- [GitHub Actions Security Hardening](https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions)
- [Keeping your GitHub Actions secure](https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions#using-third-party-actions)
- [Understanding pull_request_target](https://securitylab.github.com/research/github-actions-preventing-pwn-requests/)
