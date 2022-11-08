using System.Collections;

// ReSharper disable once CheckNamespace
namespace System;

public static class StringExtensions
{
    /// <summary>
    /// Converts a text to a list of text-lines.
    /// </summary>
    /// <param name="this">The text that we split, contains \r\n or \n.</param>
    /// <param name="sort">The list will be sorted alphabetically if true.</param>
    /// <returns>An instance of <see cref="List{String}"/> with the isolated text-lines.</returns>
    internal static List<string> ToListOfLines(this string? @this, bool sort = false)
    {
        if (@this is null || @this.Length is 0) return new List<string>();
        var result = @this.ToListOfLines<List<string>>(1);
        if (sort) result.Sort();
        return result;
    }

    private static T ToListOfLines<T>(this string @this, int index) where T : IList, new()
    {
        var result = new T();
        var lineStart = 0;
        var length = @this.Length;

        while (index < length)
        {
            if (@this[index] is '\n')
            {
                var lineLength = index - lineStart;
                if (@this[index - 1] is '\r') lineLength--;

                result.Add(@this.Substring(lineStart, lineLength));
                lineStart = index + 1;
            }

            index++;
        }

        if (@this.Length > lineStart)
        {
            result.Add(@this[lineStart..]);
        }

        return result;
    }
}