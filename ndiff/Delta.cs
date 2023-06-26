namespace NDiff
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the delta type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly record struct Delta<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Delta{T}"/> struct.
        /// </summary>
        /// <param name="diff">The <see cref="DiffEntry"/> describing the change.</param>
        /// <param name="added">The items added.</param>
        public Delta(DiffEntry diff, IEnumerable<T> added)
        {
            Diff = diff;
            Added = added;
        }

        /// <summary>
        /// Gets the <see cref="DiffEntry"/> describing the change.
        /// </summary>
        public DiffEntry Diff { get; }

        /// <summary>
        /// Gets the items added.
        /// </summary>
        public IEnumerable<T> Added { get; }
    }
}
