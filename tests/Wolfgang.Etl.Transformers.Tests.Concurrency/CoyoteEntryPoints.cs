using System.Threading.Tasks;
using Microsoft.Coyote.SystematicTesting;

namespace Wolfgang.Etl.Transformers.Tests.Concurrency;

/// <summary>
/// Coyote systematic-testing entry points. The weekly <c>concurrency.yaml</c> workflow
/// rewrites this assembly (<c>coyote rewrite</c>) and runs each method with a large
/// iteration budget (<c>coyote test ... -m &lt;method&gt; -i 10000</c>), exploring thousands
/// of schedule interleavings to surface races and deadlocks.
/// </summary>
public static class CoyoteEntryPoints
{
    [Test]
    public static Task ConcurrentEnumerations() => Scenarios.TwoConcurrentEnumerationsAsync();


    [Test]
    public static Task BufferedProducerConsumer() => Scenarios.BufferedProducerConsumerAsync();


    [Test]
    public static Task CancellationDuringEnumeration() => Scenarios.CancellationDuringEnumerationAsync();
}
