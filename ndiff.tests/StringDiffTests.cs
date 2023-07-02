namespace NDiff.Tests
{
    using System;
    using Xunit;

    public class StringDiffTests
    {
        [Fact]
        public void EqualSequenceAnalysis()
        {
            const string text1 = @"line1
line2
line3";
            const string text2 = @"line1
line2
line3";

            var diffs = StringDiffExtensions.DiffText(text1, text2);

            Assert.Empty(diffs);
        }

        [Fact]
        public void CaseInsensitiveEqualSequenceAnalysis()
        {
            const string text1 = @"liNe1
linE2
line3";
            const string text2 = @"line1
line2
line3";

            var diffs = StringDiffExtensions.DiffText(text1, text2, equalityComparer: StringComparer.OrdinalIgnoreCase);

            Assert.Empty(diffs);
        }

        [Fact]
        public void LongSequenceAnalysis()
        {
            const string text1 = @"line1
line2
line3
line4";
            const string text2 = @"line1
line2
lineX
line4";

            var diffs = StringDiffExtensions.DiffText(text1, text2);

            Assert.Single(diffs);
            Assert.Equal(new DiffEntry(2, 2, 1, 1), diffs[0]);
        }

        [Fact]
        public void ShortSequenceAnalysis()
        {
            const string text1 = @"line1
line2
line3";
            const string text2 = @"line1
line2
lineX";

            var diffs = StringDiffExtensions.DiffText(text1, text2);

            Assert.Single(diffs);
            Assert.Equal(new DiffEntry(2, 2, 1, 1), diffs[0]);
        }

        [Fact]
        public void HtmlTextFormatting()
        {
            const string text1 = @"line1
line2
line3";
            const string text2 = @"line1
line2
lineX";

            var html = text1.FormatDiffTextAsHtml(text2, null, null, true, true, true);

            Assert.Equal("line1<br/>line2<br/><del>line3</del><br/><b>lineX</b>", html);
        }

        [Fact]
        public void HtmlCharFormatting()
        {
            const string text1 = "text1";
            const string text2 = "text2";

            var html = text1.FormatDiffCharsAsHtml(text2);

            Assert.Equal("text<del>1</del><em>2</em>", html);
        }
    }
}
