namespace NDiff
{
    using System;

    /// <summary>
    /// Defines the diff entry type.
    /// </summary>
    public struct DiffEntry : IEquatable<DiffEntry>
    {
        private readonly int _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffEntry"/> class.
        /// </summary>
        /// <param name="startSource">The start index in the source sequence.</param>
        /// <param name="startCompared">The start index in the compared sequence.</param>
        /// <param name="deletedSource">The index of the deleted item in the source sequence.</param>
        /// <param name="insertedCompared">The index of the deleted item in the compared sequence.</param>
        public DiffEntry(int startSource, int startCompared, int deletedSource, int insertedCompared)
        {
            StartSource = startSource;
            StartCompared = startCompared;
            DeletedSource = deletedSource;
            InsertedCompared = insertedCompared;
            _hash = new
            {
                StartA = StartSource,
                StartB = StartCompared,
                DeletedA = DeletedSource,
                InsertedB = InsertedCompared
            }.GetHashCode();
        }

        /// <summary>
        /// Gets the start index in the source sequence.
        /// </summary>
        public int StartSource { get; }

        /// <summary>
        /// Gets the start index in the compared sequence.
        /// </summary>
        public int StartCompared { get; }

        /// <summary>
        /// Gets the index of the deleted item in the source sequence.
        /// </summary>
        public int DeletedSource { get; }

        /// <summary>
        /// Gets the index of the deleted item in the compared sequence.
        /// </summary>
        public int InsertedCompared { get; }

        /// <inheritdoc />
        public bool Equals(DiffEntry other)
        {
            return StartSource == other.StartSource
                   && StartCompared == other.StartCompared
                   && DeletedSource == other.DeletedSource
                   && InsertedCompared == other.InsertedCompared;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is DiffEntry other && Equals(other);
        }

        public static bool operator ==(DiffEntry instance, DiffEntry other)
        {
            return instance.Equals(other);
        }

        public static bool operator !=(DiffEntry instance, DiffEntry other)
        {
            return !instance.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _hash;
        }
    }
}
