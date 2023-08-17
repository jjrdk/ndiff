// ReSharper disable CognitiveComplexity

namespace NDiff
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DiffFormat
    {
        public static string FormatDiffTextAsHtml(
            this string a,
            string b,
            (ReadOnlyMemory<char>, ReadOnlyMemory<char>)? addFormatter = null,
            (ReadOnlyMemory<char>, ReadOnlyMemory<char>)? removeFormatter = null,
            bool trimSpace = false,
            bool ignoreSpace = false,
            IEqualityComparer<string>? comparer = null)
        {
            var diff = StringDiffExtensions.DiffText(a, b, trimSpace, ignoreSpace, equalityComparer: comparer);
            return FormatDiffTextAsHtml(a, b, diff, addFormatter, removeFormatter);
        }

        public static string FormatDiffTextAsHtml(
            this string a,
            string b,
            DiffEntry[] diffset,
            (ReadOnlyMemory<char>, ReadOnlyMemory<char>)? addFormatter = null,
            (ReadOnlyMemory<char>, ReadOnlyMemory<char>)? removeFormatter = null)
        {
            var addFormatting = addFormatter ?? Emphasize();
            var removeFormatting = removeFormatter ?? Delete();

            var text1Lines = a.Split('\n').Select(x => x.Trim('\r').AsMemory()).ToList();
            var text2Lines = b.Split('\n').Select(x => x.Trim('\r').AsMemory()).ToList();
            var resultLines = new StringBuilder();

            for (var x = 0; x < diffset.Length; x++)
            {
                var item = AddUntouchedLines(diffset, x, text1Lines, resultLines);
                AddDeletedLines(item, text1Lines, resultLines, removeFormatting);
                AddInsertedLines(item, text2Lines, addFormatting, resultLines);
            }

            return resultLines.ToString();
        }

        private const string Separator = "<br/>";

        private static void AddInsertedLines(
            DiffEntry diffEntry,
            List<ReadOnlyMemory<char>> text2Lines,
            (ReadOnlyMemory<char>, ReadOnlyMemory<char>) addFormatting,
            StringBuilder resultLines)
        {
            if (diffEntry.InsertedCompared <= 0)
            {
                return;
            }

            var range = text2Lines.GetRange(diffEntry.StartCompared, diffEntry.InsertedCompared);
            foreach (var memory in range)
            {
                resultLines.Append(addFormatting.Item1);
                resultLines.Append(memory);
                resultLines.Append(addFormatting.Item2);
            }
        }

        private static void AddDeletedLines(
            DiffEntry diffEntry,
            List<ReadOnlyMemory<char>> text1Lines,
            StringBuilder resultLines,
            (ReadOnlyMemory<char>, ReadOnlyMemory<char>) removeFormatting)
        {
            for (var i = 0; i < diffEntry.DeletedSource; i++)
            {
                var line = text1Lines[diffEntry.StartSource + i];
                resultLines.Append(removeFormatting.Item1);
                resultLines.Append(line);
                resultLines.Append(removeFormatting.Item2);
            }

            resultLines.Append(Separator.AsMemory());
        }

        private static DiffEntry AddUntouchedLines(
            DiffEntry[] diff,
            int x,
            List<ReadOnlyMemory<char>> text1Lines,
            StringBuilder resultLines)
        {
            var item = diff[x];
            var offset = x == 0 ? 0 : diff[x - 1].StartSource + diff[x - 1].DeletedSource;
            var count = item.StartSource - offset;
            var untouched = text1Lines.GetRange(offset, count);
            for (int i = 0; i < untouched.Count; i++)
            {
                resultLines.Append(untouched[i]);
                resultLines.Append(Separator.AsMemory());
            }

            return item;
        }

        public static string FormatDiffCharsAsHtml(this ReadOnlySpan<char> a, ReadOnlySpan<char> b)
        {
            var objArray = StringDiffExtensions.DiffChars(a, b);
            var stringBuilder = new StringBuilder();
            var index1 = 0;
            foreach (var obj in objArray)
            {
                for (; index1 < obj.StartCompared && index1 < b.Length; ++index1)
                {
                    stringBuilder.Append(b[index1]);
                }

                if (obj.DeletedSource > 0)
                {
                    stringBuilder.Append("<del>");
                    for (var index2 = 0; index2 < obj.DeletedSource; ++index2)
                    {
                        stringBuilder.Append(a[obj.StartSource + index2]);
                    }

                    stringBuilder.Append("</del>");
                }

                if (index1 < obj.StartCompared + obj.InsertedCompared)
                {
                    stringBuilder.Append("<em>");
                    for (; index1 < obj.StartCompared + obj.InsertedCompared; ++index1)
                    {
                        stringBuilder.Append(b[index1]);
                    }

                    stringBuilder.Append("</em>");
                }
            }

            for (; index1 < b.Length; ++index1)
            {
                stringBuilder.Append(b[index1]);
            }

            return stringBuilder.ToString();
        }

        private const string StartDel = "<del>";
        private const string EndDel = "</del>";
        private const string StartBold = "<b>";
        private const string EndBold = "</b>";

        private static (ReadOnlyMemory<char>, ReadOnlyMemory<char>) Delete()
        {
            return (StartDel.AsMemory(), EndDel.AsMemory());
        }

        private static (ReadOnlyMemory<char>, ReadOnlyMemory<char>) Emphasize()
        {
            return (StartBold.AsMemory(), EndBold.AsMemory());
        }
    }
}
