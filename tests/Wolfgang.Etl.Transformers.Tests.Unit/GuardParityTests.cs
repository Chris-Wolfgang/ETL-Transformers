using System;
using System.Globalization;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// The argument guards must throw the same exception on every target framework: on .NET 6.0+ /
/// .NET 8.0+ the runtime's <c>ThrowIfNull</c> / <c>ThrowIfLessThan</c>, elsewhere the internal
/// polyfills. The per-operator tests already pin <c>ParamName</c>; these pin <c>ActualValue</c>
/// and the message format so a divergence between polyfill and runtime on any slot fails.
/// </summary>
public class GuardParityTests
{
    /// <summary>
    /// The runtime formats the values in its message with the current culture; the expectation must too.
    /// </summary>
    private static string ExpectedMessagePrefix(string paramName, int actual, int floor) =>
        string.Format(CultureInfo.CurrentCulture, "{0} ('{1}') must be greater than or equal to '{2}'.", paramName, actual, floor);



    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ChunkTransformer_when_size_is_below_1_throws_with_ParamName_ActualValue_and_the_runtime_message(int size)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new ChunkTransformer<int>(size));

        Assert.Equal("size", ex.ParamName);
        Assert.Equal(size, ex.ActualValue);
        Assert.StartsWith(ExpectedMessagePrefix("size", size, 1), ex.Message, StringComparison.Ordinal);
    }



    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void BufferedTransformer_when_capacity_is_below_1_throws_with_ParamName_ActualValue_and_the_runtime_message(int capacity)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new BufferedTransformer<int>(capacity));

        Assert.Equal("capacity", ex.ParamName);
        Assert.Equal(capacity, ex.ActualValue);
        Assert.StartsWith(ExpectedMessagePrefix("capacity", capacity, 1), ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void Range_guard_when_the_current_culture_has_a_different_negative_sign_formats_the_message_with_that_culture()
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NegativeSign = "−";
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = culture;
        try
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new ChunkTransformer<int>(-1));

            Assert.StartsWith("size ('−1') must be greater than or equal to '1'.", ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }



    [Fact]
    public void WhereTransformer_when_predicate_is_null_throws_ArgumentNullException_naming_predicate()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new WhereTransformer<int>((Func<int, bool>)null!));

        Assert.Equal("predicate", ex.ParamName);
    }
}
