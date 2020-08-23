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
            if (diffEntry.InsertedB <= 0)
            {
                return;
            }

            var inserted = text2Lines.Skip(diffEntry.StartB).Take(diffEntry.InsertedB).Select(addFormatting);

            resultLines.AddRange(inserted);
        }

        private static void AddDeletedLines(
            DiffEntry diffEntry,
            List<string> text1Lines,
            List<string> resultLines,
            Func<string, string> removeFormatting)
        {
            var deleted = Enumerable.Range(0, diffEntry.DeletedA)
                .Select(i => removeFormatting(text1Lines[i + diffEntry.StartA]));
            resultLines.AddRange(deleted);
        }

        private static DiffEntry AddUntouchedLines(DiffEntry[] diff, int x, List<string> text1Lines, List<string> resultLines)
        {
            var item = diff[x];
            var offset = x == 0 ? 0 : (diff[x - 1].StartA + diff[x - 1].DeletedA);
            var count = item.StartA - offset;
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
                for (; index1 < obj.StartB && index1 < b.Length; ++index1)
                {
                    stringBuilder.Append(b[index1]);
                }

                if (obj.DeletedA > 0)
                {
                    stringBuilder.Append("<del>");
                    for (var index2 = 0; index2 < obj.DeletedA; ++index2)
                    {
                        stringBuilder.Append(a[obj.StartA + index2]);
                    }

                    stringBuilder.Append("</del>");
                }

                if (index1 < obj.StartB + obj.InsertedB)
                {
                    stringBuilder.Append("<em>");
                    for (; index1 < obj.StartB + obj.InsertedB; ++index1)
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
