type: internal

Clear the InspectCode `AssignNullToNotNullAttribute` findings on the enumerator read in `SelectManyTransformer` with two line-scoped suppressions, the reason recorded above the method. No behavior change: the synchronous-selector path reads the inner enumerator's `Current` inside the error-policy guard, exactly as the asynchronous path does (this replaces the interim approaches of #411 and #417).
