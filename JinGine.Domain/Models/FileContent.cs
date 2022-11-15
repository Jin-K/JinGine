using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Models;

public class FileTextLineComparer : IComparer<StringSegment>, IEqualityComparer<StringSegment>
{
    public static readonly FileTextLineComparer Default = new();

    public int Compare(StringSegment x, StringSegment y) => x.Offset.CompareTo(y.Offset);
    public bool Equals(StringSegment x, StringSegment y) => x == y && x.Offset == y.Offset;
    public int GetHashCode(StringSegment obj) => (obj, obj.Offset).GetHashCode();
}

// value object
public class FileContent : IReadOnlyList<char>, IEquatable<FileContent>
{
    private static readonly char[] LineTerminators = { '\r', '\n' };

    public static readonly FileContent Empty = new(string.Empty);

    private readonly StringSegment[] _lines;

    public string TextContent { get; }

    public FileContent(string textContent)
    {
        TextContent = textContent;
        _lines = CreateLines(textContent);
    }

    public bool IsEmpty => _lines.Length is 1 && _lines[0].Length is 0;
    
    public IReadOnlyList<StringSegment> TextLines => _lines;

    public int Count => TextContent.Length;

    public char this[int index] => TextContent[index];

    private static StringSegment[] CreateLines(string text)
    {
        var res = new List<StringSegment>();
        StringSegment textAsSegment = text;

        var lineOffsetInText = 0;
        while (lineOffsetInText <= text.Length)
        {
            var lineTerminatorIndex = text.IndexOfAny(LineTerminators, lineOffsetInText);
            if (lineTerminatorIndex is not -1)
            {
                var segmentStart = lineOffsetInText;
                var segmentLength = lineTerminatorIndex - lineOffsetInText + 1;
                lineOffsetInText = lineTerminatorIndex + 1;
                if (text[lineTerminatorIndex] is '\r' &&
                    lineTerminatorIndex + 1 < text.Length &&
                    text[lineTerminatorIndex + 1] is '\n')
                {
                    lineOffsetInText++;
                    segmentLength++;
                }
                var segment = textAsSegment.Subsegment(segmentStart, segmentLength);
                res.Add(segment);
            }
            else
            {
                var segment = textAsSegment.Subsegment(lineOffsetInText, text.Length - lineOffsetInText);
                res.Add(segment);
                break;
            }
        }

        return res.ToArray();
    }

    public FileContent AddLine() => new(TextContent + Environment.NewLine);

    public FileContent InsertChar(char value, int lineIndex, int columnIndex)
    {
        var insertOffset = _lines[lineIndex].Offset + columnIndex;
        var length = TextContent.Length + 1;
        var stateValueTuple = (TextContent, value, insertOffset);
        var textContent = string.Create(length, stateValueTuple, (span, valueTuple) =>
        {
            var (content, @char, insertIndex) = valueTuple;
            content.AsSpan()[..insertIndex].CopyTo(span);
            span[insertIndex] = @char;
            content.AsSpan()[insertIndex..].CopyTo(span[(insertIndex + 1)..]);
        });

        return new FileContent(textContent);
    }

    public override bool Equals(object? obj) => Equals(obj as FileContent);

    public bool Equals(FileContent? other) =>
        other is not null && _lines.SequenceEqual(other._lines, new FileTextLineComparer());

    public IEnumerator<char> GetEnumerator() => TextContent.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode() => _lines.GetHashCode();

    public static bool operator ==(FileContent left, FileContent right) => left.Equals(right);

    public static bool operator !=(FileContent left, FileContent right) => !(left == right);

    public override string ToString() => TextContent;

    public static implicit operator string(FileContent content) => content.ToString();
}
