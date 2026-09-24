type: feature

`SkipWhileTransformer`, `TakeWhileTransformer` and `DistinctTransformer` accept the same optional `DelegateTransformerOptions` as the other delegate-invoking transformers. These three were missing from the original scope of the feature despite invoking a caller-supplied predicate or comparer per item.
