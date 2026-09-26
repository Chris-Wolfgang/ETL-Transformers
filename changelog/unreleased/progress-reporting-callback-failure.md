type: docs

`ProgressReportingTransformer` now documents that a throwing callback ends the run without delivering the failing item, why it does not take an `ErrorPolicy`, and how to keep a run alive by handling failures inside the callback.
