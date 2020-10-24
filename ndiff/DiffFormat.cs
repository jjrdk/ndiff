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
            Func<string, string>? addFormatter = null,
            Func<string, string>? removeFormatter = null,
            bool trimSpace = false,
            bool ignoreSpace = false,
            bool ignoreCase = false)
        {
            var diff = StringDiffExtensions.DiffText(a, b, trimSpace, ignoreSpace, ignoreCase);
            return FormatDiffTextAsHtml(a, b, diff, addFormatter, removeFormatter);
        }

        public static string FormatDiffTextAsHtml(
            this string a,
            string b,
            DiffEntry[] diffset,
            Func<string, string>? addFormatter = null,
            Func<string, string>? removeFormatter = null)
        {
            var addFormatting = addFormatter ?? Emphasize;
            var removeFormatting = removeFormatter ?? Delete;

            var text1Lines = a.Split('\n').Select(x => x.Trim('\r')).ToList();
            var text2Lines = b.Split('\n').Select(x => x.Trim('\r')).ToList();
            var resultLines = new List<string>();

            for (var x = 0; x < diffset.Length; x++)
            {
                var item = AddUntouchedLines(diffset, x, text1Lines, resultLines);
                AddDeletedLines(item, text1Lines, resultLines, removeFormatting);
                AddInsertedLines(item, text2Lines, addFormatting, resultLines);
            }

            return string.Join("<br/>", resultLines);
        }

        private static void AddInsertedLines(
            DiffEntry diffEntry,
            List<string> text2Lines,
            Func<string, string> addFormatting,
            List<string> resultLines)
        {
            if (diffEntry.InsertedCompared <= 0)
            {
                return;
            }

            var inserted = text2Lines.Skip(diffEntry.StartCompared).Take(diffEntry.InsertedCompared).Select(addFormatting);

            resultLines.AddRange(inserted);
        }

        private static void AddDeletedLines(
            DiffEntry diffEntry,
            List<string> text1Lines,
            List<string> resultLines,
            Func<string, string> removeFormatting)
        {
            var deleted = Enumerable.Range(0, diffEntry.DeletedSource)
                .Select(i => removeFormatting(text1Lines[i + diffEntry.StartSource]));
            resultLines.AddRange(deleted);
        }

        private static DiffEntry AddUntouchedLines(DiffEntry[] diff, int x, List<string> text1Lines, List<string> resultLines)
        {
            var item = diff[x];
            var offset = x == 0 ? 0 : (diff[x - 1].StartSource + diff[x - 1].DeletedSource);
            var count = item.StartSource - offset;
            var untouched = text1Lines.GetRange(offset, count);
            resultLines.AddRange(untouched);
            return item;
        }

        public static string FormatDiffCharsAsHtml(this string a, string b)
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

        private static string Delete(string txt)
        {
            return $"<del>{txt}</del>";
        }

        private static string Emphasize(string txt)
        {
            return $"<b>{txt}</b>";
        }
    }
}
