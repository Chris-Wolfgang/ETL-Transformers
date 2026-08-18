using System.Threading.Tasks;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;

namespace Wolfgang.Etl.Transformers.Tests.Unit;

// The test-code IS held to 100% line coverage (#218). CountingSource lives in TestHelpers
// because three transformer tests use it to prove a pre-cancelled token stops the source
// from being pulled — a scenario in which PullCount never advances past 0, so those tests
// don't cover the Enumerate iterator itself. This suite exercises the happy path so the
// helper's own behavior is verified and its state machine is covered.
public class TestHelpersTests
{
    [Fact]
    public async Task CountingSource_Enumerate_yields_expected_items_and_counts_each_pull()
    {
        var source = new CountingSource(3);

        var collected = await CollectAsync(source.Enumerate());

        Assert.Equal(new[] { 0, 1, 2 }, collected);
        Assert.Equal(3, source.PullCount);
    }


    [Fact]
    public async Task CountingSource_Enumerate_when_item_count_is_zero_yields_nothing()
    {
        var source = new CountingSource(0);

        var collected = await CollectAsync(source.Enumerate());

        Assert.Empty(collected);
        Assert.Equal(0, source.PullCount);
    }
}
