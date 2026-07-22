# Third-party notices

`Wolfgang.Etl.Transformers` (MIT) is distributed with the following packages
(direct **and** transitive). All are permissively licensed. Build-time-only
analyzer and tooling packages (`PrivateAssets="all"`) are not distributed and
are omitted; see [.github/license-audit/](.github/license-audit/README.md).

This table is produced by the [`license-audit.yaml`](.github/workflows/license-audit.yaml)
workflow (`nuget-license --include-transitive`). Regenerate it when a runtime
dependency changes. Baseline reflects `main` at the time the audit was
introduced.

| Package | Version | License |
| --- | --- | --- |
| Microsoft.Bcl.AsyncInterfaces | 10.0.9 | MIT |
| Microsoft.Build.Tasks.Git | 10.0.300 | MIT |
| Microsoft.NETCore.Platforms | 1.1.0 | MS-EULA |
| NETStandard.Library | 2.0.3 | MIT |
| System.Buffers | 4.6.1 | MIT |
| System.ComponentModel.Annotations | 5.0.0 | MIT |
| System.IO.Hashing | 10.0.8 | MIT |
| System.Memory | 4.6.3 | MIT |
| System.Numerics.Vectors | 4.6.1 | MIT |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 | MIT |
| System.Threading.Channels | 10.0.9 | MIT |
| System.Threading.Tasks.Extensions | 4.6.3 | MIT |
| Wolfgang.Etl.Abstractions | 0.15.0 | MIT |

> All packages are published by Microsoft / the .NET Foundation under MIT,
> except `Microsoft.NETCore.Platforms` (a legacy runtime-targeting package)
> whose license is Microsoft's .NET Library EULA. `Wolfgang.Etl.Abstractions`
> is a sibling package in this project family, also MIT.
