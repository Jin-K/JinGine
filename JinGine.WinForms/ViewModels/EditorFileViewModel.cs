namespace JinGine.WinForms.ViewModels;

public class EditorFileViewModel
{
    internal static readonly EditorFileViewModel Default = new(new [] { ArraySegment<char>.Empty });

    internal IReadOnlyList<ArraySegment<char>> TextLines { get; set; }

    internal int LineNumber { get; set; }

    internal int ColumnNumber { get; set; }

    internal int Offset { get; set; }

    internal EditorFileViewModel(IReadOnlyList<ArraySegment<char>> textLines)
    {
        TextLines = textLines;
        LineNumber = 1;
        ColumnNumber = 1;
        Offset = 0;
    }

    internal void UpdateCaretPositions(int offset)
    {
        var lineIndex = TextLines
            .TakeWhile(tl => tl.Offset <= offset)
            .Skip(1)
            .Count();
        
        LineNumber = lineIndex + 1;
        ColumnNumber = offset - TextLines[lineIndex].Offset + 1;
        Offset = offset;
    }
}