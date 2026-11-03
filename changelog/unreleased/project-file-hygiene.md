type: internal

Package metadata: `PackageTags` now describe the current operators, and `Authors` / `Copyright` / `RepositoryType` come from `Directory.Build.props` (the copyright year is no longer hard-coded). Test projects drop unused package references and use `xunit.runner.visualstudio` 3.0.1 on net8.0+.
