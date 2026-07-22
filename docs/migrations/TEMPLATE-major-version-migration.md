# Migrating from vX to vY

> Copy this file to `vX-to-vY.md` when preparing a major release and fill in
> every section. Delete guidance blockquotes (like this one) as you go. Link
> the finished guide from the GitHub Release notes for vY.

`Wolfgang.Etl.Transformers` vY contains breaking changes relative to vX. This
guide lists every break, why it changed, and how to update your code.

## At a glance

| Area | vX | vY | Action |
| --- | --- | --- | --- |
| _e.g._ `WhereTransformer<T>` ctor | `new WhereTransformer<T>(pred)` | `new WhereTransformer<T>(pred, options)` | Pass `TransformerOptions.Default` |

## Breaking changes

> One subsection per break. Keep them ordered by how likely a consumer is to
> hit them (most common first).

### 1. <short title of the change>

**What changed** — <one or two sentences.>

**Why** — <the motivating reason: correctness, API consistency, perf, removing
a footgun. Link the ADR in `docs/adr/` if one exists.>

**Before (vX)**

```csharp
// old code
```

**After (vY)**

```csharp
// new code
```

## Deprecations (not yet removed)

> APIs still present in vY but marked `[Obsolete]`, with the version they are
> scheduled for removal. Consumers can migrate off these before the next major.

| API | Replacement | Removed in |
| --- | --- | --- |
| _e.g._ `OldMethodAsync` | `NewMethodAsync` | vZ |

## Deprecation timeline

- **vX** — <API introduced / last version before the break>.
- **vY** — <break lands; old API removed or `[Obsolete]`>.
- **vZ (planned)** — <obsolete APIs removed>.

## Need help?

Open a [discussion](https://github.com/Chris-Wolfgang/ETL-Transformers/discussions)
or [issue](https://github.com/Chris-Wolfgang/ETL-Transformers/issues) if a
migration step is unclear.
