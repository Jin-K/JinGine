namespace JinGine.WinForms.Views;

internal interface IEditorView
{
    event EventHandler<char> KeyPressed;
    event EventHandler<Point> CaretPointChanged;

    void SetLines(string[] textLines);
    void SetCaretPosition(int line, int column, int offset);
}