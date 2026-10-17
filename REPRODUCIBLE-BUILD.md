# Reproducible Builds

`Wolfgang.Etl.Transformers` is built **deterministically**: compiling the same
commit **with the same SDK** produces byte-for-byte identical assemblies and symbols,
on any operating system and regardless of the machine or working directory. This lets
anyone independently confirm that a published package was built from the tagged source
and contains no hidden modifications.

## The guarantee

For a given commit built **with the same .NET SDK**, the following outputs are
bit-for-bit identical, **on Linux and Windows alike**:

- the compiled assembly `Wolfgang.Etl.Transformers.dll` (every target framework),
- its debug symbols `Wolfgang.Etl.Transformers.pdb`.

> **The package files are not byte-reproducible.** The `.nupkg` / `.snupkg` are zip
> containers, and NuGet stamps every entry with the time the package was packed and
> writes platform-specific zip attributes and newlines into its own metadata
> (`.nuspec`, `.psmdcp`, `_rels/.rels`). Packing the same commit twice, even on one
> machine, gives two different package files. What is inside them is stable: the
> assemblies and symbols in every package are identical across runs and across
> operating systems. Compare those, not the package file itself (see below).

## How it is achieved

Determinism is configured in [`Directory.Build.props`](Directory.Build.props) and
[`Directory.Build.targets`](Directory.Build.targets):

| Setting | Effect |
| --- | --- |
| `Deterministic` (SDK default `true`) | Removes timestamps and other non-deterministic metadata from the assembly. |
| `ContinuousIntegrationBuild=true` (set when `CI=true`) | Enables `DeterministicSourcePaths`, so absolute source paths are replaced with stable, machine-independent tokens. |
| `Microsoft.SourceLink.GitHub` + `EmbedUntrackedSources` | Maps source paths to their canonical repository location, which is what makes the PDB reproducible across operating systems. |
| `NormalizeGeneratedLineEndings` target (`Directory.Build.targets`) | The SDK writes its generated sources (`*.AssemblyInfo.cs`, `*.AssemblyAttributes.cs`) with the OS newline, CRLF on Windows and LF on Linux. The PDB records each file's hash and embeds its text, so the PDB, and through its checksum and the module version id the DLL, differed per OS. The target rewrites them to LF before compilation. |
| `Microsoft.NETFramework.ReferenceAssemblies` package reference (`Directory.Build.props`, .NET Framework TFMs) | The PDB records every referenced assembly's timestamp and module version id. Windows would otherwise compile against the machine's installed targeting pack and Linux against this NuGet package, which are different builds of the same reference assemblies. Referencing the package makes every OS use it. |

`ContinuousIntegrationBuild` + SourceLink path normalization removes the
machine- and directory-specific differences. The last two rows remove the two
operating-system-specific inputs that remained.

## Continuous verification

The [`Reproducible Build`](.github/workflows/reproducible-build.yaml) workflow
checks reproducibility on every pull request:

1. It packs the project on **both** `ubuntu-latest` and `windows-latest` with
   `ContinuousIntegrationBuild=true`.
2. Each job records the `sha256sum` of every produced `.dll`, `.pdb`, `.nupkg`
   and `.snupkg`.
3. A final job diffs the two manifests for the `.dll` and `.pdb` files and **fails** if
   any hash differs. The package files are left out of the comparison because they
   embed the pack time (see above).

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
source. The same holds for every other target framework, and for the `.pdb` files
in the `.snupkg`. Do not compare the `.nupkg` file's own hash: it embeds the pack
time, so it differs on every build.

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
- the SHA-256 of every published `.nupkg` / `.snupkg` (informational: it identifies the
  exact file released, but cannot be reproduced because the package embeds its pack time),
- the SHA-256 of each assembly (`lib/**/*.dll`, `lib/**/*.pdb`) *inside* those packages
  (the `contents` of each artifact), which is what you can reproduce.

The `contents` hashes are the authoritative list of expected hashes — you don't have to
eyeball individual `sha256sum` output. To verify a release end to end:

```bash
# 1. Check out the tagged source and pack it with the pinned SDK (see the note above).
git checkout v<version>
CI=true dotnet pack src/Wolfgang.Etl.Transformers/Wolfgang.Etl.Transformers.csproj \
  -c Release -p:ContinuousIntegrationBuild=true -p:Deterministic=true -o artifacts

# 2. Regenerate the manifest from your own build.
python3 scripts/generate-repro-manifest.py --packages artifacts --out my-manifest.json \
  --repository Chris-Wolfgang/ETL-Transformers --tag v<version>

# 3. Download the published manifest from the GitHub Release and diff the hashes.
#    (repository/commit/tag/sdk metadata may differ; the per-assembly `contents` hashes
#    must match. The package files' own `sha256` is excluded: it embeds the pack time.)
curl -sSL -o released-manifest.json \
  https://github.com/Chris-Wolfgang/ETL-Transformers/releases/download/v<version>/reproducible-build-manifest.json
diff \
  <(jq -S '[.artifacts[] | {file, contents}]' released-manifest.json) \
  <(jq -S '[.artifacts[] | {file, contents}]' my-manifest.json)
```

An empty `diff` is proof that the assemblies and symbols you can rebuild from source are
byte-identical to the ones that were published.

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
