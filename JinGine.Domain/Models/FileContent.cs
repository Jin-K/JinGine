using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JinGine.Domain.Models;

// value object
public class FileContent : IReadOnlyList<char>, IEquatable<FileContent>
{
    public static readonly FileContent Empty = new(string.Empty);
    
    private readonly ArraySegment<char>[] _lines;

    public string Text { get; }

    public FileContent(string text)
    {
        Text = text;
        _lines = CreateLineSegments(text.ToCharArray());
    }

    public bool IsEmpty => Text.Equals(string.Empty);

    public IReadOnlyList<ArraySegment<char>> Lines => Array.AsReadOnly(_lines);

    public int Count => Text.Length;

    public char this[int index] => Text[index];

    private static ArraySegment<char>[] CreateLineSegments(char[] textChars)
    {
        var res = new List<ArraySegment<char>>();
        var textCharsAsSegment = new ArraySegment<char>(textChars);
        var textCharsAsSpan = new ReadOnlySpan<char>(textChars);
        var length = textChars.Length;

        for (var pos = 0; pos <= length;)
        {
            var indexOfLineTerminator = textCharsAsSpan.IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                if (textCharsAsSpan[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < textCharsAsSpan.Length &&
                    textCharsAsSpan[indexOfLineTerminator + 1] is '\n')
                    indexOfLineTerminator++;

                var segmentLength = indexOfLineTerminator + 1;
                var segment = textCharsAsSegment.Slice(pos, segmentLength);
                res.Add(segment);
                pos += segmentLength;
                textCharsAsSpan = textCharsAsSpan.Slice(segmentLength);
            }
            else
            {
                var segment = textCharsAsSegment.Slice(pos);
                res.Add(segment);
                break;
            }
        }

        return res.ToArray();
    }

    public FileContent AddLine() => new(Text + Environment.NewLine);

    public FileContent InsertChar(char value, int lineIndex, int columnIndex)
    {
        var insertOffset = _lines[lineIndex].Offset + columnIndex;
        var length = Text.Length + 1;
        var stateValueTuple = (Text, value, insertOffset);
        var text = string.Create(length, stateValueTuple, (span, valueTuple) =>
        {
            var (content, @char, insertIndex) = valueTuple;
            content.AsSpan()[..insertIndex].CopyTo(span);
            span[insertIndex] = @char;
            content.AsSpan()[insertIndex..].CopyTo(span[(insertIndex + 1)..]);
        });

        return new FileContent(text);
    }

    public override bool Equals(object? obj) => Equals(obj as FileContent);

    public bool Equals(FileContent? other) => other is not null && _lines.SequenceEqual(other._lines);

    public IEnumerator<char> GetEnumerator() => Text.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode() => _lines.GetHashCode();

    public static bool operator ==(FileContent left, FileContent right) => left.Equals(right);

    public static bool operator !=(FileContent left, FileContent right) => !(left == right);

    public override string ToString() => Text;

    public static implicit operator string(FileContent content) => content.ToString();
}
