namespace NDiff.Tests
{
    using System;

    public class TestItem : IEquatable<TestItem>
    {
        public string Text { get; set; }

        public int Value { get; set; }

        public bool Equals(TestItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Text, other.Text) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is TestItem ti)
            {
                return Equals(ti);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Text != null ? Text.GetHashCode() : 0) * 397) ^ Value;
            }
        }
    }
}