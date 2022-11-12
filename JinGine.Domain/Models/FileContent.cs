using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace JinGine.Domain.Models;

public record FileContent(string Content)
{
    private static readonly char[] LineTerminators = { '\r', '\n' };

    public static readonly FileContent Empty = new(string.Empty);

    public TextLines TextLines { get; } = CreateLines(Content);

    private static TextLines CreateLines(StringSegment text)
    {
        var res = new List<StringSegment>();

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

        return new TextLines(res);
    }
}

