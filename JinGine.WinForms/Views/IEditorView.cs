namespace JinGine.WinForms.Views;

internal interface IEditorView
{
    event EventHandler<char> KeyPressed;
    event EventHandler<Point> CaretLocationChanged;

    void SetLines(string[] textLines);
    void SetCaret(int line, int column, int offset);
    void SetProjector(GridProjector projector);
}