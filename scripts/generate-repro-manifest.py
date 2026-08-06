#!/usr/bin/env python3
"""Generate a reproducible-build manifest for a set of packed NuGet artifacts.

The manifest lists the SHA-256 of every ``.nupkg`` / ``.snupkg`` released, plus the
SHA-256 of each assembly (``lib/**/*.dll``) inside the package. A third party who
rebuilds the same tag with the same SDK can regenerate this file and diff it against
the copy attached to the GitHub Release to confirm the published bits match source.

Emitted per release by ``.github/workflows/release.yaml`` as
``reproducible-build-manifest.json`` and documented in ``REPRODUCIBLE-BUILD.md``.

Pure standard library — runs anywhere Python 3 does, no jq or NuGet tooling required.

Usage:
    generate-repro-manifest.py --packages <dir> --out <file.json> \
        [--repository owner/repo] [--tag v0.0.0] [--commit <sha>] [--sdk <version>]
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
import sys
import zipfile


def sha256_bytes(data: bytes) -> str:
    digest = hashlib.sha256()
    digest.update(data)
    return digest.hexdigest()


def sha256_file(path: str) -> str:
    digest = hashlib.sha256()
    with open(path, "rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def package_contents(path: str) -> list[dict[str, str]]:
    """SHA-256 of each assembly under lib/ inside a .nupkg/.snupkg (sorted, stable)."""
    contents: list[dict[str, str]] = []
    with zipfile.ZipFile(path) as archive:
        for name in sorted(archive.namelist()):
            lowered = name.lower()
            if lowered.startswith("lib/") and lowered.endswith((".dll", ".pdb")):
                contents.append({"path": name, "sha256": sha256_bytes(archive.read(name))})
    return contents


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--packages", required=True, help="directory holding the packed artifacts")
    parser.add_argument("--out", required=True, help="manifest output path")
    parser.add_argument("--repository", default=os.environ.get("GITHUB_REPOSITORY", ""))
    parser.add_argument("--tag", default="")
    parser.add_argument("--commit", default=os.environ.get("GITHUB_SHA", ""))
    parser.add_argument("--sdk", default="", help="the .NET SDK version used to build")
    args = parser.parse_args()

    artifacts = []
    for entry in sorted(os.listdir(args.packages)):
        if not entry.endswith((".nupkg", ".snupkg")):
            continue
        full = os.path.join(args.packages, entry)
        artifacts.append(
            {
                "file": entry,
                "sha256": sha256_file(full),
                "contents": package_contents(full),
            }
        )

    if not artifacts:
        print(f"::error::No .nupkg/.snupkg found in {args.packages}", file=sys.stderr)
        return 1

    manifest = {
        "schemaVersion": 1,
        "repository": args.repository,
        "tag": args.tag,
        "commit": args.commit,
        "sdkVersion": args.sdk,
        "buildCommand": (
            "dotnet pack src/Wolfgang.Etl.Transformers/Wolfgang.Etl.Transformers.csproj "
            "-c Release -p:ContinuousIntegrationBuild=true -p:Deterministic=true"
        ),
        "artifacts": artifacts,
    }

    with open(args.out, "w", encoding="utf-8", newline="\n") as handle:
        json.dump(manifest, handle, indent=2, ensure_ascii=False)
        handle.write("\n")

    print(f"Wrote {args.out}: {len(artifacts)} artifact(s)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
