namespace NDiff.Tests
{
    using Xunit;

    public class StringDiffTests
    {
        [Fact]
        public void EqualSequenceAnalysis()
        {
            var text1 = @"line1
line2
line3";
            var text2 = @"line1
line2
line3";

            var diffs = StringDiffExtensions.DiffText(text1, text2);

            Assert.Empty(diffs);
        }

        [Fact]
        public void LongSequenceAnalysis()
        {
            var text1 = @"line1
line2
line3
line4";
            var text2 = @"line1
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
            var text1 = @"line1
line2
line3";
            var text2 = @"line1
line2
lineX";

            var diffs = StringDiffExtensions.DiffText(text1, text2);

            Assert.Single(diffs);
            Assert.Equal(new DiffEntry(2, 2, 1, 1), diffs[0]);
        }

        [Fact]
        public void HtmlTextFormatting()
        {
            var text1 = @"line1
line2
line3";
            var text2 = @"line1
line2
lineX";

            var html = text1.FormatDiffTextAsHtml(text2, null, null, true, true, true);

            Assert.Equal("line1<br/>line2<br/><del>line3</del><br/><b>lineX</b>", html);
        }

        [Fact]
        public void HtmlCharFormatting()
        {
            var text1 = "text1";
            var text2 = "text2";

            var html = text1.FormatDiffCharsAsHtml(text2);

            Assert.Equal("text<del>1</del><em>2</em>", html);
        }
    }
}
