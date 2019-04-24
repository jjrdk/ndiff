namespace NDiff
{
    public readonly struct Compared<T>
    {
        public Compared(T item, ChangeAction change)
        {
            Item = item;
            Change = change;
        }

        public ChangeAction Change { get; }

        public T Item { get; }
    }
}