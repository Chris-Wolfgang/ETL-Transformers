using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Etl.Transformers.Tests.Concurrency;

/// <summary>
/// Fast xunit smoke over the concurrency <see cref="Scenarios"/>: runs each scenario many
/// times on the real scheduler so a race that reproduces easily is caught in normal CI.
/// The exhaustive systematic exploration is the weekly Coyote workflow via
/// <see cref="CoyoteEntryPoints"/>.
/// </summary>
[Trait("Category", "Concurrency")]
public sealed class ConcurrencyStressTests
{
    private const int Repetitions = 50;


    [Fact]
    public async Task ConcurrentEnumerations_do_not_interfere()
    {
        for (var i = 0; i < Repetitions; i++)
        {
            await Scenarios.TwoConcurrentEnumerationsAsync();
        }
    }


    [Fact]
    public async Task BufferedProducerConsumer_yields_every_item()
    {
        for (var i = 0; i < Repetitions; i++)
        {
            await Scenarios.BufferedProducerConsumerAsync();
        }
    }


    [Fact]
    public async Task CancellationDuringEnumeration_does_not_deadlock()
    {
        for (var i = 0; i < Repetitions; i++)
        {
            await Scenarios.CancellationDuringEnumerationAsync();
        }
    }


    // Companion to the above. Covers the race outcome where cancellation LOSES —
    // consumer completes before Cancel() runs. Same shared implementation as the
    // wins-the-race scenario, so the pair jointly exercises both branches of the
    // try/catch that swallows OCE (#225).
    [Fact]
    public async Task EnumerationBeforeLateCancellation_completes_normally_without_deadlock()
    {
        for (var i = 0; i < Repetitions; i++)
        {
            await Scenarios.EnumerationBeforeLateCancellationAsync();
        }
    }
}
