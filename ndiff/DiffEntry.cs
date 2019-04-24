namespace NDiff
{
    using System;

    public struct DiffEntry : IEquatable<DiffEntry>
    {
        private readonly int _hash;

        public DiffEntry(int startA, int startB, int deletedA, int insertedB)
        {
            StartA = startA;
            StartB = startB;
            DeletedA = deletedA;
            InsertedB = insertedB;
            _hash = new { StartA, StartB, DeletedA, InsertedB }.GetHashCode();
        }

        public int StartA { get; }
        public int StartB { get; }
        public int DeletedA { get; }
        public int InsertedB { get; }

        public bool Equals(DiffEntry other)
        {
            return StartA == other.StartA && StartB == other.StartB && DeletedA == other.DeletedA && InsertedB == other.InsertedB;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DiffEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _hash;
        }
    }
}
