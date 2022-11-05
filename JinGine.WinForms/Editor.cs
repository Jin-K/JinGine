using JinGine.WinForms.Views;

namespace JinGine.WinForms;

/// <summary>
/// Represents our custom code-editor control.
/// </summary>
public partial class Editor : UserControl, IEditorView
{
    private int Line { set => _lineLabel.Text = Convert.ToString(value); }
    private int Column { set => _columnLabel.Text = Convert.ToString(value); }
    private int Offset { set => _offsetLabel.Text = Convert.ToString(value); }

    public event EventHandler<Point> CaretLocationChanged
    {
        add => _editorTextViewer.CaretLocationChanged += value;
        remove => _editorTextViewer.CaretLocationChanged -= value;
    }

    public event EventHandler<char> KeyPressed
    {
        add => _editorTextViewer.KeyPressed += value;
        remove => _editorTextViewer.KeyPressed -= value;
    }

    public Editor() => InitializeComponent();

    public void SetLines(string[] textLines) => _editorTextViewer.SetLines(textLines);

    public void SetCaret(int line, int column, int offset)
    {
        (Line, Column, Offset) = (line, column, offset);
        _editorTextViewer.CaretLocation = new Point(column - 1, line - 1);
    }

    public void SetProjector(GridProjector projector)
    {
        _editorTextViewer.SetProjector(projector);
    }
}
