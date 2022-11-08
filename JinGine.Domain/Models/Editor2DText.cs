using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JinGine.Domain.Models;

public class Editor2DText : IReadOnlyList<Editor2DText.LineSegment>
{
    private static readonly char[] LineTerminators = { '\r', '\n' };
    private string _content;
    private LineSegment[] _lines;

    public string Content
    {
        get => _content;
        internal set
        {
            if (_content == value) return;
            _content = value;
            _lines = RenderLines(value);
        }
    }

    public int Count => _lines.Length;

    public LineSegment this[int index]
    {
        get
        {
            if (index < 0 || index >= _lines.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "That line does not exist.");

            return _lines[index];
        }
    }

    public Editor2DText(string content)
    {
        _content = content;
        _lines = RenderLines(content);
    }

    private static LineSegment[] RenderLines(string text)
    {
        if (text.Length is 0) return Array.Empty<LineSegment>();

        var res = new List<LineSegment>();

        for (var lineStart = 0; lineStart <= text.Length;)
        {
            var index = text.IndexOfAny(LineTerminators, lineStart);
            if (index >= 0)
            {
                var content = ToPrintable(text, lineStart, index - lineStart);
                res.Add(new LineSegment(content, lineStart));
                lineStart = index + 1;
                if (text[index] is '\r' && index + 1 < text.Length && text[index + 1] is '\n')
                    lineStart++;
            }
            else
            {
                var content = ToPrintable(text, lineStart, text.Length - lineStart);
                res.Add(new LineSegment(content, lineStart));
                break;
            }
        }

        return res.ToArray();
    }

    public IEnumerator<LineSegment> GetEnumerator() => _lines.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static string ToPrintable(ReadOnlySpan<char> source, int startIndex, int length)
    {
        var chars = length > 512 ? new char[length] : stackalloc char[512];
        chars = chars[..length];

        for (var i = 0; i < length; i++)
        {
            var ch = source[i + startIndex];
            chars[i] = ch switch
            {
                (char)0 => ',',
                '\t' => '·',
                < ' ' => '…',
                < (char)0x7f => ch,
                < (char)0xa1 => '¡',
                _ => ch,
            };
        }

        return new string(chars);
    }

    public record LineSegment(string Content, int OffsetInText);
}