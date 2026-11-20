type: fix

`Distinct` and `DistinctBy` on `IEtlPipeline<T>` now have an explicit overload without the comparer instead of an optional `comparer = null` parameter, so the overload with the most parameters is the one that carries optional parameters (RS0027). Calls such as `pipeline.Distinct()` and `pipeline.DistinctBy(x => x.Id)` compile unchanged; code that was compiled against the earlier optional-parameter signature should be rebuilt.
