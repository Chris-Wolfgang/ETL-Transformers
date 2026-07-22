using System;
using Wolfgang.Etl.Transformers;

// End-to-end SourceLink "step into" fixture. The debugger sets a breakpoint on
// the line marked STEP_INTO_TARGET and issues a step-into (the F11 a consumer
// would press). If SourceLink is intact the debugger resolves the library's real
// source (from GitHub) at the constructor below, instead of a decompiled
// placeholder. ChunkTransformer's constructor is a plain, non-async method, which
// makes it a clean and stable step-into target.

int size = 1000;
var transformer = new ChunkTransformer<int>(size); // STEP_INTO_TARGET
Console.WriteLine(transformer.GetType().FullName);
