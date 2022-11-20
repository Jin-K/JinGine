using System.Buffers;
using System.Runtime.CompilerServices;
using JinGine.Domain.Models;
using JinGine.WinForms.Mappers;
using JinGine.WinForms.ViewModels;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

internal class EditorPresenter : IDisposable
{
    private readonly IEditorView _view;
    private readonly EditorFileViewModel _viewModel;
    private int _charsLength;
    private int _pos;
    private char[] _charsToReturn;

    internal EditorPresenter(IEditorView view, EditorFile editorFile)
    {
        var length = editorFile.Content.Length;
        var textChars = ArrayPool<char>.Shared.Rent(length);
        editorFile.Content.CopyTo(textChars.AsSpan());
        var textLines = TextLinesMapper.Map(textChars, length);

        _view = view;
        _viewModel = new EditorFileViewModel(textLines);
        _charsLength = length;
        _pos = editorFile.Content.Length;
        _charsToReturn = textChars;

        view.KeyPressed += OnKeyPressed;
        view.CaretPointChanged += OnCaretPointChanged;

        _view.SetViewModel(_viewModel);
        _viewModel.UpdateCaretPositions(_pos);
        SetCaretPositionsInView();
        ReplaceUnprintableChars(textChars.AsSpan(), length);
    }

    private void AutoRentChars()
    {
        if (_charsLength <= _charsToReturn.Length) return;

        var oldCharsToReturn = _charsToReturn;
        _charsToReturn = ArrayPool<char>.Shared.Rent(_charsLength);
        oldCharsToReturn.CopyTo(_charsToReturn.AsSpan());
        ArrayPool<char>.Shared.Return(oldCharsToReturn);
    }

    private void HandleCharKey(char value)
    {
        var oldCharsLength = _charsLength;
        switch (value)
        {
            case '\r' or '\n':
            {
                var newLine = Environment.NewLine.AsSpan();
                
                _charsLength = oldCharsLength + newLine.Length;
                AutoRentChars();

                _charsToReturn.AsSpan(_pos, oldCharsLength - _pos)
                    .CopyTo(_charsToReturn.AsSpan(_pos + newLine.Length));
                newLine.CopyTo(_charsToReturn.AsSpan(_pos));
                _charsToReturn.AsSpan(0, _pos)
                    .CopyTo(_charsToReturn.AsSpan());
                _pos += newLine.Length;

                break;
            }
            case (char)ConsoleKey.Backspace:
            {
                if (_pos is 0) return;

                var retreat = 1;
                if (_pos >= 2 && _charsToReturn[_pos - 1] is '\n' && _charsToReturn[_pos - 2] is '\r')
                    retreat++;
                
                _charsLength = oldCharsLength - retreat;
                AutoRentChars();

                _charsToReturn.AsSpan(_pos, oldCharsLength - _pos)
                    .CopyTo(_charsToReturn.AsSpan(_pos - retreat));
                _charsToReturn.AsSpan(0, _pos - retreat)
                    .CopyTo(_charsToReturn.AsSpan());
                _pos -= retreat;

                break;
            }
            default:
            {
                _charsLength++;
                AutoRentChars();

                _charsToReturn.AsSpan(_pos, oldCharsLength - _pos)
                    .CopyTo(_charsToReturn.AsSpan(_pos + 1));
                _charsToReturn[_pos] = value;
                _charsToReturn.AsSpan(0, _pos)
                    .CopyTo(_charsToReturn.AsSpan());
                _pos++;

                break;
            }
        }
        
        _viewModel.TextLines = TextLinesMapper.Map(_charsToReturn, _charsLength);
    }

    private void OnCaretPointChanged(object? sender, Point e)
    {
        var offsetInText = _viewModel.TextLines[e.Y].Offset;
        _pos = offsetInText + e.X;
        _view.SetCaret(e.Y + 1, e.X + 1, _pos);
    }

    private void OnKeyPressed(object? sender, char e)
    {
        HandleCharKey(e);
        _viewModel.UpdateCaretPositions(_pos);
        SetCaretPositionsInView();
    }

    private void SetCaretPositionsInView()
    {
        // TODO missing two-way-data-binding to let the view detect changes and avoid this method
        _view.SetCaret(_viewModel.LineNumber, _viewModel.ColumnNumber, _viewModel.Offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReplaceUnprintableChars(Span<char> charsSpan, int length)
    {
        for (var i = 0; i < length; i++)
        {
            switch (charsSpan[i])
            {
                case (char)0: charsSpan[i] = ','; continue;
                case '\t': charsSpan[i] = '·'; continue;
                case '\r' or '\n': continue;
                case < ' ': charsSpan[i] = '…'; continue;
                case < (char)0x7f: continue;
                case < (char)0xa1: charsSpan[i] = '¡'; continue;
            }
        }
    }

    // TODO dispose when needed
    public void Dispose() => ArrayPool<char>.Shared.Return(_charsToReturn);
}