type: fix

An `OperationCanceledException` thrown by a delegate is no longer passed to the per-item `ErrorPolicy`, so cancellation propagates instead of being skipped and counted.
