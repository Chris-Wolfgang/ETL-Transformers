# Reproducible Builds

`Wolfgang.Etl.Transformers` is built **deterministically**: compiling the same
commit **with the same OS and SDK** produces byte-for-byte identical outputs,
regardless of the machine or working directory. This lets anyone independently
confirm that a published package was built from the tagged source and contains no
hidden modifications.

## The guarantee

For a given commit built **on the same operating system with the same SDK**, the
following outputs are bit-for-bit identical:

- the compiled assembly `Wolfgang.Etl.Transformers.dll` (every target framework),
- its debug symbols `Wolfgang.Etl.Transformers.pdb`,
- the produced `.nupkg` / `.snupkg` package.

> **Cross-OS is advisory, not guaranteed.** Building the same commit on a
> *different* operating system may currently produce different bytes — .NET does
> not guarantee cross-OS byte-identity (embedded source ordering, path
> normalization, and ref/targeting-pack differences can leak). The CI check reports
> cross-OS divergence as a warning, and closing that gap is tracked in
> [#193](https://github.com/Chris-Wolfgang/ETL-Transformers/issues/193).
> Reproduce a release on the **same OS** the release was built on (see below).

## How it is achieved

Determinism is configured in [`Directory.Build.props`](Directory.Build.props):

| Setting | Effect |
| --- | --- |
| `Deterministic` (SDK default `true`) | Removes timestamps and other non-deterministic metadata from the assembly. |
| `ContinuousIntegrationBuild=true` (set when `CI=true`) | Enables `DeterministicSourcePaths`, so absolute source paths are replaced with stable, machine-independent tokens. |
| `Microsoft.SourceLink.GitHub` + `EmbedUntrackedSources` | Maps source paths to their canonical repository location, which is what makes the PDB reproducible across operating systems. |

`ContinuousIntegrationBuild` + SourceLink path normalization removes the
machine- and directory-specific differences, which is what makes builds
reproducible on a given OS. Cross-OS byte-identity additionally depends on factors
.NET does not pin (see the advisory note above).

## Continuous verification

The [`Reproducible Build`](.github/workflows/reproducible-build.yaml) workflow
checks reproducibility on every pull request:

1. It packs the project on **both** `ubuntu-latest` and `windows-latest` with
   `ContinuousIntegrationBuild=true`.
2. Each job records the `sha256sum` of every produced `.dll`, `.pdb`, `.nupkg`
   and `.snupkg`.
3. A final job diffs the two manifests. Divergence is reported as an **advisory
   warning** (not a failure), because cross-OS byte-identity is not yet
   guaranteed; closing that gap is tracked as a follow-up before the check is
   flipped to hard-fail.

Same-OS reproducibility is additionally enforced implicitly: two builds of the same
commit on the same runner OS/SDK produce identical hashes.

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
