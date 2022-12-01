using System.Buffers;
using System.Runtime.CompilerServices;
using JinGine.App.Mappers;
using JinGine.Domain.Models;
using JinGine.WinForms.ViewModels;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

internal class EditorPresenter : IDisposable
{
    private readonly EditorFileViewModel _viewModel;
    private int _charsLength;
    private int _pos;
    private char[] _rentedChars;

    internal EditorPresenter(IEditorView view, EditorFile editorFile)
    {
        var length = editorFile.Content.Length;
        var textChars = ArrayPool<char>.Shared.Rent(length);
        editorFile.Content.CopyTo(textChars.AsSpan());
        var charsSegment = new ArraySegment<char>(textChars, 0, length);
        var textLines = EditorTextLinesMapper.Map(charsSegment);
        
        _viewModel = new EditorFileViewModel(textLines);
        _charsLength = length;
        _pos = editorFile.Content.Length;
        _rentedChars = textChars;

        view.KeyPressed += OnKeyPressed;
        view.CaretPointChanged += OnCaretPointChanged;

        view.SetViewModel(_viewModel);
        _viewModel.UpdateCaretPositions(_pos);
        ReplaceUnprintableChars(textChars.AsSpan(0, length));
    }

    public void Dispose() => ArrayPool<char>.Shared.Return(_rentedChars);

    private void EnsureRentedCharsSize()
    {
        if (_charsLength <= _rentedChars.Length) return;

        var charsToReturn = _rentedChars;
        _rentedChars = ArrayPool<char>.Shared.Rent(_charsLength);
        charsToReturn.CopyTo(_rentedChars.AsSpan());
        ArrayPool<char>.Shared.Return(charsToReturn);
    }

    private void HandleCharKey(char value)
    {
        var oldCharsLength = _charsLength;
        switch (value)
        {
            case '\r' or '\n':
                var newLine = Environment.NewLine.AsSpan();

                // determine chars length
                _charsLength = oldCharsLength + newLine.Length;
                EnsureRentedCharsSize();

                // copy ending chars
                _rentedChars.AsSpan(_pos, oldCharsLength - _pos)
                    .CopyTo(_rentedChars.AsSpan(_pos + newLine.Length));
                
                // set new line chars at position
                newLine.CopyTo(_rentedChars.AsSpan(_pos));

                // copy leading chars
                _rentedChars.AsSpan(0, _pos).CopyTo(_rentedChars.AsSpan());
                
                // set new position
                _pos += newLine.Length;

                break;
            case (char)ConsoleKey.Backspace:
                if (_pos is 0) return;

                // determine retreat count
                var retreat = 1;
                if (_pos >= 2 && _rentedChars[_pos - 1] is '\n' && _rentedChars[_pos - 2] is '\r')
                    retreat++;

                // determine chars length
                _charsLength = oldCharsLength - retreat;
                EnsureRentedCharsSize();

                // copy ending chars
                _rentedChars.AsSpan(_pos, oldCharsLength - _pos)
                    .CopyTo(_rentedChars.AsSpan(_pos - retreat));
                
                // copy leading chars
                _rentedChars.AsSpan(0, _pos - retreat).CopyTo(_rentedChars.AsSpan());
                
                // set new position
                _pos -= retreat;

                break;
            default:
                // determine chars length
                _charsLength++;
                EnsureRentedCharsSize();

                // copy ending chars
                _rentedChars.AsSpan(_pos, oldCharsLength - _pos)
                    .CopyTo(_rentedChars.AsSpan(_pos + 1));
                
                // set new char value at position
                _rentedChars[_pos] = value;
                
                // copy leading chars
                _rentedChars.AsSpan(0, _pos).CopyTo(_rentedChars.AsSpan());
                
                // set new position
                _pos++;

                break;
        }

        var charsSegment = new ArraySegment<char>(_rentedChars, 0, _charsLength);
        _viewModel.TextLines = EditorTextLinesMapper.Map(charsSegment);
    }

    private void OnCaretPointChanged(object? sender, Point e)
    {
        var offsetInText = _viewModel.TextLines[e.Y].Offset;
        _pos = offsetInText + e.X;
        _viewModel.ColumnNumber = e.X + 1;
        _viewModel.LineNumber = e.Y + 1;
        _viewModel.Offset = _pos;
    }

    private void OnKeyPressed(object? sender, char e)
    {
        HandleCharKey(e);
        _viewModel.UpdateCaretPositions(_pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReplaceUnprintableChars(Span<char> charsSpan)
    {
        var length = charsSpan.Length;
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
}