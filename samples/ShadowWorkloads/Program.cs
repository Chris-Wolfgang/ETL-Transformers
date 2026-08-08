using BenchmarkDotNet.Running;

// Shadow-testing entry point. `dotnet run -c Release -- --filter "*"` runs every scenario.
BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

// Named type so BenchmarkSwitcher.FromAssembly has a stable anchor (top-level Program).
internal sealed partial class Program
{
}
