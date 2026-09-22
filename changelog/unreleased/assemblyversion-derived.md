type: fix

`AssemblyVersion` now moves with the minor as ADR-0011 requires (this release binds as `0.7.0.0`); v0.5.0 and v0.6.0 shipped with a stale `0.4.0.0` identity. It is derived from `<Version>` from now on so it cannot fall behind again.
