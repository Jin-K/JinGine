using System.ComponentModel;
using JinGine.WinForms.Views;
using JinGine.WinForms.Views.Models;

namespace JinGine.WinForms.Controls;

/// <summary>
/// Represents our custom code-editor control.
/// </summary>
public partial class Editor : UserControl, IEditorView
{
    private int Line { set => _lineLabel.Text = Convert.ToString(value); }
    private int Column { set => _columnLabel.Text = Convert.ToString(value); }
    private int Offset { set => _offsetLabel.Text = Convert.ToString(value); }

    [Bindable(true)]
    public TextSelectionRange TextSelection
    {
        get => _editorTextViewer.TextSelection;
        set => _editorTextViewer.TextSelection = value;
    }

    public event EventHandler<Point> CaretPointChanged
    {
        add => _editorTextViewer.CaretPointChanged += value;
        remove => _editorTextViewer.CaretPointChanged -= value;
    }

    public event EventHandler<char> KeyPressed
    {
        add => _editorTextViewer.KeyPressed += value;
        remove => _editorTextViewer.KeyPressed -= value;
    }

    public Editor()
    {
        InitializeComponent();
        base.Dock = DockStyle.Fill;
    }

    public void SetCaret(int line, int column, int offset)
    {
        (Line, Column, Offset) = (line, column, offset);
        _editorTextViewer.CaretPoint = new Point(column - 1, line - 1);
    }

    public void SetLines(IReadOnlyList<ArraySegment<char>> textLines) => _editorTextViewer.SetLines(textLines);
}
