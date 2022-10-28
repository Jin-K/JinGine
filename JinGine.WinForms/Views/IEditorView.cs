namespace JinGine.WinForms.Views;

internal interface IEditorView
{
    int Line { get; }
    int Column { get; }
    int Offset { get; }

    event EventHandler<char> KeyPressed;

    void SetLines(string[] textLines);
    void SetCaretPosition(int line, int column, int offset);
}