namespace NDiff;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class PatchWriterExtensions
{
    public static string ToPatchString(
        this string source,
        Delta<string>[] deltas,
        string? comment = null,
        int padding = 3)
    {
        return ToPatchString(source.Split(Environment.NewLine, StringSplitOptions.TrimEntries), deltas,
            comment, padding);
    }

    public static string ToPatchString<T>(
        this Memory<T> source,
        Delta<T>[] deltas,
        string? comment = null,
        int padding = 3,
        Func<T, string>? formatter = null)
    {
        formatter ??= x => x?.ToString() ?? string.Empty;
        StringBuilder result = new();
        if (comment != null)
        {
            result.AppendLine($"Subject: [PATCH] {comment}");
            result.AppendLine("---");
        }

        result.AppendLine("diff");
        foreach (var delta in deltas)
        {
            var start = Math.Max(0, delta.Diff.StartSource - padding);
            result.AppendLine(
                $"@@ -{delta.Diff.StartSource},{delta.Diff.DeletedSource} +{delta.Diff.StartCompared},{delta.Diff.InsertedCompared} @@");
            result.AppendLine($"    {string.Join(Environment.NewLine,
                Format(source.Slice(start, delta.Diff.StartSource - start), formatter))}");
            result.AppendLine($"+   {string.Join(Environment.NewLine, delta.Added.Select(formatter))}");
            var deletedRows = delta.Diff.StartSource + delta.Diff.DeletedSource;
            result.AppendLine($"-   {string.Join(Environment.NewLine,
                Format(source.Slice(delta.Diff.StartSource, deletedRows - delta.Diff.StartSource), formatter))}");
            var trailingStart = delta.Diff.StartSource + delta.Diff.DeletedSource;
            var trailingRows = Math.Min(padding, source.Length - deletedRows);
            if (trailingRows > 0)
            {
                result.AppendLine(string.Join(Environment.NewLine,
                    Format(source.Slice(trailingStart, trailingRows), formatter)));
            }
        }

        return result.ToString();

        static IEnumerable<string> Format(Memory<T> items, Func<T, string> f)
        {
            for (var i = 0; i < items.Length; i++)
            {
                yield return f(items.Span[i]);
            }
        }
    }
}
