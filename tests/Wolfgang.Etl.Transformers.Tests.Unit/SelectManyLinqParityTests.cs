using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;



namespace Wolfgang.Etl.Transformers.Tests.Unit;

/// <summary>
/// With the default options, <see cref="SelectManyTransformer{TSource, TDestination}"/> must behave
/// like LINQ's <c>SelectMany</c>: the same items in the same order, the same exception when
/// something fails, and the same inner sequences disposed. Each scenario runs through LINQ and
/// through the transformer and compares the two outcomes.
/// </summary>
/// <remarks>
/// The synchronous-selector constructor is compared with <c>Enumerable.SelectMany</c> and the
/// asynchronous-selector constructor with <c>AsyncEnumerable.SelectMany</c>. Disposal is compared
/// as the number of distinct inner enumerators disposed, because LINQ's synchronous
/// <c>SelectMany</c> disposes a failed inner enumerator twice.
/// </remarks>
public class SelectManyLinqParityTests
{
    private static readonly int[] DefaultOuter = { 0, 1, 2, 3 };



    // Shared by several scenarios. "empty outer" never calls its selector, so it must not own a
    // lambda of its own: that lambda would never run.
    private static readonly Func<int, DisposalLog, IEnumerable<int>?> PlainInner =
        (i, log) => new ScriptedSequence(i, log);



    private static readonly Dictionary<string, Scenario> ScenarioTable = new()
    {
        ["plain"] = new Scenario(PlainInner),
        ["empty outer"] = new Scenario(PlainInner, outer: Array.Empty<int>()),
        ["every inner sequence empty"] = new Scenario((_, log) => new ScriptedSequence(0, log)),
        ["selector throws"] = new Scenario
        (
            (i, log) => i == 2 ? throw new InvalidOperationException("selector") : new ScriptedSequence(i, log)
        ),
        ["selector returns null"] = new Scenario((i, log) => i == 2 ? null : new ScriptedSequence(i, log)),
        ["inner MoveNext throws"] = new Scenario
        (
            (i, log) => i == 2 ? new ScriptedSequence(3, log, moveNextThrowsAt: 2) : new ScriptedSequence(i, log)
        ),
        ["inner Current throws"] = new Scenario
        (
            (i, log) => i == 2 ? new ScriptedSequence(2, log, currentThrowsAt: 1) : new ScriptedSequence(i, log)
        ),
        ["inner Dispose throws"] = new Scenario
        (
            (i, log) => new ScriptedSequence(i, log, disposeThrows: i == 1)
        ),
        ["inner MoveNext and Dispose throw"] = new Scenario
        (
            (i, log) => i == 2
                ? new ScriptedSequence(3, log, moveNextThrowsAt: 2, disposeThrows: true)
                : new ScriptedSequence(i, log)
        ),
        ["inner Current and Dispose throw"] = new Scenario
        (
            (i, log) => i == 2
                ? new ScriptedSequence(2, log, currentThrowsAt: 1, disposeThrows: true)
                : new ScriptedSequence(i, log)
        ),
        ["outer source throws"] = new Scenario(PlainInner, outerThrowsAfter: 1),
        ["consumer stops early"] = new Scenario((i, log) => new ScriptedSequence(i + 2, log), stopAfter: 2),
    };



    public static TheoryData<string> Scenarios
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var name in ScenarioTable.Keys)
            {
                data.Add(name);
            }

            return data;
        }
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task TransformAsync_with_sync_selector_matches_Enumerable_SelectMany(string scenarioName)
    {
        var scenario = ScenarioTable[scenarioName];

        var linq = Run
        (
            log => OuterSync(scenario)
                .SelectMany(i => scenario.Inner(i, log)!),
            scenario
        );
        var ours = await RunAsync
        (
            log => new SelectManyTransformer<int, int>(i => scenario.Inner(i, log)!)
                .TransformAsync(OuterAsync(scenario)),
            scenario
        );

        Assert.Equal(linq, ours);
    }



    [Theory]
    [MemberData(nameof(Scenarios))]
    public async Task TransformAsync_with_async_selector_matches_AsyncEnumerable_SelectMany(string scenarioName)
    {
        var scenario = ScenarioTable[scenarioName];

        var linq = await RunAsync
        (
            log => OuterAsync(scenario)
                .SelectMany(i => AsyncView.Of(scenario.Inner(i, log))),
            scenario
        );
        var ours = await RunAsync
        (
            log => new SelectManyTransformer<int, int>(i => AsyncView.Of(scenario.Inner(i, log)))
                .TransformAsync(OuterAsync(scenario)),
            scenario
        );

        Assert.Equal(linq, ours);
    }



    [Fact]
    public void ScriptedSequence_non_generic_members_behave_like_the_generic_ones()
    {
        // LINQ and the transformer only use the generic surface; this keeps the helper's
        // non-generic interface members honest rather than leaving them unexercised.
        IEnumerable sequence = new ScriptedSequence(1, new DisposalLog());
        var enumerator = sequence.GetEnumerator();
        using (enumerator as IDisposable)
        {
            Assert.True(enumerator.MoveNext());
            Assert.Equal(10, enumerator.Current);
            Assert.Throws<NotSupportedException>(() => enumerator.Reset());
        }
    }



    private static string Run(Func<DisposalLog, IEnumerable<int>> build, Scenario scenario)
    {
        var log = new DisposalLog();
        var items = new List<int>();
        var error = "none";
        try
        {
            foreach (var item in build(log))
            {
                items.Add(item);
                if (items.Count == scenario.StopAfter)
                {
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            error = Describe(exception);
        }

        return Outcome(items, error, log);
    }



    private static async Task<string> RunAsync(Func<DisposalLog, IAsyncEnumerable<int>> build, Scenario scenario)
    {
        var log = new DisposalLog();
        var items = new List<int>();
        var error = "none";
        try
        {
            await foreach (var item in build(log))
            {
                items.Add(item);
                if (items.Count == scenario.StopAfter)
                {
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            error = Describe(exception);
        }

        return Outcome(items, error, log);
    }



    private static string Describe(Exception exception) => $"{exception.GetType().Name}: {exception.Message}";



    private static string Outcome(List<int> items, string error, DisposalLog log) =>
        $"items [{string.Join(",", items)}]; error {error}; disposed {log.DistinctDisposed}";



    private static IEnumerable<int> OuterSync(Scenario scenario)
    {
        for (var index = 0; index < scenario.Outer.Length; index++)
        {
            if (index == scenario.OuterThrowsAfter + 1 && scenario.OuterThrowsAfter >= 0)
            {
                throw new InvalidOperationException("outer");
            }

            yield return scenario.Outer[index];
        }
    }



    private static async IAsyncEnumerable<int> OuterAsync(Scenario scenario)
    {
        await Task.Yield();
        foreach (var item in OuterSync(scenario))
        {
            yield return item;
        }
    }



    /// <summary>
    /// One parity scenario: the inner sequence each source item expands to, the source items,
    /// where the source itself fails (-1 for never), and when the consumer stops (null for never).
    /// </summary>
    private sealed class Scenario
    {
        public Scenario
        (
            Func<int, DisposalLog, IEnumerable<int>?> inner,
            int[]? outer = null,
            int outerThrowsAfter = -1,
            int? stopAfter = null
        )
        {
            Inner = inner;
            Outer = outer ?? DefaultOuter;
            OuterThrowsAfter = outerThrowsAfter;
            StopAfter = stopAfter;
        }



        public Func<int, DisposalLog, IEnumerable<int>?> Inner { get; }



        public int[] Outer { get; }



        public int OuterThrowsAfter { get; }



        public int? StopAfter { get; }
    }



    /// <summary>
    /// Records which inner enumerators were disposed, counting each one once.
    /// </summary>
    private sealed class DisposalLog
    {
        private readonly List<object> _disposed = new();



        public int DistinctDisposed => _disposed.Count;



        // By reference: a second disposal of the same enumerator is not a second enumerator.
        public void Record(object enumerator)
        {
            if (!_disposed.Exists(disposed => ReferenceEquals(disposed, enumerator)))
            {
                _disposed.Add(enumerator);
            }
        }
    }



    /// <summary>
    /// An inner sequence of <c>count</c> items (10, 20, ...) whose enumerator can be told to throw
    /// from <c>MoveNext</c> or <c>Current</c> at a given position, or from <c>Dispose</c>.
    /// </summary>
    private sealed class ScriptedSequence : IEnumerable<int>
    {
        private readonly int _count;
        private readonly DisposalLog _log;
        private readonly int _moveNextThrowsAt;
        private readonly int _currentThrowsAt;
        private readonly bool _disposeThrows;



        public ScriptedSequence
        (
            int count,
            DisposalLog log,
            int moveNextThrowsAt = -1,
            int currentThrowsAt = -1,
            bool disposeThrows = false
        )
        {
            _count = count;
            _log = log;
            _moveNextThrowsAt = moveNextThrowsAt;
            _currentThrowsAt = currentThrowsAt;
            _disposeThrows = disposeThrows;
        }



        public IEnumerator<int> GetEnumerator() => new ScriptedEnumerator(this);



        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



        private sealed class ScriptedEnumerator : IEnumerator<int>
        {
            private readonly ScriptedSequence _sequence;
            private int _position;



            public ScriptedEnumerator(ScriptedSequence sequence)
            {
                _sequence = sequence;
            }



            public int Current => _position == _sequence._currentThrowsAt
                ? throw new InvalidOperationException("current")
                : _position * 10;



            object IEnumerator.Current => Current;



            public bool MoveNext()
            {
                _position++;
                if (_position == _sequence._moveNextThrowsAt)
                {
                    throw new InvalidOperationException("move next");
                }

                return _position <= _sequence._count;
            }



            public void Reset() => throw new NotSupportedException();



            public void Dispose()
            {
                _sequence._log.Record(this);
                if (_sequence._disposeThrows)
                {
                    FailDisposal();
                }
            }



            // The fault this double exists to inject: an inner sequence whose cleanup fails.
            private static void FailDisposal() => throw new InvalidOperationException("dispose");
        }
    }



    /// <summary>
    /// An asynchronous view of an inner sequence that keeps its enumerator's own
    /// <c>MoveNext</c>, <c>Current</c> and <c>Dispose</c> behaviour, so both asynchronous
    /// implementations see exactly the failures the synchronous ones do.
    /// </summary>
    private sealed class AsyncView : IAsyncEnumerable<int>
    {
        private readonly IEnumerable<int> _source;



        private AsyncView(IEnumerable<int> source)
        {
            _source = source;
        }



        // A null inner sequence stays null, so it fails where a null IEnumerable would.
        public static IAsyncEnumerable<int> Of(IEnumerable<int>? source) =>
            source is null ? null! : new AsyncView(source);



        public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
            new Enumerator(_source.GetEnumerator());



        private sealed class Enumerator : IAsyncEnumerator<int>
        {
            private readonly IEnumerator<int> _inner;



            public Enumerator(IEnumerator<int> inner)
            {
                _inner = inner;
            }



            public int Current => _inner.Current;



            public ValueTask<bool> MoveNextAsync() => new(_inner.MoveNext());



            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return default;
            }
        }
    }
}
