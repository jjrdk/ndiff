namespace NDiff.Tests;

using System;
using Xunit;

public class PatchWriterTests
{
    [Fact]
    public void CanGeneratePatchFromStrings()
    {
        const string text1 = """
                             line1
                             line2
                             line3
                             """;
        const string text2 = """
                             line1
                             line2
                             lineX
                             """;

        var deltas = text1.Split(Environment.NewLine, StringSplitOptions.TrimEntries)
            .CreateDelta(text2.Split(Environment.NewLine, StringSplitOptions.TrimEntries));

        var patch = text1.ToPatchString(deltas);

        const string expected = """
                                diff
                                @@ -2,1 +2,1 @@
                                    line1
                                line2
                                +   lineX
                                -   line3

                                """;

        Assert.Equal(expected, patch);
    }

    [Fact]
    public void CanGeneratePatchFromObjects()
    {
        TestItem[] source = [new TestItem("test", 1), new TestItem("test", 2), new TestItem("test", 3)];
        TestItem[] target = [new TestItem("test", 1), new TestItem("test", 2), new TestItem("test", 4)];

        var deltas = source.CreateDelta(target);

        var patch = source.AsMemory().ToPatchString(deltas);

        const string expected = """
                                diff
                                @@ -2, 1 + 2, 1 @@
                                line1
                                line2
                                + lineX
                                - line3
                                """;
    }
}
