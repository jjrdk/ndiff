namespace NDiff
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the diff calculation extensions for objects.
    /// </summary>
    public static class ObjectDiffExtensions
    {
        /// <summary>
        /// <para>
        /// Calculates the diff set between two sequences.
        /// </para>
        /// <para>If the type of object being compared does not implement <see cref="IEquatable{T}"/>, then an <see cref="IEqualityComparer{T}"/> should be provided to avoid comparing by reference.</para>
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of objects being compared.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="other">The different sequence.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>An array of <see cref="DiffEntry"/>.</returns>
        public static DiffEntry[] Diff<T>(
            this IReadOnlyList<T> source,
            IReadOnlyList<T> other,
            IEqualityComparer<T>? equalityComparer = null) where T : notnull
        {
            var h = new Dictionary<T, int>(source.Count + other.Count, equalityComparer);
            var diffData1 = new DiffData(DiffCodes(source, h));
            var diffData2 = new DiffData(DiffCodes(other, h));
            h.Clear();
            var num = diffData1.Length + diffData2.Length + 1;
            var downVector = new int[2 * num + 2];
            var upVector = new int[2 * num + 2];
            Lcs(diffData1, 0, diffData1.Length, diffData2, 0, diffData2.Length, downVector, upVector);
            Optimize(diffData1);
            Optimize(diffData2);
            return CreateDiffs(diffData1, diffData2);
        }

        /// <summary>
        /// Formats the source sequence with the changes from the different sequence.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of objects being compared.</typeparam>
        /// <param name="diffs">The array of <see cref="DiffEntry"/> to apply.</param>
        /// <param name="source">The source sequence.</param>
        /// <param name="other">The different sequence.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Compared{T}"/> items.</returns>
        public static IReadOnlyCollection<Compared<T>> Format<T>(this DiffEntry[] diffs, T[] source, T[] other)
        {
            var resultLines = new List<Compared<T>>();

            for (var x = 0; x < diffs.Length; x++)
            {
                var item = AddUntouchedLines(diffs, x, source, resultLines);
                AddDeletedLines(item, source, resultLines);
                AddInsertedLines(item, other, resultLines);
            }

            return resultLines;
        }

        public static Delta<T>[] CreateDelta<T>(
            this IReadOnlyList<T> source,
            IReadOnlyList<T> other,
            IEqualityComparer<T>? equalityComparer = null)
            where T : IEquatable<T>
        {
            return source.Diff(other, equalityComparer)
                .Select(d => new Delta<T>(d, other.Skip(d.StartCompared).Take(d.InsertedCompared).ToArray()))
                .ToArray();
        }

        public static T[] ApplyDeltas<T>(this IReadOnlyCollection<T> source, params Delta<T>[] diff)
        {
            var position = 0;
            var output = Enumerable.Empty<T>();
            foreach (var item in diff)
            {
                output = output.Concat(source.Skip(position).Take(item.Diff.StartSource - position).Concat(item.Added));
                position = item.Diff.StartSource + item.Diff.DeletedSource;
            }

            output = output.Concat(source.Skip(position));

            return output.ToArray();
        }

        private static void AddInsertedLines<T>(DiffEntry diffEntry, T[] lines, List<Compared<T>> resultLines)
        {
            var inserted = lines.Skip(diffEntry.StartCompared)
                .Take(diffEntry.InsertedCompared)
                .Select(x => new Compared<T>(x, ChangeAction.Added));

            resultLines.AddRange(inserted);
        }

        private static void AddDeletedLines<T>(DiffEntry diffEntry, T[] lines, List<Compared<T>> resultLines)
        {
            var deleted = lines.Skip(diffEntry.StartSource - 1)
                .Take(diffEntry.DeletedSource)
                .Select(x => new Compared<T>(x, ChangeAction.Removed));
            resultLines.AddRange(deleted);
        }

        private static DiffEntry AddUntouchedLines<T>(DiffEntry[] diff, int x, T[] lines, List<Compared<T>> resultLines)
        {
            var item = diff[x];
            var offset = x == 0 ? 0 : diff[x - 1].StartSource + diff[x - 1].DeletedSource;
            var count = item.StartSource - offset;
            var untouched = lines.Skip(offset).Take(count).Select(a => new Compared<T>(a, ChangeAction.Unchanged));

            resultLines.AddRange(untouched);
            return item;
        }

        private static void Optimize(DiffData data)
        {
            var index1 = 0;
            while (index1 < data.Length)
            {
                while (index1 < data.Length && !data.Modified[index1])
                {
                    ++index1;
                }

                var index2 = index1;
                while (index2 < data.Length && data.Modified[index2])
                {
                    ++index2;
                }

                if (index2 < data.Length && data.Data[index1] == data.Data[index2])
                {
                    data.Modified[index1] = false;
                    data.Modified[index2] = true;
                }
                else
                {
                    index1 = index2;
                }
            }
        }

        private static int[] DiffCodes<T>(IReadOnlyList<T> items, Dictionary<T, int> h) where T : notnull
        {
            var count = h.Count;
            var numArray = new int[items.Count];
            for (var index1 = 0; index1 < items.Count; ++index1)
            {
                var index2 = items[index1];

                if (!h.TryGetValue(index2, out var num))
                {
                    ++count;
                    h[index2] = count;
                    numArray[index1] = count;
                }
                else
                {
                    numArray[index1] = num;
                }
            }

            return numArray;
        }

        private static Smsrd Sms(
            DiffData dataA,
            int lowerA,
            int upperA,
            DiffData dataB,
            int lowerB,
            int upperB,
            int[] downVector,
            int[] upVector)
        {
            var num1 = dataA.Length + dataB.Length + 1;
            var num2 = lowerA - lowerB;
            var num3 = upperA - upperB;
            var flag = (uint)(upperA - lowerA - (upperB - lowerB) & 1) > 0U;
            var num4 = num1 - num2;
            var num5 = num1 - num3;
            var num6 = (upperA - lowerA + upperB - lowerB) / 2 + 1;
            downVector[num4 + num2 + 1] = lowerA;
            upVector[num5 + num3 - 1] = upperA;
            for (var index1 = 0; index1 <= num6; ++index1)
            {
                var num7 = num2 - index1;
                while (num7 <= num2 + index1)
                {
                    int index2;
                    if (num7 == num2 - index1)
                    {
                        index2 = downVector[num4 + num7 + 1];
                    }
                    else
                    {
                        index2 = downVector[num4 + num7 - 1] + 1;
                        if (num7 < num2 + index1 && downVector[num4 + num7 + 1] >= index2)
                        {
                            index2 = downVector[num4 + num7 + 1];
                        }
                    }

                    for (var index3 = index2 - num7;
                        index2 < upperA && index3 < upperB && dataA.Data[index2] == dataB.Data[index3];
                        ++index3)
                    {
                        ++index2;
                    }

                    downVector[num4 + num7] = index2;
                    if (flag
                        && num3 - index1 < num7
                        && (num7 < num3 + index1 && upVector[num5 + num7] <= downVector[num4 + num7]))
                    {
                        return new Smsrd(downVector[num4 + num7], downVector[num4 + num7] - num7);
                    }

                    num7 += 2;
                }

                var num8 = num3 - index1;
                while (num8 <= num3 + index1)
                {
                    int num9;
                    if (num8 == num3 + index1)
                    {
                        num9 = upVector[num5 + num8 - 1];
                    }
                    else
                    {
                        num9 = upVector[num5 + num8 + 1] - 1;
                        if (num8 > num3 - index1 && upVector[num5 + num8 - 1] < num9)
                        {
                            num9 = upVector[num5 + num8 - 1];
                        }
                    }

                    for (var index2 = num9 - num8;
                        num9 > lowerA && index2 > lowerB && dataA.Data[num9 - 1] == dataB.Data[index2 - 1];
                        --index2)
                    {
                        --num9;
                    }

                    upVector[num5 + num8] = num9;
                    if (!flag
                        && num2 - index1 <= num8
                        && (num8 <= num2 + index1 && upVector[num5 + num8] <= downVector[num4 + num8]))
                    {
                        return new Smsrd(downVector[num4 + num8], downVector[num4 + num8] - num8);
                    }

                    num8 += 2;
                }
            }

            throw new Exception("the algorithm should never come here.");
        }

        private static void Lcs(
            DiffData dataA,
            int lowerA,
            int upperA,
            DiffData dataB,
            int lowerB,
            int upperB,
            int[] downVector,
            int[] upVector)
        {
            for (; lowerA < upperA && lowerB < upperB && dataA.Data[lowerA] == dataB.Data[lowerB]; ++lowerB)
            {
                ++lowerA;
            }

            for (; lowerA < upperA && lowerB < upperB && dataA.Data[upperA - 1] == dataB.Data[upperB - 1]; --upperB)
            {
                --upperA;
            }

            if (lowerA == upperA)
            {
                while (lowerB < upperB)
                {
                    dataB.Modified[lowerB++] = true;
                }
            }
            else if (lowerB == upperB)
            {
                while (lowerA < upperA)
                {
                    dataA.Modified[lowerA++] = true;
                }
            }
            else
            {
                var smsrd = Sms(dataA, lowerA, upperA, dataB, lowerB, upperB, downVector, upVector);
                Lcs(dataA, lowerA, smsrd.X, dataB, lowerB, smsrd.Y, downVector, upVector);
                Lcs(dataA, smsrd.X, upperA, dataB, smsrd.Y, upperB, downVector, upVector);
            }
        }

        private static DiffEntry[] CreateDiffs(DiffData dataA, DiffData dataB)
        {
            var objList = new List<DiffEntry>();
            var index1 = 0;
            var index2 = 0;
            while (index1 < dataA.Length || index2 < dataB.Length)
            {
                if (index1 < dataA.Length
                    && !dataA.Modified[index1]
                    && (index2 < dataB.Length && !dataB.Modified[index2]))
                {
                    ++index1;
                    ++index2;
                }
                else
                {
                    var num1 = index1;
                    var num2 = index2;
                    while (index1 < dataA.Length && (index2 >= dataB.Length || dataA.Modified[index1]))
                    {
                        ++index1;
                    }

                    while (index2 < dataB.Length && (index1 >= dataA.Length || dataB.Modified[index2]))
                    {
                        ++index2;
                    }

                    if (num1 < index1 || num2 < index2)
                    {
                        objList.Add(new DiffEntry(num1, num2, index1 - num1, index2 - num2));
                    }
                }
            }

            return objList.ToArray();
        }

        private readonly struct Smsrd
        {
            public Smsrd(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }

            public int Y { get; }
        }
    }
}