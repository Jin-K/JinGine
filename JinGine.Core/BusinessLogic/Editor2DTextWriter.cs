using System;
using System.Text;
using JinGine.Core.Models;

namespace JinGine.Core.BusinessLogic;

public class Editor2DTextWriter : IEditor2DTextWriter
{
    private readonly StringBuilder _textBuilder;
    private readonly Editor2DText _model;

    public int PositionInText { get; private set; }

    public Editor2DTextWriter(Editor2DText model)
    {
        _model = model;
        _textBuilder = new StringBuilder(model.Content);
        PositionInText = model.Content.Length;
    }

    public override string ToString() => _textBuilder.ToString();

    public void GoTo(int positionInText)
    {
        PositionInText = positionInText;
    }

    public void Write(char value)
    {
        switch (value)
        {
            case (char)ConsoleKey.Enter or '\n':
                _textBuilder.Insert(PositionInText, Environment.NewLine);
                PositionInText += Environment.NewLine.Length;
                break;
            case (char)ConsoleKey.Backspace:
                // TODO there is a bug here when we try to remove last char from the text, _model.Content ends empty
                PositionInText--;
                var removeLength = 1;
                if (_textBuilder[PositionInText] is '\n' && PositionInText >= 1 && _textBuilder[PositionInText - 1] is '\r')
                {
                    PositionInText--;
                    removeLength++;
                }
                _textBuilder.Remove(PositionInText, removeLength);
                break;
            default:
                _textBuilder.Insert(PositionInText, value);
                PositionInText++;
                break;
        }
        
        _model.Content = _textBuilder.ToString();
    }

    public void Write(string value)
    {
        _textBuilder.Insert(PositionInText, value);
        _model.Content = _textBuilder.ToString();
        PositionInText += value.Length;
    }

    public void WriteLine() => Write((char)ConsoleKey.Enter);
}