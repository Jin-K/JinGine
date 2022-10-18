namespace JinGine.WinForms.Views;

internal interface IEditorView
{
    event EventHandler<char> PressedKey;

    void Render(IReadOnlyDictionary<int, string> textLinesByNumber);
    void ScrollTo(int line, int column);
}