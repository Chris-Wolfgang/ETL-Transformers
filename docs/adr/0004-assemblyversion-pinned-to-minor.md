# ADR-0004: Pin `AssemblyVersion` to `major.minor.0.0`

- **Status**: Accepted (mechanism amended in 0.7.0 - see "Update: derived, not hand-pinned" below)
- **Date**: 2026-07-20
- **Deciders**: @Chris-Wolfgang

## Context

The csproj carries two distinct versions:

```xml
<Version>0.2.1</Version>          <!-- NuGet package + informational version -->
<AssemblyVersion>0.2.0.0</AssemblyVersion>
```

On .NET Framework, the CLR binds by the strong-name `AssemblyVersion`. If
`AssemblyVersion` changed on every patch release, a consumer that compiled
against `0.2.0` would need a binding redirect to load `0.2.1` at runtime — a
notorious source of "could not load file or assembly … version" failures in
LOB apps that pull the library in transitively.

## Decision

`AssemblyVersion` is pinned to `major.minor.0.0` and only bumped on a
minor/major release. The patch component lives in the NuGet `Version` (and
`FileVersion`), which do **not** participate in CLR binding. Patch releases are
therefore binary drop-in replacements: `0.2.0`, `0.2.1`, `0.2.2` all present
`AssemblyVersion 0.2.0.0`.

## Consequences

- **Positive** — patch upgrades need no binding redirects on .NET Framework;
  transitive consumers pick up fixes without recompiling or editing config.
- **Negative / accepted trade-off** — two assemblies with the same
  `AssemblyVersion` but different `FileVersion` can coexist confusingly at
  diagnosis time; you must read `FileVersion`/`InformationalVersion` to know the
  exact build. A genuinely binary-breaking change **must** be released as at
  least a minor bump so `AssemblyVersion` moves.
- **Neutral** — irrelevant to `net5.0+`, which does not bind on
  `AssemblyVersion`; the pin exists for the .NET Framework targets (see
  [ADR-0003](0003-multi-tfm-targeting.md)).

## Alternatives considered

- **Let `AssemblyVersion` track the full `Version`** — strictest binding, but
  forces a binding redirect (or recompile) on every patch for .NET Framework
  consumers.

## Update: derived, not hand-pinned (0.7.0)

The decision stands, but `AssemblyVersion` is no longer a hand-maintained literal like the
`0.2.0.0` shown under Context. Since 0.7.0 the csproj derives it from `<Version>`, following
ETL-Abstractions ADR-0011:

```xml
<Version>0.8.1</Version>
<AssemblyVersion>0.$([System.Version]::Parse($(Version.Split('-')[0])).Minor).0.0</AssemblyVersion>
```

So `0.8.0` and `0.8.1` both present `AssemblyVersion 0.8.0.0`, a minor bump moves it
automatically, and a release-prep PR cannot forget to update it. Pre-1.0 the minor is SemVer's
breaking position, which is why the derivation keys on the minor.
