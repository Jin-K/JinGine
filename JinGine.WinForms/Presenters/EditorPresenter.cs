using System.Text;
using JinGine.Domain.Models;
using JinGine.WinForms.Views;
using Microsoft.Extensions.Primitives;

namespace JinGine.WinForms.Presenters;

internal class EditorPresenter
{
    private readonly IEditorView _view;
    private readonly EditorFile _editorFile;
    private readonly StringBuilder _sb;
    private int _pos;

    internal EditorPresenter(IEditorView view, EditorFile editorFile)
    {
        _view = view;
        _editorFile = editorFile;
        _sb = new StringBuilder(editorFile.Content);
        _pos = editorFile.Content.Count;

        view.KeyPressed += OnKeyPressed;
        view.CaretPointChanged += OnCaretPointChanged;

        SetCaretPositionInView();
        SetLinesInView();
    }

    private void HandleCharKey(char value)
    {
        // TODO use commands to update the domain model
        switch (value)
        {
            case (char)ConsoleKey.Enter or '\n':
                _sb.Insert(_pos, Environment.NewLine);
                _pos += Environment.NewLine.Length;
                break;
            case (char)ConsoleKey.Backspace:
                // TODO there is a bug here when we try to remove last char from the text, _model.Content ends empty
                _pos--;
                var removeLength = 1;
                if (_sb[_pos] is '\n' && _pos >= 1 && _sb[_pos - 1] is '\r')
                {
                    _pos--;
                    removeLength++;
                }
                _sb.Remove(_pos, removeLength);
                break;
            default:
                _sb.Insert(_pos, value);
                _pos++;
                break;
        }

        _editorFile.ResetTextContent(_sb.ToString());
    }

    private void OnCaretPointChanged(object? sender, Point e)
    {
        var offsetInText = _editorFile.Content.TextLines[e.Y].Offset;
        _pos = offsetInText + e.X;
        _view.SetCaret(e.Y + 1, e.X + 1, _pos);
    }

    private void OnKeyPressed(object? sender, char e)
    {
        HandleCharKey(e);
        SetCaretPositionInView();
        SetLinesInView();
    }

    private void SetCaretPositionInView()
    {
        var textLines = _editorFile.Content.TextLines;
        var lineIndex = textLines.TakeWhile(tl => tl.Offset <= _pos).Skip(1).Count();
        var lineOffset = textLines[lineIndex].Offset;
        _view.SetCaret(lineIndex + 1, _pos - lineOffset + 1, _pos);
    }

    private void SetLinesInView()
    {
        var fileContent = _editorFile.Content;
        // recreating a second potential big string :'(
        // we could just reuse the original string AND text-lines if it was already formatted and that could
        // maybe be done in the EditorFile entity but then we would lose the original chars coming from the file.
        var printableTextContent = string.Create(fileContent.Count, fileContent.TextContent, ConvertToPrintableChars);

        var textLines = fileContent.TextLines;
        var textLinesCount = textLines.Count;
        var printableTextLines = new StringSegment[textLinesCount]; // a new array on the heap :'(
        for (var i = 0; i < textLinesCount; i++)
        {
            var oldSegment = textLines[i];
            var newSegmentLength = oldSegment.Length;
            var indexOfLineTerminator = oldSegment.AsSpan().IndexOfAny('\r', '\n');
            if (indexOfLineTerminator is not -1) newSegmentLength = indexOfLineTerminator;
            printableTextLines[i] = new StringSegment(printableTextContent, oldSegment.Offset, newSegmentLength);
        }
        
        _view.SetLines(printableTextLines);
    }

    private static void ConvertToPrintableChars(Span<char> destSpan, string src)
    {
        src.CopyTo(destSpan);
        var length = src.Length;
        for (var i = 0; i < length; i++)
        {
            var ch = src[i];
            switch (ch)
            {
                case (char)0: destSpan[i] = ','; continue;
                case '\t': destSpan[i] = '·'; continue;
                case < ' ': destSpan[i] = '…'; continue;
                case < (char)0x7f: continue;
                case < (char)0xa1: destSpan[i] = '¡'; continue;
            }
        }
    }
}