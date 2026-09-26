type: feature

The delegate-invoking transformers accept an optional `DelegateTransformerOptions` carrying an `ErrorPolicy`, so an exception thrown for a single item can skip that item and continue instead of ending the run. `SelectTransformer` and `SelectManyTransformer` adopt it first and report the dropped count through `IReportsItemErrors.CurrentErrorItemCount`. Constructed without the record, behaviour is unchanged and the exception still propagates.
