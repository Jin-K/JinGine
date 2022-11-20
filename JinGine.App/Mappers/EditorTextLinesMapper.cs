namespace JinGine.App.Mappers;

public static class EditorTextLinesMapper
{
    public static ArraySegment<char>[] Map(ArraySegment<char> charsSegment)
    {
        var length = charsSegment.Count;
        ReadOnlySpan<char> charsSpan = charsSegment;
        var res = new List<ArraySegment<char>>();

        for (var pos = 0; pos <= length;)
        {
            var indexOfLineTerminator = charsSpan.IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1)
            {
                var segmentLength = indexOfLineTerminator;

                if (charsSpan[indexOfLineTerminator] is '\r' &&
                    indexOfLineTerminator + 1 < length &&
                    charsSpan[indexOfLineTerminator + 1] is '\n')
                    indexOfLineTerminator++;

                res.Add(charsSegment.Slice(pos, segmentLength));

                var deltaPos = indexOfLineTerminator + 1;
                pos += deltaPos;
                charsSpan = charsSpan[deltaPos..];
            }
            else
            {
                res.Add(charsSegment.Slice(pos, length - pos));
                break;
            }
        }

        return res.ToArray();
    }
}