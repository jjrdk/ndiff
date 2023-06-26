namespace NDiff
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the result of comparing an item against a previous version.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of item being compared.</typeparam>
    public readonly record struct Compared<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Compared{T}"/> type.
        /// </summary>
        /// <param name="item">The compared item.</param>
        /// <param name="change">The change action.</param>
        public Compared(T item, ChangeAction change)
        {
            Item = item;
            Change = change;
        }

        /// <summary>
        /// Gets the change action.
        /// </summary>
        public ChangeAction Change { get; }

        /// <summary>
        /// Gets the compared item.
        /// </summary>
        public T Item { get; }
    }
}
