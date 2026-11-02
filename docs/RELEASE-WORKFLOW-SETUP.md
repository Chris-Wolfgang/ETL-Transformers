# Release Workflow Setup Guide

This guide explains how this repository's `release.yaml` workflow is configured and what it does when
a release is published.

## Overview

The release workflow triggers when you **publish a GitHub Release** and runs a validation and
deployment pipeline that:
- ✅ Checks that the release tag matches the csproj `<Version>`
- ✅ Tests every target framework of every test project on Windows
- ✅ Enforces the 95% line-coverage threshold (`CODECOV_MINIMUM: 95`)
- ✅ Packs the NuGet package, smoke-tests installing it, and generates an SBOM and a
  reproducible-build manifest
- ✅ Verifies that the documentation builds and that the `.snupkg` carries portable PDBs
- ✅ Publishes to NuGet.org through Trusted Publishing (OIDC; no API-key secret)
- ✅ Checks the symbol server, deploys the docs, and attaches the packages, coverage report and a
  SLSA build-provenance attestation to the Release

## Required Configuration

### NuGet Trusted Publishing policy

**Location:** nuget.org → your account → Trusted Publishing

`publish-nuget` uses `NuGet/login` to exchange the workflow's OIDC token for a one-hour API key, so
no `NUGET_API_KEY` secret is needed. nuget.org must have a Trusted Publishing policy for:

- **Owner:** `Chris-Wolfgang`
- **Repository:** `ETL-Transformers`
- **Workflow:** `release.yaml`

The job declares `id-token: write` for the token exchange, and runs the login immediately before
the push because the temporary key expires after an hour.

### Optional: package signing certificate

`pack-and-validate` author-signs the packages when the `NUGET_SIGN_CERT_BASE64` (base64 `.pfx`) and
`NUGET_SIGN_CERT_PASSWORD` repository secrets exist, and skips signing when they do not. NuGet.org
adds its own repository signature either way.

### Verify Branch Protection Rules

**Location:** Settings → Rules → Rulesets

> **Note:** `scripts/Fix-BranchRuleset.ps1` audits and repairs the `main` ruleset with the GitHub
> CLI. If you change the ruleset by hand, use the checklist below.

Ensure the following settings are enabled:

- ✅ **Require a pull request before merging**
  - **Single developer repos:** 0 approvals (default)
  - **Multi-developer repos:** 1+ approvals (recommended)
- ✅ **Require status checks to pass before merging**
  - Required checks should include the following status check contexts:
    - "Stage 1: Linux Tests (.NET 5.0-10.0) + Coverage Gate"
    - "Stage 2: Windows Tests (.NET 5.0-10.0, Framework 4.6.2-4.8.1)"
    - "Stage 3: macOS Tests (.NET 6.0-10.0)"
    - "Security Scan (DevSkim)"
    - "Security Scan (CodeQL)"
- ✅ **Require branches to be up to date before merging**
- ✅ **Require conversation resolution before merging**
- ✅ **Do not allow bypassing the above settings** (recommended, even for admins)
- ✅ **Restrict deletions**
- ✅ **Require linear history** (optional but recommended)

**What this does:** Ensures all code merged to `main` has passed comprehensive validation, preventing broken releases.

## Testing the Release Workflow

1. Go to your repository's **Releases** page
2. Click **"Draft a new release"**
3. Choose or create a tag (e.g., `v0.0.1-test`); it must match the csproj `<Version>` or
   `validate-release` fails at its first step
4. Add a title and description (optional for a test)
5. Check **"Set as a pre-release"** for test releases
6. Click **"Publish release"**

The workflow triggers automatically when the release is published.

### Expected Workflow Behavior

| Job | Runs on | Needs | What it does |
|---|---|---|---|
| `validate-release` | Windows | - | Tag/version check, Release build, every test project on every TFM with coverage, 95% gate, uploads the coverage report |
| `pack-and-validate` | Windows | `validate-release` | Third-party notices, pack, smoke-test install, CycloneDX SBOM, reproducible-build manifest, optional signing, uploads the packages |
| `verify-docs-build` | Windows | `validate-release`, `pack-and-validate` | Builds the DocFX metadata and site without deploying and checks the output |
| `sourcelink-snupkg` | Ubuntu | `pack-and-validate` | Verifies the `.snupkg` carries portable PDBs |
| `publish-nuget` | Windows | `pack-and-validate`, `verify-docs-build`, `sourcelink-snupkg` | `NuGet/login` (OIDC), then pushes the packages to NuGet.org |
| `sourcelink-symbol-server-smoke` | Ubuntu | `publish-nuget` | Polls the NuGet symbol server until every published PDB is served |
| `trigger-docs` | reusable `docfx.yaml` | `validate-release` | Builds and deploys the documentation to GitHub Pages |
| `update-release-artifacts` | Ubuntu | `validate-release`, `pack-and-validate`, `publish-nuget` | SLSA build-provenance attestation, then attaches the packages and the zipped coverage report to the Release |

### Monitoring the Workflow

- **Actions Tab:** Shows workflow progress in real-time
- **Artifacts:** the coverage report and the NuGet packages
- **Releases:** the attached packages, coverage zip and attestation after a successful run

## Troubleshooting

### NuGet login fails

**Problem:** `publish-nuget` fails at "NuGet login (OIDC → temporary API key)".

**Solution:**
1. Check the Trusted Publishing policy on nuget.org (owner, repository and workflow file name must match exactly)
2. Re-run the failed jobs from the Actions tab (do not re-publish the release)

### Tests Fail on Specific Framework

**Problem:** Tests pass on some frameworks but fail on others (e.g., net462).

**Solution:**
1. Check the test logs for framework-specific issues
2. Fix compatibility issues in your code
3. Test locally: `dotnet test --framework net462`
4. Push fix, then re-publish the release (or re-run the workflow from the Actions tab)

### Coverage Below 95% Threshold

**Problem:** Workflow fails at "Verify coverage threshold".

**Solution:**
1. Review the uploaded coverage report artifact
2. Add tests for uncovered code paths
3. Ensure tests run on all frameworks
4. Push fix, then re-publish the release (or re-run the workflow from the Actions tab)

### Smoke Test Fails to Install Package

**Problem:** Package packs successfully but fails smoke test installation.

**Solution:**
1. Check package dependencies in `.csproj`
2. Verify framework compatibility in `<TargetFrameworks>`
3. Test locally: `dotnet pack` then try installing in a test project
4. Fix packaging issues and re-publish the release (or re-run the workflow from the Actions tab)

## Production Release Checklist

Before creating a production GitHub Release (e.g., `v1.0.0`):

- [ ] All tests pass on all platforms (pr.yaml workflow)
- [ ] Coverage meets 95% for src (and 100% for test assemblies, which pr.yaml enforces)
- [ ] Security scan shows no critical issues
- [ ] `<Version>` updated in the csproj (`AssemblyVersion` is derived from it)
- [ ] `CHANGELOG.md` assembled from `changelog/unreleased/` with `pwsh ./scripts/changelog.ps1 assemble -Version X.Y.Z`
- [ ] All PRs merged to `main` branch
- [ ] Local gate passes: `pwsh ./scripts/build-pr.ps1`

**Create a production release:**
1. Go to your repository's **Releases** page
2. Click **"Draft a new release"**
3. Choose or create the version tag (e.g., `v1.0.0`) targeting `main`
4. Add a title and release notes
5. Click **"Publish release"**

**After workflow completes:**
- [ ] Verify packages appear on NuGet.org
- [ ] Test installing package from NuGet.org in a clean project
- [ ] Announce release (if applicable)

## Workflow Architecture

```
Published GitHub Release
          |
          v
  validate-release --------------------------+------------> trigger-docs (docfx.yaml)
          |                                   |
          v                                   |
  pack-and-validate -----------+              |
          |                    |              |
          v                    v              |
  sourcelink-snupkg     verify-docs-build <---+
          |                    |
          +---------+----------+
                    v
              publish-nuget ------------> sourcelink-symbol-server-smoke
                    |
                    v
        update-release-artifacts (attestation + Release assets)
```

## Support

If you encounter issues not covered in this guide:

1. Check the Actions tab of this repository on GitHub for detailed logs
2. Review artifacts uploaded by failed jobs
3. Consult the [GitHub Actions documentation](https://docs.github.com/en/actions)
4. Open an issue in this repository with:
   - Workflow run URL
   - Error message and logs
   - Steps to reproduce
