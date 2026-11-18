type: fix

`CurrentErrorItemCount` is now per run: it restarts at zero when a transformer is enumerated again while no other enumeration of it is active, so re-running a pipeline no longer reports the earlier run's skips in `EtlPipelineProgress.ErrorItemCount`. Enumerations that overlap in time still share one count.
