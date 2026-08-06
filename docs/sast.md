# Static analysis (SAST)

Overlapping static-analysis signals — the goal is coverage from independent
scanners, not one authoritative tool.

| Tool | Where | Cost | Notes |
| --- | --- | --- | --- |
| **CodeQL** | `codeql.yaml` | free (OSS) | security-extended query pack; the baseline SAST. |
| **DevSkim** | `pr.yaml` (Security Scan step) | free | Microsoft pattern-based scanner. |
| **Semgrep (OSS)** | `semgrep.yaml` | free, no account | `p/csharp` + `p/security-audit` + `p/secrets` rulesets; diff-aware on PRs (`--baseline-commit`), SARIF → code scanning. |

## Paid-tier SAST — pending project decision

The tools below catch classes CodeQL/Semgrep miss (semantic taint, dataflow) but
are a **budget/account decision**, not a per-repo one, so they are **not** wired
in here:

- **Snyk Code** — has a free OSS tier worth trying first (needs a `SNYK_TOKEN`
  secret + account).
- **Veracode SAST**, **Fortify SCA**, **SonarQube Cloud**, **Checkmarx One** —
  commercial, per-developer licensing.

When a tool is adopted: obtain the account/license, add its workflow (findings as
PR annotations), commit a baseline scan of `main` so per-PR scans report only new
findings, and record the triage outcome (false-positive / fixed / accepted-risk)
in an audit log here.

## Triage log

_No findings triaged yet. When a scanner surfaces a finding, record it here:_
`YYYY-MM-DD — <tool> <rule> at <file:line> — false-positive | fixed (<commit>) | accepted-risk (<why>)`.
