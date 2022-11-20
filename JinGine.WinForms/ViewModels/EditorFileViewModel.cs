namespace JinGine.WinForms.ViewModels;

public class EditorFileViewModel : BaseViewModel
{
    internal static readonly EditorFileViewModel Default = new(new [] { ArraySegment<char>.Empty });
    private int _columnNumber;
    private int _lineNumber;
    private int _offset;
    private IReadOnlyList<ArraySegment<char>> _textLines;

    internal int ColumnNumber
    {
        get => _columnNumber;
        set => SetField(ref _columnNumber, value);
    }

    internal int LineNumber
    {
        get => _lineNumber;
        set => SetField(ref _lineNumber, value);
    }

    internal int Offset
    {
        get => _offset;
        set => SetField(ref _offset, value);
    }

    internal IReadOnlyList<ArraySegment<char>> TextLines
    {
        get => _textLines;
        set => SetField(ref _textLines, value);
    }

    internal EditorFileViewModel(IReadOnlyList<ArraySegment<char>> textLines)
    {
        _textLines = textLines;
        _lineNumber = 1;
        _columnNumber = 1;
        _offset = 0;
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
