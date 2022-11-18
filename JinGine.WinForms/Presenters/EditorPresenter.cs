using System.Runtime.CompilerServices;
using System.Text;
using JinGine.Domain.Models;
using JinGine.WinForms.Views;

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
        var textLines = _editorFile.Content.TextLines;
        var textLinesCount = textLines.Count;
        var printableChars = new char[_editorFile.Content.Count]; // TODO get from ArrayPool<char>.Shared.Rent and make this IDisposable to call ArrayPool<char>.Shared.Return() at the end
        var printableTextLines = new ArraySegment<char>[textLinesCount];

        for (var i = 0; i < textLinesCount; i++)
        {
            var textLine = textLines[i];
            printableTextLines[i] = new ArraySegment<char>(printableChars, textLine.Offset, textLine.Count);
            CopyCharsOrReplaceUnprintable(textLine, printableTextLines[i]);
        }

        _view.SetLines(printableTextLines);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyCharsOrReplaceUnprintable(ReadOnlySpan<char> srcSpan, Span<char> destSpan)
    {
        srcSpan.CopyTo(destSpan);
        var length = srcSpan.Length;
        for (var i = 0; i < length; i++)
        {
            var ch = srcSpan[i];
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