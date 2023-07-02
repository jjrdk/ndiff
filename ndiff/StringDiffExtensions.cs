// Adapted from the original code at http://www.mathertel.de/Diff/

// ReSharper disable CognitiveComplexity

namespace NDiff
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the string diff extensions.
    /// </summary>
    ///
#if NET7_0_OR_GREATER
    public static partial class StringDiffExtensions
    {
        [GeneratedRegex("\\s+", RegexOptions.Multiline | RegexOptions.Compiled)]
        private static partial Regex Spaces();
#else
    public static class StringDiffExtensions
    {
        private static readonly Regex Spaces = new Regex("\\s+", RegexOptions.Multiline | RegexOptions.Compiled);
#endif

        /// <summary>
        /// Performs a difference calculation on the passed texts
        /// </summary>
        /// <param name="textSource">The source text.</param>
        /// <param name="textCompared">The compared text.</param>
        /// <param name="trimSpace">Sets whether to trim whitespace.</param>
        /// <param name="ignoreSpace">Sets whether to ignore whitespace.</param>
        /// <param name="ignoreCase">Sets whether to ignore case.</param>
        /// <param name="equalityComparer">Sets the <see cref="IEqualityComparer{T}"/> for the text comparison.</param>
        /// <returns>An array of <see cref="DiffEntry"/>/</returns>
        public static DiffEntry[] DiffText(
            string textSource,
            string textCompared,
            bool trimSpace = false,
            bool ignoreSpace = false,
            bool ignoreCase = false,
            IEqualityComparer<string>? equalityComparer = null)
        {
            var h = new Dictionary<string, int>(textSource.Length + textCompared.Length, equalityComparer);
            var diffData1 = new DiffData(DiffCodes(textSource, h, trimSpace, ignoreSpace, ignoreCase));
            var diffData2 = new DiffData(DiffCodes(textCompared, h, trimSpace, ignoreSpace, ignoreCase));
            h.Clear();
            var num = diffData1.Length + diffData2.Length + 1;
            Span<int> downVector = stackalloc int[2 * num + 2];
            Span<int> upVector = stackalloc int[2 * num + 2];
            Lcs(diffData1, 0, diffData1.Length, diffData2, 0, diffData2.Length, downVector, upVector);
            Optimize(diffData1);
            Optimize(diffData2);
            return CreateDiffs(diffData1, diffData2);
        }

        /// <summary>
        /// Performs a difference comparison of characters in two strings.
        /// </summary>
        /// <param name="charA">The source character array.</param>
        /// <param name="charB">The compared character array.</param>
        /// <returns>An array of <see cref="DiffEntry"/>/</returns>
        public static DiffEntry[] DiffChars(ReadOnlySpan<char> charA, ReadOnlySpan<char> charB)
        {
            Span<int> arrayA = stackalloc int[charA.Length];
            Span<int> arrayB = stackalloc int[charB.Length];
            for (var i = 0; i < charA.Length; i++)
            {
                arrayA[i] = charA[i];
            }

            for (var i = 0; i < charB.Length; i++)
            {
                arrayB[i] = charB[i];
            }

            return DiffInt(arrayA, arrayB);
        }

        /// <summary>
        /// Performs a difference comparison of characters in two strings.
        /// </summary>
        /// <param name="arrayA">The source integer array.</param>
        /// <param name="arrayB">The compared integer array.</param>
        /// <returns>An array of <see cref="DiffEntry"/>/</returns>
        public static DiffEntry[] DiffInt(Span<int> arrayA, Span<int> arrayB)
        {
            var dataA = new DiffData(arrayA);
            var dataB = new DiffData(arrayB);
            var num = dataA.Length + dataB.Length + 1;
            Span<int> downVector = stackalloc int[2 * num + 2];
            Span<int> upVector = stackalloc int[2 * num + 2];
            Lcs(dataA, 0, dataA.Length, dataB, 0, dataB.Length, downVector, upVector);
            return CreateDiffs(dataA, dataB);
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

        private static Span<int> DiffCodes(
            string aText,
            Dictionary<string, int> h,
            bool trimSpace,
            bool ignoreSpace,
            bool ignoreCase)
        {
            var count = h.Count;
            var strArray = aText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Span<int> numArray = new int[strArray.Length];
            for (var i = 0; i < strArray.Length; ++i)
            {
                var index2 = strArray[i];
                if (trimSpace)
                {
                    index2 = index2.Trim();
                }
                else
                {
                    index2 = index2.TrimEnd('\r');
                }

                if (ignoreSpace)
                {
#if NET7_0_OR_GREATER
                    index2 = Spaces().Replace(index2, " ");
#else
                    index2 = Spaces.Replace(index2, " ");
#endif
                }

                if (ignoreCase)
                {
                    index2 = index2.ToLower();
                }

                if (!h.TryGetValue(index2, out var num))
                {
                    ++count;
                    h[index2] = count;
                    numArray[i] = count;
                }
                else
                {
                    numArray[i] = num;
                }
            }

            return numArray;
        }

        private static (int x, int y) Sms(
            DiffData dataA,
            int lowerA,
            int upperA,
            DiffData dataB,
            int lowerB,
            int upperB,
            Span<int> downVector,
            Span<int> upVector)
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
                        return new(downVector[num4 + num7], downVector[num4 + num7] - num7);
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
                        return new(downVector[num4 + num8], downVector[num4 + num8] - num8);
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
            Span<int> downVector,
            Span<int> upVector)
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
                var (x, y) = Sms(dataA, lowerA, upperA, dataB, lowerB, upperB, downVector, upVector);
                Lcs(dataA, lowerA, x, dataB, lowerB, y, downVector, upVector);
                Lcs(dataA, x, upperA, dataB, y, upperB, downVector, upVector);
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
    }
}
