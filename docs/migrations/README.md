# Migration guides

Per-major-version upgrade guides for `Wolfgang.Etl.Transformers`.

The library is currently **0.x** — no major version with breaking changes has
shipped yet, so there are no migration guides to list here. This folder and its
[template](TEMPLATE-major-version-migration.md) exist so the convention is ready
the moment the first breaking release is prepared.

## Convention

- When a release introduces breaking changes (0.x → next 0.x during pre-1.0, or
  any `X.0` major after 1.0), copy `TEMPLATE-major-version-migration.md` to
  `vX-to-vY.md` and fill it in **during release prep** — not after the release
  ships. Retrofitting a migration guide after consumers have already hit the
  break is far more expensive.
- Each guide inventories every breaking change with a before/after code sample
  and the reasoning (link the corresponding [ADR](../adr/) where one exists).
- Link the finished guide from the GitHub Release notes for that version.

## Guides

_None yet — the first lands with the first breaking release._
