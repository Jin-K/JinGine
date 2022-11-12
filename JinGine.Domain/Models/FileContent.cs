using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Models;

// value object
public readonly struct FileContent : IReadOnlyCollection<TextLine>, IEquatable<FileContent>
{
    private static readonly char[] LineTerminators = { '\r', '\n' };

    public static readonly FileContent Empty = CreateFromRawContent(string.Empty);

    private readonly TextLine[] _lines;

    public FileContent(TextLine[] lines)
    {
        if (lines.Length is 0)
            throw new ArgumentException("Should have at least 1 line.", nameof(lines));

        _lines = lines;
    }

    public bool IsEmpty => _lines.Length is 1 && _lines[0].Length is 0;

    public static FileContent CreateFromRawContent(string rawContent) => new(CreateLines(rawContent));

    private static TextLine[] CreateLines(StringSegment text)
    {
        var res = new List<TextLine>();

        var lineOffset = 0;
        while (lineOffset <= text.Length)
        {
            var lineTerminatorIndex = text.IndexOfAny(LineTerminators, lineOffset);
            if (lineTerminatorIndex is not -1)
            {
                res.Add(text.Subsegment(lineOffset, lineTerminatorIndex - lineOffset));
                lineOffset = lineTerminatorIndex + 1;
                if (text[lineTerminatorIndex] is '\r' &&
                    lineTerminatorIndex + 1 < text.Length &&
                    text[lineTerminatorIndex + 1] is '\n')
                {
                    lineOffset++;
                }
            }
            else
            {
                res.Add(text.Subsegment(lineOffset, text.Length - lineOffset));
                break;
            }
        }

        return res.ToArray();
    }

    public FileContent AddLine()
    {
        var newLines = new TextLine[Count + 1];
        _lines.CopyTo(newLines, 0);
        newLines[Count] = new TextLine(string.Empty);
        return new FileContent(newLines);
    }

    public int Count => _lines.Length;

    public IEnumerator<TextLine> GetEnumerator() => _lines.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override bool Equals(object? obj) => obj is FileContent other && Equals(other);

    public bool Equals(FileContent other) => _lines.SequenceEqual(other);

    public override int GetHashCode() => _lines.GetHashCode();

    public static bool operator ==(FileContent left, FileContent right) => left.Equals(right);

    public static bool operator !=(FileContent left, FileContent right) => !(left == right);
}

