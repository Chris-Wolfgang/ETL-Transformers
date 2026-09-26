type: feature

`CastTransformer` accepts the same optional `DelegateTransformerOptions`, so an item that does not convert can be dropped and counted rather than ending the run. This differs from `OfTypeTransformer`, which drops mismatches silently: the policy form records each failure in `CurrentErrorItemCount` and hands it to the policy, which can log it or route it to a dead-letter collection.
