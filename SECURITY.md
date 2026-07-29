# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

1. **Do not** create a public issue on this repository.
2. In the top navigation of this repository, click the **Security** tab.
3. In the top right, click the **Report a vulnerability** button.
4. Fill out the provided form with:
   - A description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if you have one)

## Response Timeline

We will acknowledge your report within 48 hours and provide an estimated timeline for a fix.

## Thank You

Your help is greatly appreciated!
Responsible disclosure of security vulnerabilities helps protect our entire community.

## Release path & compromise scope

Facts a maintainer would need at 2am if the release identity is compromised. Generic incident-response steps (rotating credentials, revoking OAuth apps, publishing advisories, unlisting NuGet packages) are not duplicated here — GitHub's and NuGet's own docs update faster than a checked-in runbook.

- **Release path**: OIDC / NuGet Trusted Publishing via `NuGet/login@v1` in `.github/workflows/release.yaml`. The workflow mints an ephemeral push token per run via OIDC — the release path does not depend on a long-lived API key stored in GitHub secrets or on the NuGet account. During an incident, check the NuGet account for any long-lived API keys anyway (they can be created outside of CI) and delete anything you don't recognize.
- **Fallback**: none. If Trusted Publishing is compromised, the incident is at the GitHub-account level (the OIDC identity is `Chris-Wolfgang/ETL-Transformers`).
- **Owner**: @Chris-Wolfgang.
- **Downstream consumers**: no known Wolfgang.* repository takes a package dependency on this library today — it is a leaf library consumed directly by end-user ETL pipelines. Unknown external consumers may exist on nuget.org.
- **Package coordinates for unlisting**: `Wolfgang.Etl.Transformers` on nuget.org — https://www.nuget.org/packages/Wolfgang.Etl.Transformers/. This repo ships a single package.

## Verifying the supply chain

Every release ships artifacts that let a consumer prove the package they downloaded was
built from this repository, unmodified. Replace `<version>` with the release you are checking.

### 1. Package signature — `nuget verify`

NuGet.org applies a **repository signature** to every package on publish, so the signature
is verifiable for any release:

```bash
nuget verify -Signatures Wolfgang.Etl.Transformers.<version>.nupkg
```

If an **author signature** is also present (published once a code-signing certificate is
provisioned — see the secret-gated signing step in `release.yaml`), the same command reports
it alongside the repository signature.

### 2. Build provenance — SLSA attestation

Each release records a Sigstore-backed [SLSA build-provenance](https://slsa.dev/) attestation
tying the package to this workflow and commit. Verify it with the GitHub CLI:

```bash
gh attestation verify Wolfgang.Etl.Transformers.<version>.nupkg \
  --repo Chris-Wolfgang/ETL-Transformers
```

A successful check confirms the artifact was produced by `release.yaml` in this repo, not
re-uploaded or tampered with.

### 3. SBOM

Each release attaches a CycloneDX SBOM (`Wolfgang.Etl.Transformers.bom.json`) enumerating the
dependency graph, for vulnerability scanning and license review.

### 4. Reproducible build

The build is byte-for-byte reproducible: you can rebuild the tag from source and compare
hashes against the attached `reproducible-build-manifest.json`. See
[REPRODUCIBLE-BUILD.md](REPRODUCIBLE-BUILD.md) for the full procedure.

Together these close the chain: **reproducible build** (bits match source) → **provenance**
(this repo/workflow produced them) → **signature** (they were not altered after signing) →
**SBOM** (what is inside).
