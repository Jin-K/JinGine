using System;
using System.Text;

namespace JinGine.Core.Models;

/// <summary>
/// Represents a mutable &amp; navigable text to use within an editor.
/// </summary>
public class EditorText
{
    public enum NavigationDestination
    {
        Begin,
        Current,
        End,
    }
    
    private readonly StringBuilder _textBuilder;
    private EditorCaret _caret;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorText"/> class.
    /// </summary>
    public EditorText() : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorText"/> class.
    /// </summary>
    /// <param name="content">The initial text content.</param>
    public EditorText(string content)
    {
        _textBuilder = new StringBuilder(content);
        Lines = new EditorLinesList(content);
        _caret = EditorCaret.Origin;
    }

    /// <summary>
    /// Gets the enumerable <see cref="EditorLinesList.LineSegment"/> projection of the text in the editor.
    /// </summary>
    public EditorLinesList Lines { get; }

    /// <summary>
    /// Gets the caret 0-based text offset.
    /// </summary>
    public int Position => _caret.TextOffset;

    /// <summary>
    /// Gets the number of the line where the caret is positioned.
    /// </summary>
    public int LineNumber => _caret.Line + 1;

    /// <summary>
    /// Gets the number of the column where the caret is positioned.
    /// </summary>
    public int ColumnNumber => _caret.Column + 1;

    /// <summary>
    /// Gets the char at current position, or <see langword="null"/> if positioned at the end.
    /// </summary>
    public char? CurrentChar => Position == _textBuilder.Length ? null : _textBuilder[Position];

    public void Navigate(NavigationDestination destination, int offset = 0)
    {
        var length = _textBuilder.Length;

        offset = destination switch
        {
            NavigationDestination.Begin => offset >= 0
                ? offset
                : throw new ArgumentOutOfRangeException(nameof(offset), "Has to be a positive value."),
            NavigationDestination.Current => Position + offset,
            NavigationDestination.End => length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(destination)),
        };

        if (offset < 0)
            throw new InvalidOperationException("Cannot position caret before content.");
        if (offset > length)
            throw new InvalidOperationException("Cannot position caret after content.");

        SetCaret(offset);
    }

    private void SetCaret(int offset)
    {
        var segment = Lines.GetOverlapping(offset);
        var columnNumber = offset - segment.TextOffset + 1;
        _caret = new EditorCaret(offset, columnNumber, segment.LineNumber);
    }

    /// <summary>
    /// Converts the value of this instance to a <see cref="string"/>.
    /// </summary>
    /// <returns>The value of the text in the editor.</returns>
    public override string ToString() => _textBuilder.ToString();

    /// <summary>
    /// Writes a <see langword="char"/> at caret position;
    /// </summary>
    /// <param name="value">The char to write.</param>
    public void Write(char value) => WriteAt(value, Position);

    private void WriteAt(char value, int offset)
    {
        switch (value)
        {
            case (char)ConsoleKey.Enter or '\n':
                WriteEndOfLine(ref offset);
                break;
            case (char)ConsoleKey.Backspace:
                _textBuilder.Remove(--offset, 1);
                break;
            default:
                _textBuilder.Insert(offset++, value);
                break;
        }
        
        Lines.Render(_textBuilder.ToString()); // TODO improve, could be a performance issue ?

        SetCaret(offset);
    }

    private void WriteEndOfLine(ref int offset)
    {
        _textBuilder.Insert(offset, Environment.NewLine);
        offset += Environment.NewLine.Length;
    }
}