namespace JinGine.App.Mappers;

public static class EditorTextLinesMapper
{
    public static ArraySegment<char>[] Map(ArraySegment<char> text)
    {
        var length = text.Count;
        var textSpan = new ReadOnlySpan<char>(text.Array, text.Offset, text.Count);
        var res = new List<ArraySegment<char>>();
        var pos = 0;

        while (pos < length)
        {
            var indexOfNewLineChar = textSpan.IndexOfAny('\r', '\n');
            
            if ((uint)indexOfNewLineChar >= (uint)length) break;
            
            var deltaPos = indexOfNewLineChar + 1;

            if (textSpan[indexOfNewLineChar] is '\r')
            {
                var nextCharPos = indexOfNewLineChar + 1;
                if ((uint)nextCharPos < (uint)length && textSpan[nextCharPos] is '\n')
                {
                    deltaPos++;
                }
            }

            res.Add(text.Slice(pos, indexOfNewLineChar));

            pos += deltaPos;
            textSpan = textSpan[deltaPos..];
        }

        res.Add(text.Slice(pos, length - pos));

        return res.ToArray();
    }
}