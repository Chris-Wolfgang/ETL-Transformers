# Snapshot (approval) testing

`tests/Wolfgang.Etl.Transformers.Tests.Snapshots` uses
[Verify](https://github.com/VerifyTests/Verify) to lock the **shape** of
representative complex output so accidental format/behaviour drift is caught by a
diff against a committed `.verified.txt`.

## Scope (intentionally narrow)

The library is largely a **pure-data pass-through**: the transformers filter,
project, dedup, and batch generic `T` streams and render **no** text, JSON, XML,
or CSV. So snapshot testing has limited applicability — it is valuable only where
the *composed* output has an interesting shape. The committed snapshots cover:

- a **record/anonymous-object projection** (`Select`),
- a **chunk grouping** (`Chunk` — nested list structure), and
- an **end-to-end pipeline** (`DistinctBy` → `Select`).

Targeted unit tests remain the primary coverage; these snapshots are a
drift-detector on the structured shapes above.

## Working with snapshots

- Snapshots live next to the test under `Snapshots/` as `*.verified.txt` and are
  committed.
- On a change, the run writes `*.received.txt` and fails; review the diff and, if
  the new output is correct, replace the `.verified.txt` with it (or use your
  Verify diff tool). `*.received.*` is gitignored.
- CI just diffs against the committed `.verified.txt`. `DeterministicSourcePaths`
  is disabled for this project so Verify resolves the files by their real path on
  the runner.
