# OpenSSF Scorecard

[`scorecard.yml`](../.github/workflows/scorecard.yml) runs the
[OpenSSF Scorecard](https://github.com/ossf/scorecard) action weekly (and on
push to `main` / branch-protection-rule changes). Scorecard grades the
repository's **security configuration** — branch protection, SHA-pinned
actions, least-privilege token permissions, dangerous-workflow patterns,
vulnerability-response time, and ~15 other checks — and produces a 0–10 score.

It is complementary to CodeQL/SAST, which scan the *code*; Scorecard scans the
*repo setup*. Results upload to **Security → Code scanning** alongside CodeQL
alerts, and publish to the public Scorecard API that backs the README badge.

## Score floor

- **Threshold: 7.5 / 10.** If a Scorecard run drops the aggregate score below
  7.5, that is a signal for reviewer attention — note the regression and its
  cause in `CHANGELOG.md` (under a `### Security` entry) and open a follow-up to
  restore it, or record why the lower score is accepted.
- Individual check failures that are **known and accepted** (e.g. an ecosystem
  limitation) should be documented here so they aren't re-investigated each run.

## Baseline

The initial score is established by the first scheduled/`main` run after this
workflow lands (Scorecard does not run on pull requests — it needs the default
branch and repo metadata). Record the baseline score and any accepted findings
in this section once that first run completes.

_Baseline: pending first run on `main`._
