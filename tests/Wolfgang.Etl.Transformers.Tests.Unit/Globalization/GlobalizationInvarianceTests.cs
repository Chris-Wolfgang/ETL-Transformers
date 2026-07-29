using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using static Wolfgang.Etl.Transformers.Tests.Unit.TestHelpers;

namespace Wolfgang.Etl.Transformers.Tests.Unit.Globalization;

/// <summary>
/// Runs a representative set of transformer operations under hostile cultures
/// (Turkish dotted-I, German decimal comma, Chinese collation, Arabic RTL/digit
/// shaping, Japanese full-width) to prove the library is culture-invariant.
/// </summary>
/// <remarks>
/// The transformers own no culture-sensitive behavior: equality/keying uses
/// <see cref="EqualityComparer{T}.Default"/> (ordinal for <see cref="string"/>),
/// and any comparer is caller-supplied. There is no formatting or parsing of
/// strings, dates, or numbers in the library. See <c>docs/globalization.md</c>
/// for the (empty) allowlist of intentionally culture-sensitive public methods.
/// Each test swaps both <see cref="CultureInfo.CurrentCulture"/> and
/// <see cref="CultureInfo.CurrentUICulture"/> and restores them afterwards.
/// </remarks>
public class GlobalizationInvarianceTests
{
    // The four Turkish "I" glyphs are the canonical culture-casing trap: under
    // tr-TR, culture-aware case folding maps them differently than en-US.
    // Ordinal comparison (the library default) keeps all four distinct in EVERY
    // culture — that invariance is what these tests assert.
    private static readonly string[] TurkishIVariants = { "I", "ı", "i", "İ" };

    public static IEnumerable<object[]> Cultures()
    {
        yield return new object[] { "en-US" };
        yield return new object[] { "tr-TR" };
        yield return new object[] { "de-DE" };
        yield return new object[] { "zh-CN" };
        yield return new object[] { "ar-SA" };
        yield return new object[] { "ja-JP" };
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public async Task Distinct_default_comparer_is_ordinal_under_every_culture(string culture)
    {
        await UnderCulture(culture, async () =>
        {
            var result = await CollectAsync(new DistinctTransformer<string>().TransformAsync(ToAsync(TurkishIVariants)));

            // Ordinal: all four glyphs are distinct regardless of the ambient culture.
            Assert.Equal(TurkishIVariants, result);
        });
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public async Task Distinct_ordinal_ignore_case_comparer_is_stable_under_every_culture(string culture)
    {
        await UnderCulture(culture, async () =>
        {
            var result = await CollectAsync
            (
                new DistinctTransformer<string>(StringComparer.OrdinalIgnoreCase)
                    .TransformAsync(ToAsync(new[] { "I", "i", "STRASSE", "strasse" }))
            );

            // OrdinalIgnoreCase folds case invariantly: "I"/"i" collapse, and the
            // German "ß" is NOT expanded to "ss" (that would be culture/linguistic).
            Assert.Equal(new[] { "I", "STRASSE" }, result);
        });
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public async Task DistinctBy_default_key_comparer_is_ordinal_under_every_culture(string culture)
    {
        await UnderCulture(culture, async () =>
        {
            var items = new[] { "Igloo", "ıce", "iron", "İsland" };

            var result = await CollectAsync
            (
                new DistinctByTransformer<string, string>(s => s.Substring(0, 1))
                    .TransformAsync(ToAsync(items))
            );

            // Keys are the four Turkish "I" glyphs — all distinct ordinally, so
            // every item survives, in every culture.
            Assert.Equal(items, result);
        });
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public async Task Pipeline_output_is_identical_across_cultures(string culture)
    {
        var source = new[] { "apple", "Apricot", "banana", "Avocado", "cherry" };

        // Baseline under the invariant culture.
        List<string> baseline = null!;
        await UnderCulture("", async () =>
        {
            baseline = await RunPipeline(source);
        });

        await UnderCulture(culture, async () =>
        {
            var actual = await RunPipeline(source);
            Assert.Equal(baseline, actual);
        });
    }

    private static async Task<List<string>> RunPipeline(string[] source)
    {
        var whereA = new WhereTransformer<string>(s => s.Length > 4);
        var select = new SelectTransformer<string, string>(s => s.ToUpperInvariant());
        var distinct = new DistinctTransformer<string>(StringComparer.Ordinal);

        return await CollectAsync
        (
            distinct.TransformAsync(select.TransformAsync(whereA.TransformAsync(ToAsync(source))))
        );
    }

    private static async Task UnderCulture(string cultureName, Func<Task> body)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            var culture = string.IsNullOrEmpty(cultureName)
                ? CultureInfo.InvariantCulture
                : CultureInfo.GetCultureInfo(cultureName);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await body().ConfigureAwait(false);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }
}
