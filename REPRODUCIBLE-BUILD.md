# Reproducible Builds

`Wolfgang.Etl.Transformers` is built **deterministically**: compiling the same
commit with the same SDK produces byte-for-byte identical outputs, regardless of
the machine, operating system, or working directory. This lets anyone
independently confirm that a published package was built from the tagged source
and contains no hidden modifications.

## The guarantee

For a given commit, the following outputs are bit-for-bit identical across build
environments:

- the compiled assembly `Wolfgang.Etl.Transformers.dll` (every target framework),
- its debug symbols `Wolfgang.Etl.Transformers.pdb`,
- the produced `.nupkg` / `.snupkg` package.

## How it is achieved

Determinism is configured in [`Directory.Build.props`](Directory.Build.props):

| Setting | Effect |
| --- | --- |
| `Deterministic` (SDK default `true`) | Removes timestamps and other non-deterministic metadata from the assembly. |
| `ContinuousIntegrationBuild=true` (set when `CI=true`) | Enables `DeterministicSourcePaths`, so absolute source paths are replaced with stable, machine-independent tokens. |
| `Microsoft.SourceLink.GitHub` + `EmbedUntrackedSources` | Maps source paths to their canonical repository location, which is what makes the PDB reproducible across operating systems. |

Because source paths are the main thing that differs between an Ubuntu and a
Windows checkout, `ContinuousIntegrationBuild` + SourceLink normalization is the
key ingredient that makes the build reproducible *across operating systems* and
not merely on one machine.

## Continuous verification

The [`Reproducible Build`](.github/workflows/reproducible-build.yaml) workflow
proves the guarantee on every pull request:

1. It packs the project on **both** `ubuntu-latest` and `windows-latest` with
   `ContinuousIntegrationBuild=true`.
2. Each job records the `sha256sum` of every produced `.dll`, `.pdb`, `.nupkg`
   and `.snupkg`.
3. A final job diffs the two manifests and **fails on any byte divergence**,
   printing the offending hashes.

A green run is machine-checkable evidence that the two operating systems produced
identical bits.

## Verify it yourself

You can reproduce a released build locally and compare it against the published
package. Replace `<version>` with the release you are checking.

```bash
# 1. Check out the exact tagged source.
git clone https://github.com/Chris-Wolfgang/ETL-Transformers.git
cd ETL-Transformers
git checkout v<version>

# 2. Pack deterministically (CI=true turns on ContinuousIntegrationBuild).
CI=true dotnet pack src/Wolfgang.Etl.Transformers/Wolfgang.Etl.Transformers.csproj \
  -c Release -p:ContinuousIntegrationBuild=true -p:Deterministic=true -o artifacts

# 3. Hash your locally built assembly (any TFM shown here).
sha256sum src/Wolfgang.Etl.Transformers/bin/Release/net8.0/Wolfgang.Etl.Transformers.dll

# 4. Download the published package and extract the same assembly to compare.
#    (dotnet nuget, curl from nuget.org, or unzip the .nupkg you already have.)
unzip -o artifacts/Wolfgang.Etl.Transformers.<version>.nupkg -d published
sha256sum published/lib/net8.0/Wolfgang.Etl.Transformers.dll
```

If the two hashes match, the published assembly was built from exactly this
source. The same holds for the `.nupkg` itself and for every other target
framework.

> **Note on the SDK version.** Reproducibility is guaranteed for a *fixed* .NET
> SDK. A different SDK/compiler version can legitimately emit different (but still
> deterministic) bytes. Use the SDK pinned by the CI workflow (`10.0.x`) when
> reproducing a release.

## The release manifest

Every published release attaches a **`reproducible-build-manifest.json`** asset (built
by [`scripts/generate-repro-manifest.py`](scripts/generate-repro-manifest.py) inside
`release.yaml`). It records, for that exact tag:

- the `repository`, `tag`, `commit`, and `sdkVersion` used,
- the exact `buildCommand`,
- the SHA-256 of every published `.nupkg` / `.snupkg`,
- the SHA-256 of each assembly (`lib/**/*.dll`, `lib/**/*.pdb`) *inside* those packages.

This is the authoritative list of expected hashes — you don't have to eyeball
individual `sha256sum` output. To verify a release end to end:

```bash
# 1. Check out the tagged source and pack it with the pinned SDK (see the note above).
git checkout v<version>
CI=true dotnet pack src/Wolfgang.Etl.Transformers/Wolfgang.Etl.Transformers.csproj \
  -c Release -p:ContinuousIntegrationBuild=true -p:Deterministic=true -o artifacts

# 2. Regenerate the manifest from your own build.
python3 scripts/generate-repro-manifest.py --packages artifacts --out my-manifest.json \
  --repository Chris-Wolfgang/ETL-Transformers --tag v<version>

# 3. Download the published manifest from the GitHub Release and diff the hashes.
#    (repository/commit/tag/sdk metadata may differ; the `artifacts` hashes must match.)
curl -sSL -o released-manifest.json \
  https://github.com/Chris-Wolfgang/ETL-Transformers/releases/download/v<version>/reproducible-build-manifest.json
diff \
  <(jq -S '.artifacts' released-manifest.json) \
  <(jq -S '.artifacts' my-manifest.json)
```

An empty `diff` is proof that the packages you can rebuild from source are byte-identical
to the ones that were published.

## File a discrepancy

If any hash differs and you are using the SDK version recorded in the manifest, that is a
supply-chain signal worth reporting. Please
[open an issue](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/new) titled
`Reproducible-build discrepancy: v<version>` and include:

- the release tag and the `sdkVersion` from the published manifest,
- your OS / architecture and `dotnet --version`,
- your generated `my-manifest.json` and the released `reproducible-build-manifest.json`,
- the `diff` output.

Discrepancies are treated as a security concern — see [SECURITY.md](SECURITY.md).

## Publish a third-party attestation

Independent verification is more valuable when it is *public*. If you have reproduced a
release, you can publish an attestation that others can find:

- Follow the [Reproducible Builds project](https://reproducible-builds.org/) conventions
  for recording a successful independent rebuild, or
- publish a signed attestation via a service such as [vouchsafe.io](https://vouchsafe.io/)
  referencing the release tag and the manifest hashes you confirmed.

Then link your attestation from a comment on the release, or in a
`Reproducible-build attestation: v<version>` issue, so future consumers can see the build
has been independently verified.
