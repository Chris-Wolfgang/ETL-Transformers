type: fix

The token overloads of `Select`, `Where`, `SelectMany`, `Cast`, `OfType`, `Distinct`, `DistinctBy`, `Skip`, `SkipWhile`, `Take`, `TakeWhile`, `Chunk` and `ProgressReporting` now observe the token themselves, before the first pull and per item, so a cancelled run throws `OperationCanceledException` instead of draining a source that ignores the token, matching `PassThrough`, `Throttle` and `Buffered`.
