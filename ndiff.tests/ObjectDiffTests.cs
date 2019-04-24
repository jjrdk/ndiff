namespace NDiff.Tests
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Xunit;

    public class ObjectDiffTests
    {
        [Fact]
        public void ShortSequenceAnalysis()
        {
            var sequence1 = new[]
            {
                new TestItem {Text = "Item 1", Value = 1},
                new TestItem {Text = "Item 2", Value = 2},
                new TestItem {Text = "Item 3", Value = 3},
            };
            var sequence2 = new[]
            {
                new TestItem {Text = "Item 1", Value = 1},
                new TestItem {Text = "Item 2", Value = 2},
                new TestItem {Text = "Item 4", Value = 4},
            };

            var diff = sequence1.Diff(sequence2);

            Assert.Single(diff);
            Assert.Equal(new DiffEntry(2, 2, 1, 1), diff[0]);
        }

        [Fact]
        public void Formatting()
        {
            var sequence1 = new[]
            {
                new TestItem {Text = "Item 1", Value = 1},
                new TestItem {Text = "Item 2", Value = 2},
                new TestItem {Text = "Item 3", Value = 3},
            };
            var sequence2 = new[]
            {
                new TestItem {Text = "Item 1", Value = 1},
                new TestItem {Text = "Item 2", Value = 2},
                new TestItem {Text = "Item 4", Value = 4},
            };

            var diff = sequence1.Diff(sequence2);
            var formattedCollection = diff.Format(sequence1, sequence2).Select(x => x.Change);

            Assert.Equal(
                new[] { ChangeAction.Unchanged, ChangeAction.Unchanged, ChangeAction.Removed, ChangeAction.Added },
                formattedCollection);
        }
    }
}