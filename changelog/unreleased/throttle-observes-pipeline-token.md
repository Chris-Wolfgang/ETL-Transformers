type: fix

`ThrottleTransformer<T>` implements `ITransformWithCancellationAsync`, so its pacing delay observes the `EtlPipeline` cancellation token.
