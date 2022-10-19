using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinGine.Core.Models;

/// <summary>
/// Represents a list of line-segments of a text.
/// </summary>
public class EditorLinesList : IReadOnlyList<EditorLinesList.LineSegment>
{
    /// <summary>
    /// Represents a line-segment derived from a text.
    /// </summary>
    /// <param name="Content">The line-segment text content.</param>
    /// <param name="TextOffset">The offset in the original text where this line-segment begins.</param>
    /// <param name="Line">The line-number of this segment in the original text.</param>
    public record LineSegment(string Content, int TextOffset, int Line);
    
    private LineSegment[] _segments;

    internal EditorLinesList(string text) => _segments = CreateSegments(text);

    /// <summary>
    /// Renders the list again from a text.
    /// </summary>
    /// <remarks>
    /// This methods makes our class mutable, however, we can be provided as <see cref="IReadOnlyList{T}"/> contract.
    /// Check <see cref="List{T}"/> which does the same thing: it's mutable but provides an immutable version.
    /// </remarks>
    /// <param name="text">The text to use for rendering.</param>
    internal void Render(string text) => _segments = CreateSegments(text);

    /// <summary>
    /// Gets the overlapping or covering segment for a given text offset.
    /// </summary>
    /// <param name="textOffset">The offset/index/position in the original text.</param>
    /// <returns>The found <see cref="LineSegment"/> instance.</returns>
    /// <exception cref="InvalidOperationException">No overlapping <see cref="LineSegment"/> found.</exception>
    internal LineSegment GetOverlapping(int textOffset)
        => Array.FindLast(_segments, segment => segment.TextOffset <= textOffset)
           ?? throw new InvalidOperationException($"No overlapping {nameof(LineSegment)} found.");

    private static LineSegment[] CreateSegments(string text)
    {
        var list = new List<LineSegment>();
        var buffer = new StringBuilder();
        var number = 1;

        // TODO improve ; don't use a stringBuilder per line, instead, call substring correctly on the original string
        for (var pos = 0; pos < text.Length; pos++)
        {
            var c = text[pos];

            if (c is not '\r' and not '\n')
            {
                buffer.Append(c);
                if (pos != text.Length - 1) continue;
            }

            var segment = new LineSegment(buffer.ToString(), pos - buffer.Length, number++);
            list.Add(segment);
            buffer.Clear();

            if (c is not '\r') continue;
            if (pos >= text.Length - 1) continue;
            if (text[pos + 1] is not '\n') continue;

            pos++;
        }

        return list.ToArray();
    }

    //
    // Implementing IEnumerable<LineSegment>
    //

    public IEnumerator<LineSegment> GetEnumerator() => _segments.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.AsEnumerable().GetEnumerator();

    //
    // Implementing IReadOnlyCollection<LineSegment>
    //

    public int Count => _segments.Length;

    //
    // Implementing IReadOnlyList<LineSegment>
    //

    public LineSegment this[int index] => _segments[index];
}