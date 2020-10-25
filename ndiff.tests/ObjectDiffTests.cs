namespace NDiff.Tests
{
    using System.Collections.Generic;
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
                new[] {ChangeAction.Unchanged, ChangeAction.Unchanged, ChangeAction.Removed, ChangeAction.Added},
                formattedCollection);
        }

        [Theory]
        [MemberData(nameof(DeltaSequences))]
        public void Deltas(int[] sequence1, int[] sequence2)
        {
            var deltas = sequence1.CreateDelta(sequence2);
            var applied = sequence1.ApplyDeltas(deltas);

            Assert.Equal(sequence2, applied);
        }

        public static IEnumerable<object[]> DeltaSequences()
        {
            yield return new object[] {new[] {1, 2, 3, 5, 6, 8, 9}, new[] {1, 2, 4, 6, 7, 8, 9}};
            yield return new object[] {new[] {3, 5, 6, 8, 9}, new[] {1, 2, 4, 6, 7, 8, 9}};
            yield return new object[] {new[] {1, 2, 3, 5, 6, 8, 9, 10}, new[] {1, 2, 4, 6, 7, 8, 9}};
            yield return new object[] {new[] {1, 2, 3, 5, 6, 8, 9, 10}, new[] {1, 2, 4, 6, 7, 8, 9, 11}};
            yield return new object[] {new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, new[] {6, 7, 8, 9, 11}};
        }
    }
}
