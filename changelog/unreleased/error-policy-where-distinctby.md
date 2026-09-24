type: feature

`WhereTransformer` and `DistinctByTransformer` accept the same optional `DelegateTransformerOptions` as the projecting transformers, so a predicate, key selector or comparer that throws for one item can skip that item instead of ending the run. An item merely filtered out, or dropped as a duplicate, is not counted as an error.
