using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JinGine.Core.Models;

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

    public LineSegment this[int index] => _lines[index];

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
                res.Add(new LineSegment(text[lineStart..index], lineStart));
                lineStart = index + 1;
                if (text[index] is '\r' && index + 1 < text.Length && text[index + 1] is '\n')
                    lineStart++;
            }
            else
            {
                res.Add(new LineSegment(text[lineStart..], lineStart));
                break;
            }
        }

        return res.ToArray();
    }

    public IEnumerator<LineSegment> GetEnumerator() => _lines.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public record LineSegment(string Content, int OffsetInText);
}