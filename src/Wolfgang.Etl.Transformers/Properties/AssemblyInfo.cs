using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Wolfgang.Etl.Transformers.Tests.Unit")]

// The Native AOT run of the same unit tests (tests/Wolfgang.Etl.Transformers.Tests.Aot links
// Tests.Unit's sources), so it needs the same access to internals.
[assembly: InternalsVisibleTo("Wolfgang.Etl.Transformers.Tests.Aot")]