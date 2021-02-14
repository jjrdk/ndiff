namespace NDiff
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the result of comparing an item against a previous version.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of item being compared.</typeparam>
    public readonly struct Compared<T> : IEquatable<Compared<T>>
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

        /// <inheritdoc />
        public bool Equals(Compared<T> other)
        {
            return Change == other.Change && EqualityComparer<T>.Default.Equals(Item, other.Item);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Compared<T> other && Equals(other);
        }

        public static bool operator ==(Compared<T>? instance, Compared<T>? other)
        {
            if (instance is null || other is null)
            {
                return false;
            }
            return instance.Equals(other);
        }

        public static bool operator !=(Compared<T>? instance, Compared<T>? other)
        {
            if (instance is null || other is null)
            {
                return true;
            }
            return !instance.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return new { Change, Item }.GetHashCode();
        }
    }
}
