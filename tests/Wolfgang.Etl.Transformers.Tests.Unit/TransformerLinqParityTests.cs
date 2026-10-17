using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// With the default options, each transformer that takes an error policy must behave like its LINQ
/// counterpart: the same items in the same order, the same exception when something fails, the same
/// number of delegate calls, the same number of source items pulled, and the source disposed in the
/// same way. Each scenario runs through LINQ and through the transformer and compares the outcomes.
/// </summary>
/// <remarks>
/// Both the synchronous-delegate and the <see cref="ValueTask{TResult}"/>-delegate constructors are
/// compared with the synchronous <c>Enumerable</c> operator: an asynchronous delegate must not change
/// what the operator does. <see cref="SelectManyTransformer{TSource, TDestination}"/> has its own
/// LINQ-parity suite.
/// </remarks>
public class TransformerLinqParityTests
{
    private static readonly int[] Numbers = { 1, 2, 2, 3, 1, 4, 5 };



    // The value at which a delegate throws in the "delegate throws" scenario. Not a const: a const
    // declaration never executes, which would leave its line uncovered.
    private static readonly int FailingValue = 3;



    private static readonly Dictionary<string, Scenario> ScenarioTable = new()
    {
        ["normal"] = new Scenario(),
        ["delegate throws"] = new Scenario(delegateThrows: true),
        ["source throws"] = new Scenario(sourceThrowsAt: 4),
        ["consumer stops early"] = new Scenario(stopAfter: 2),
        ["empty source"] = new Scenario(empty: true),
    };



    public static TheoryData<string> Scenarios => Names(ScenarioTable.Keys);



    // For operators with no delegate of their own, where "delegate throws" would repeat "normal".
    public static TheoryData<string> ScenariosWithoutDelegate =>
        Names(ScenarioTable.Keys.Where(name => !ScenarioTable[name].DelegateThrows));



    private static readonly Dictionary<string, CastCase> CastTable = new()
    {
        ["all castable"] = new CastCase(new object[] { "a", "b", "c" }),
        ["int among strings"] = new CastCase(new object[] { "a", "b", 3, "d" }),
        ["boxed int to long"] = new CastCase(new object[] { 1, 2 }, toLong: true),
        ["source throws"] = new CastCase(new object[] { "a", "b", "c" }, sourceThrowsAt: 1),
        ["consumer stops early"] = new CastCase(new object[] { "a", "b", "c" }, stopAfter: 1),
        ["empty source"] = new CastCase(Array.Empty<object>()),
    };



    public static TheoryData<string> CastScenarios => Names(CastTable.Keys);



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task SelectTransformer_with_sync_selector_matches_Enumerable_Select(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .Select(x => Times10(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new SelectTransformer<int, int>(x => Times10(p, s, x))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task SelectTransformer_with_ValueTask_selector_matches_Enumerable_Select(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .Select(x => Times10(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new SelectTransformer<int, int>(x => LaterAsync(() => Times10(p, s, x)))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task WhereTransformer_with_sync_predicate_matches_Enumerable_Where(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .Where(x => IsEven(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new WhereTransformer<int>(x => IsEven(p, s, x))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task WhereTransformer_with_ValueTask_predicate_matches_Enumerable_Where(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .Where(x => IsEven(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new WhereTransformer<int>(x => LaterAsync(() => IsEven(p, s, x)))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task SkipWhileTransformer_with_sync_predicate_matches_Enumerable_SkipWhile(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .SkipWhile(x => IsBelowFailingValue(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new SkipWhileTransformer<int>(x => IsBelowFailingValue(p, s, x))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task SkipWhileTransformer_with_ValueTask_predicate_matches_Enumerable_SkipWhile(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .SkipWhile(x => IsBelowFailingValue(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new SkipWhileTransformer<int>(x => LaterAsync(() => IsBelowFailingValue(p, s, x)))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task TakeWhileTransformer_with_sync_predicate_matches_Enumerable_TakeWhile(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .TakeWhile(x => IsBelowFailingValue(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new TakeWhileTransformer<int>(x => IsBelowFailingValue(p, s, x))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task TakeWhileTransformer_with_ValueTask_predicate_matches_Enumerable_TakeWhile(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .TakeWhile(x => IsBelowFailingValue(p, s, x)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new TakeWhileTransformer<int>(x => LaterAsync(() => IsBelowFailingValue(p, s, x)))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task DistinctTransformer_with_custom_comparer_matches_Enumerable_Distinct(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .Distinct(new ModuloThreeComparer(p, s)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new DistinctTransformer<int>(new ModuloThreeComparer(p, s))
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(ScenariosWithoutDelegate))]
    public async Task DistinctTransformer_with_default_comparer_matches_Enumerable_Distinct(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];

        var linq = Run
        (
            p => p.Source(s.Items, s.SourceThrowsAt)
                .Distinct(),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new DistinctTransformer<int>()
                .TransformAsync(p.SourceAsync(s.Items, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



#if NET6_0_OR_GREATER
    // Enumerable.DistinctBy exists from .NET 6, so these two run on net6.0 and later only.
    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task DistinctByTransformer_with_default_comparer_matches_Enumerable_DistinctBy(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];
        var words = s.Words;

        var linq = Run
        (
            p => p.Source(words, s.SourceThrowsAt)
                .DistinctBy(w => LowerKey(p, s, w)),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new DistinctByTransformer<string, string>(w => LowerKey(p, s, w))
                .TransformAsync(p.SourceAsync(words, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task DistinctByTransformer_with_custom_comparer_matches_Enumerable_DistinctBy(string scenarioName)
    {
        var s = ScenarioTable[scenarioName];
        var words = s.Words;

        var linq = Run
        (
            p => p.Source(words, s.SourceThrowsAt)
                .DistinctBy(w => SameKey(p, s, w), StringComparer.OrdinalIgnoreCase),
            s.StopAfter
        );
        var ours = await RunAsync
        (
            p => new DistinctByTransformer<string, string>(w => SameKey(p, s, w), StringComparer.OrdinalIgnoreCase)
                .TransformAsync(p.SourceAsync(words, s.SourceThrowsAt)),
            s.StopAfter
        );

        Assert.Equal(linq, ours);
    }



    private static string LowerKey(Probe probe, Scenario scenario, string word) =>
        SameKey(probe, scenario, word).ToLowerInvariant();



    private static string SameKey(Probe probe, Scenario scenario, string word)
    {
        probe.Call();
        return scenario.DelegateThrows && string.Equals(word, "c", StringComparison.Ordinal)
            ? throw new InvalidOperationException($"key at {word}")
            : word;
    }
#endif



    [Theory]
    [MemberData(nameof(CastScenarios))]
    public async Task CastTransformer_matches_Enumerable_Cast(string scenarioName)
    {
        var c = CastTable[scenarioName];

        var (linq, ours) = c.ToLong
            ? (
                Run
                (
                    p => p.Source(c.Items, c.SourceThrowsAt)
                        .Cast<long>(),
                    c.StopAfter
                ),
                await RunAsync
                (
                    p => new CastTransformer<object, long>()
                        .TransformAsync(p.SourceAsync(c.Items, c.SourceThrowsAt)),
                    c.StopAfter
                )
            )
            : (
                Run
                (
                    p => p.Source(c.Items, c.SourceThrowsAt)
                        .Cast<string>(),
                    c.StopAfter
                ),
                await RunAsync
                (
                    p => new CastTransformer<object, string>()
                        .TransformAsync(p.SourceAsync(c.Items, c.SourceThrowsAt)),
                    c.StopAfter
                )
            );

        Assert.Equal(linq, ours);
    }



    private static TheoryData<string> Names(IEnumerable<string> names)
    {
        var data = new TheoryData<string>();
        foreach (var name in names)
        {
            data.Add(name);
        }

        return data;
    }



    private static int Times10(Probe probe, Scenario scenario, int x)
    {
        probe.Call();
        return scenario.Fails(x) ? throw new InvalidOperationException($"delegate at {x}") : x * 10;
    }



    private static bool IsEven(Probe probe, Scenario scenario, int x)
    {
        probe.Call();
        return scenario.Fails(x) ? throw new InvalidOperationException($"delegate at {x}") : x % 2 == 0;
    }



    // True for 1 and 2, false from the first 3: SkipWhile and TakeWhile must stop asking there.
    private static bool IsBelowFailingValue(Probe probe, Scenario scenario, int x)
    {
        probe.Call();
        return scenario.Fails(x) ? throw new InvalidOperationException($"delegate at {x}") : x < FailingValue;
    }



    private static async ValueTask<T> LaterAsync<T>(Func<T> work)
    {
        await Task.Yield();
        return work();
    }



    private static string Run<T>(Func<Probe, IEnumerable<T>> build, int? stopAfter)
    {
        var probe = new Probe();
        var items = new List<T>();
        var error = "none";
        try
        {
            foreach (var item in build(probe))
            {
                items.Add(item);
                if (items.Count == stopAfter)
                {
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            error = $"{exception.GetType().Name}: {exception.Message}";
        }

        return probe.Describe(items, error);
    }



    private static async Task<string> RunAsync<T>(Func<Probe, IAsyncEnumerable<T>> build, int? stopAfter)
    {
        var probe = new Probe();
        var items = new List<T>();
        var error = "none";
        try
        {
            await foreach (var item in build(probe))
            {
                items.Add(item);
                if (items.Count == stopAfter)
                {
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            error = $"{exception.GetType().Name}: {exception.Message}";
        }

        return probe.Describe(items, error);
    }



    /// <summary>
    /// One parity scenario over <see cref="Numbers"/> (or no items at all).
    /// </summary>
    private sealed class Scenario
    {
        public Scenario(bool delegateThrows = false, int sourceThrowsAt = -1, int? stopAfter = null, bool empty = false)
        {
            DelegateThrows = delegateThrows;
            SourceThrowsAt = sourceThrowsAt;
            StopAfter = stopAfter;
            Items = empty ? Array.Empty<int>() : Numbers;
#if NET6_0_OR_GREATER
            Words = empty ? Array.Empty<string>() : new[] { "a", "B", "b", "c", "C", "d", "A" };
#endif
        }



        public bool DelegateThrows { get; }



        public int SourceThrowsAt { get; }



        public int? StopAfter { get; }



        public int[] Items { get; }



#if NET6_0_OR_GREATER
        // Only the DistinctBy tests use words, and they need .NET 6.
        public string[] Words { get; }
#endif



        public bool Fails(int x) => DelegateThrows && x == FailingValue;
    }



    /// <summary>
    /// One <see cref="CastTransformer{TSource, TDestination}"/> scenario: the source objects, whether
    /// to cast to <see cref="long"/> instead of <see cref="string"/>, where the source fails and when
    /// the consumer stops.
    /// </summary>
    private sealed class CastCase
    {
        public CastCase(object[] items, bool toLong = false, int sourceThrowsAt = -1, int? stopAfter = null)
        {
            Items = items;
            ToLong = toLong;
            SourceThrowsAt = sourceThrowsAt;
            StopAfter = stopAfter;
        }



        public object[] Items { get; }



        public bool ToLong { get; }



        public int SourceThrowsAt { get; }



        public int? StopAfter { get; }
    }



    /// <summary>
    /// Counts delegate calls and source pulls and records whether the source was disposed, so both
    /// sides can be compared on what they did and not only on what they returned.
    /// </summary>
    private sealed class Probe
    {
        private int _calls;
        private int _pulls;
        private bool _sourceDisposed;



        public void Call() => _calls++;



        public string Describe<T>(List<T> items, string error) =>
            $"items [{string.Join(",", items)}]; error {error}; calls {_calls}; pulls {_pulls}; "
            + $"source disposed {_sourceDisposed}";



        public IEnumerable<T> Source<T>(T[] items, int throwsAt)
        {
            try
            {
                for (var index = 0; index < items.Length; index++)
                {
                    if (index == throwsAt)
                    {
                        throw new InvalidOperationException("source");
                    }

                    _pulls++;
                    yield return items[index];
                }
            }
            finally
            {
                _sourceDisposed = true;
            }
        }



        public async IAsyncEnumerable<T> SourceAsync<T>(T[] items, int throwsAt)
        {
            await Task.Yield();
            foreach (var item in Source(items, throwsAt))
            {
                yield return item;
            }
        }
    }



    /// <summary>
    /// Treats integers as equal when they leave the same remainder modulo 3, counting each hash as a
    /// delegate call. In the "delegate throws" scenario, hashing the failing value throws.
    /// </summary>
    private sealed class ModuloThreeComparer : IEqualityComparer<int>
    {
        private readonly Probe _probe;
        private readonly Scenario _scenario;



        public ModuloThreeComparer(Probe probe, Scenario scenario)
        {
            _probe = probe;
            _scenario = scenario;
        }



        public bool Equals(int x, int y) => x % 3 == y % 3;



        public int GetHashCode(int obj)
        {
            _probe.Call();
            if (_scenario.Fails(obj))
            {
                Fail(obj);
            }

            return obj % 3;
        }



        // The fault this comparer exists to inject, outside GetHashCode itself.
        private static void Fail(int value) => throw new InvalidOperationException($"comparer at {value}");
    }
}
