namespace NDiff
{
    using System;
    using System.Collections.Generic;

    public readonly struct Compared<T> : IEquatable<Compared<T>>
    {
        public Compared(T item, ChangeAction change)
        {
            Item = item;
            Change = change;
        }

        public ChangeAction Change { get; }

        public T Item { get; }

        /// <inheritdoc />
        public bool Equals(Compared<T> other)
        {
            return Change == other.Change && EqualityComparer<T>.Default.Equals(Item, other.Item);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Compared<T> other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return new { Change, Item }.GetHashCode();
        }
    }
}