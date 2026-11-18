type: feature

Every transformer now implements `ITransformWithCancellationAsync`, adding a `TransformAsync(items, token)` overload that forwards the token to the source. The cancellation-aware `Then` overload now composes any chain of this package's transformers, not only `PassThroughTransformer<T>` and `ThrottleTransformer<T>`.
