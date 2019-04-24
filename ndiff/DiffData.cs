namespace NDiff
{
    internal class DiffData
    {
        internal DiffData(int[] data)
        {
            Data = data;
            Length = data.Length;
            Modified = new bool[Length + 2];
        }

        internal int Length { get; }

        internal int[] Data { get; }

        internal bool[] Modified { get; }
    }
}
