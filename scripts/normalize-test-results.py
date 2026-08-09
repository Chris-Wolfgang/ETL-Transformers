#!/usr/bin/env python3
"""Normalize a VSTest ``.trx`` file into a stable, platform-independent text form.

Emits one ``<outcome>\t<testName>`` line per test, sorted by test name, so the output
of the same test suite run on different OS/architecture combinations can be diffed
directly. Any divergence — a test that passes on one platform and fails on another, or a
theory whose parameter rendering differs because of a culture-sensitive conversion —
shows up as a line difference.

Used by ``.github/workflows/cross-platform-differential.yaml``. Requires ``defusedxml``.

Usage:
    normalize-test-results.py <results.trx> [more.trx ...]
"""
from __future__ import annotations

import sys

# Parse with defusedxml (hardened against XXE / entity-expansion) rather than the stdlib
# xml.etree. The .trx input is our own CI output and thus trusted, but defusedxml is the
# recommended parser and keeps static analysis (Semgrep use-defused-xml) satisfied without
# relying on inline suppressions, whose matching varies across Semgrep versions.
import defusedxml.ElementTree as ET

TRX_NS = "{http://microsoft.com/schemas/VisualStudio/TeamTest/2010}"


def normalize(trx_paths: list[str]) -> list[str]:
    seen: dict[str, str] = {}
    for path in trx_paths:
        root = ET.parse(path).getroot()
        for result in root.iter(f"{TRX_NS}UnitTestResult"):
            name = result.get("testName")
            outcome = result.get("outcome", "Unknown")
            if name is None:
                continue
            # If the same test somehow appears twice, a Failed/oddoutcome wins over Passed
            # so a divergence is never masked.
            if name not in seen or (seen[name] == "Passed" and outcome != "Passed"):
                seen[name] = outcome
    return [f"{outcome}\t{name}" for name, outcome in sorted(seen.items())]


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: normalize-test-results.py <results.trx> [more.trx ...]", file=sys.stderr)
        return 2
    for line in normalize(sys.argv[1:]):
        print(line)
    return 0


if __name__ == "__main__":
    sys.exit(main())
