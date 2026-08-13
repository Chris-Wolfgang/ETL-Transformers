#!/usr/bin/env python3
"""Normalize a VSTest ``.trx`` file into a stable, platform-independent text form.

Emits one ``<outcome>\t<testName>\t<message>`` line per test, sorted by test name, so the
output of the same test suite run on different OS/architecture combinations can be diffed
directly. Any divergence — a test that passes on one platform and fails on another, a
theory whose parameter rendering differs because of a culture-sensitive conversion, or the
same test failing for *different reasons* / with different reported numbers on two
platforms — shows up as a line difference. The failure ``<message>`` (assertion text and
reported values) is included with absolute paths redacted, so a benign per-OS path in a
message does not read as a divergence while the meaningful assertion content still does.

Used by ``.github/workflows/cross-platform-differential.yaml``. Requires ``defusedxml``.

Usage:
    normalize-test-results.py <results.trx> [more.trx ...]
"""
from __future__ import annotations

import re
import sys

# Parse with defusedxml (hardened against XXE / entity-expansion) rather than the stdlib
# xml.etree. The .trx input is our own CI output and thus trusted, but defusedxml is the
# recommended parser and keeps static analysis (Semgrep use-defused-xml) satisfied without
# relying on inline suppressions, whose matching varies across Semgrep versions.
import defusedxml.ElementTree as ET

TRX_NS = "{http://microsoft.com/schemas/VisualStudio/TeamTest/2010}"

_WHITESPACE = re.compile(r"\s+")
# Redact absolute paths (Windows drive paths and common Unix runner roots) so a benign
# per-OS path inside an assertion/stack message is not mistaken for a real divergence,
# while the assertion text and reported numbers #86 requires diffing are preserved.
_WIN_PATH = re.compile(r"[A-Za-z]:\\[^\s\"']*")
_NIX_PATH = re.compile(r"/(?:home|Users|private|tmp|opt|usr)/[^\s\"']*")


def failure_message(result) -> str:
    """Normalized assertion/error message for a non-passing result ('' when absent)."""
    node = result.find(f"{TRX_NS}Output/{TRX_NS}ErrorInfo/{TRX_NS}Message")
    if node is None or node.text is None:
        return ""
    text = _WIN_PATH.sub("<path>", node.text)
    text = _NIX_PATH.sub("<path>", text)
    return _WHITESPACE.sub(" ", text).strip()


def normalize(trx_paths: list[str]) -> list[str]:
    seen: dict[str, tuple[str, str]] = {}
    for path in trx_paths:
        root = ET.parse(path).getroot()
        for result in root.iter(f"{TRX_NS}UnitTestResult"):
            name = result.get("testName")
            outcome = result.get("outcome", "Unknown")
            if name is None:
                continue
            message = "" if outcome == "Passed" else failure_message(result)
            # If the same test somehow appears twice, a Failed/odd outcome wins over Passed
            # so a divergence is never masked.
            if name not in seen or (seen[name][0] == "Passed" and outcome != "Passed"):
                seen[name] = (outcome, message)
    return [f"{outcome}\t{name}\t{message}" for name, (outcome, message) in sorted(seen.items())]


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: normalize-test-results.py <results.trx> [more.trx ...]", file=sys.stderr)
        return 2
    for line in normalize(sys.argv[1:]):
        print(line)
    return 0


if __name__ == "__main__":
    sys.exit(main())
