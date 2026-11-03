type: internal

One guard spelling and one error-policy implementation: `ThrottleTransformer<T>` and the `IEtlPipeline<T>` operators use the polyfilled `ArgumentNullException.ThrowIfNull` / `ArgumentOutOfRangeException.ThrowIfLessThan` (a negative `minInterval` now reports the runtime's standard "must be greater than or equal to" message, with the same `ParamName` and `ActualValue` as before), and the eight `DelegateTransformerOptions` transformers share an internal `ItemErrorCounter` instead of eight copies of the policy-and-counter code. No public API change.
