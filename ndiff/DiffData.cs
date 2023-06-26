namespace NDiff
{
    using System;

    internal ref struct DiffData
    {
        internal DiffData(Span<int> data)
        {
            Data = data;
            Length = data.Length;
            Modified = new bool[Length + 2];
        }

        internal int Length { get; }

        internal Span<int> Data { get; }

        internal Span<bool> Modified { get; }
    }
}
