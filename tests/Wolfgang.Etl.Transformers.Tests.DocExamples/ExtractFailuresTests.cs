using System.IO;
using Xunit;

namespace Wolfgang.Etl.Transformers.Tests.DocExamples;

/// <summary>
/// Direct unit tests for <see cref="DocExampleCompilationTests.ExtractFailures"/> —
/// the seam extracted from the main test so its failure-reporting path can be exercised
/// in isolation (#225). The main test's real-source scan finds no rot in green CI, so
/// without these tests the extract-failures branch is dead in coverage.
/// </summary>
public class ExtractFailuresTests
{
    [Fact]
    public void ExtractFailures_when_source_has_a_broken_doc_example_returns_a_formatted_diagnostic()
    {
        // A minimal source with one XML-doc `<example><code>` block that will not compile:
        // deliberately unterminated method declaration.
        const string source = """
            using System;

            namespace Fake;

            /// <summary>Broken example.</summary>
            /// <example>
            /// <code>
            /// class Broken { thisIsNotValidCsharp
            /// </code>
            /// </example>
            public class Foo { }
            """;

        var (examplesChecked, failures) = DocExampleCompilationTests.ExtractFailures(source, "Fake.cs");

        Assert.Equal(1, examplesChecked);
        Assert.Single(failures);
        Assert.Contains("Fake.cs", failures[0]);
    }


    [Fact]
    public void ExtractFailures_when_source_has_no_doc_examples_returns_no_failures_and_zero_checked()
    {
        const string source = """
            namespace Fake;
            public class Bar { }
            """;

        var (examplesChecked, failures) = DocExampleCompilationTests.ExtractFailures(source, "Fake.cs");

        Assert.Equal(0, examplesChecked);
        Assert.Empty(failures);
    }


    [Fact]
    public void ExtractFailures_when_source_has_a_clean_doc_example_returns_no_failures()
    {
        // A snippet the compiler-and-collect logic wraps + compiles successfully:
        // valid statements inside the harness's async Task RunAsync body.
        const string source = """
            using System;

            namespace Fake;

            /// <summary>Clean example.</summary>
            /// <example>
            /// <code>
            /// var x = 1 + 2;
            /// Console.WriteLine(x);
            /// </code>
            /// </example>
            public class Foo { }
            """;

        var (examplesChecked, failures) = DocExampleCompilationTests.ExtractFailures(source, "Fake.cs");

        Assert.Equal(1, examplesChecked);
        Assert.Empty(failures);
    }


    [Fact]
    public void ExtractFailures_when_doc_example_uses_elision_ellipsis_skips_it()
    {
        // ContainsElision filters out snippets containing "..." — a documentation convention
        // for "and so on". They are not real code and can't be compiled.
        const string source = """
            using System;

            namespace Fake;

            /// <summary>Elided example.</summary>
            /// <example>
            /// <code>
            /// var x = ...;
            /// </code>
            /// </example>
            public class Foo { }
            """;

        var (examplesChecked, failures) = DocExampleCompilationTests.ExtractFailures(source, "Fake.cs");

        Assert.Equal(0, examplesChecked);
        Assert.Empty(failures);
    }


    [Fact]
    public void FindSourceDirectory_when_no_ancestor_contains_src_throws_DirectoryNotFoundException()
    {
        // A temp directory chain that does NOT contain "src/Wolfgang.Etl.Transformers"
        // in any ancestor exercises the walk's defensive fallthrough — the branch the
        // in-repo test run can never reach because tests always run from inside the repo.
        var isolatedRoot = Path.Combine(Path.GetTempPath(), "etl-transformers-tests-" + Path.GetRandomFileName());
        Directory.CreateDirectory(isolatedRoot);
        try
        {
            var ex = Assert.Throws<DirectoryNotFoundException>
            (
                () => DocExampleCompilationTests.FindSourceDirectory(isolatedRoot)
            );
            Assert.Contains(isolatedRoot, ex.Message);
        }
        finally
        {
            Directory.Delete(isolatedRoot);
        }
    }
}
