namespace JinGine.WinForms.Mappers;

internal static class TextLinesMapper
{
    internal static ArraySegment<char>[] Map(ArraySegment<char> charsSegment, int textLength)
    {
        var span = new ReadOnlySpan<char>(charsSegment.Array);
        var res = new List<ArraySegment<char>>();

        for (var pos = 0; pos <= textLength;)
        {
            var indexOfLineTerminator = span.IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                var segmentLength = indexOfLineTerminator;

                if (span[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < textLength &&
                    span[indexOfLineTerminator + 1] is '\n')
                    indexOfLineTerminator++;

                res.Add(charsSegment.Slice(pos, segmentLength));

                var deltaPos = indexOfLineTerminator + 1;
                pos += deltaPos;
                span = span[deltaPos..];
            }
            else
            {
                res.Add(charsSegment.Slice(pos, textLength - pos));
                break;
            }
        }

        return res.ToArray();
    }
}