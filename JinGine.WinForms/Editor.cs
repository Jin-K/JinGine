using JinGine.WinForms.Views;

namespace JinGine.WinForms;

/// <summary>
/// Represents our custom code-editor control.
/// </summary>
public partial class Editor : UserControl, IEditorView
{
    public int Line
    {
        get => Convert.ToInt32(_lineLabel.Text);
        private set => _lineLabel.Text = Convert.ToString(value);
    }

    public int Column
    {
        get => Convert.ToInt32(_columnLabel.Text);
        private set => _columnLabel.Text = Convert.ToString(value);
    }

    public int Offset
    {
        get => Convert.ToInt32(_offsetLabel.Text);
        private set => _offsetLabel.Text = Convert.ToString(value);
    }

    public event EventHandler<char> KeyPressed
    {
        add => _editorTextViewer.KeyPressed += value;
        remove => _editorTextViewer.KeyPressed -= value;
    }
    
    public Editor() => InitializeComponent();

    public void SetLines(string[] textLines) => _editorTextViewer.SetLines(textLines);

    public void SetCaretPosition(int line, int column, int offset)
    {
        (Line, Column, Offset) = (line, column, offset);
        _editorTextViewer.CaretPoint = new Point(column - 1, line - 1);
    }
}
