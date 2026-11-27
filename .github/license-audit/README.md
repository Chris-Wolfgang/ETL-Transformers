# License audit

Dependabot tracks dependency *versions* and CVEs; it does **not** check the
*licenses* of transitive dependencies. A new transitive package under an
incompatible license (GPL/AGPL/OSL/business-source) could otherwise leak into
this MIT-licensed library and become a procurement/legal exposure for
downstream consumers.

The [`license-audit.yaml`](../workflows/license-audit.yaml) workflow enumerates
every package (direct **and** transitive) via
[`nuget-license`](https://github.com/sensslen/nuget-license), resolves each
declared license, and **fails** on anything outside the allowlist.

## Policy

- **Allowlist** — [`allowed-licenses.json`](allowed-licenses.json). Permissive
  SPDX identifiers only (MIT, Apache-2.0, BSD-2/3-Clause, ISC, 0BSD, MS-PL,
  MS-EULA for Microsoft runtime packages). Anything else fails the build.
- **New package under a non-allowlisted license** — the audit fails. To accept
  it, a reviewer must record explicit approval in the PR **and** add the
  license to the allowlist (or the package to the ignore list) in the same PR,
  so the decision is auditable in git history.
- **URL-only licenses** — some Microsoft packages declare their license via URL
  rather than an SPDX expression. [`license-url-mappings.json`](license-url-mappings.json)
  maps those URLs to SPDX so they resolve instead of failing as "unknown".
- **Build-time analyzers** — [`ignored-packages.json`](ignored-packages.json)
  lists `PrivateAssets="all"` analyzer/tooling packages (Roslynator, Sonar,
  Meziantou, SourceLink, …). They run at build time only and are **not**
  distributed inside the NuGet package, so their licenses (e.g. SonarAnalyzer's
  copyleft) do not attach to the shipped work. Ignoring them keeps the audit
  focused on packages that actually ship.

## Third-party notices

`THIRD-PARTY-NOTICES.md` is generated, never committed. At release time
`scripts/third-party-notices.ps1` renders each `src/` project's transitive NuGet
closure to `obj/THIRD-PARTY-NOTICES.md` using the policy files in this folder, and
`Directory.Build.props` packs it at the package root, so the attribution ships in
every `.nupkg`. To see it locally, run `pwsh ./scripts/third-party-notices.ps1 -Strict`.
