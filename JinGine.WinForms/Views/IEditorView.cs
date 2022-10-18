using JinGine.Core.Models;

namespace JinGine.WinForms.Views;

internal interface IEditorView
{
    event EventHandler<char> PressedKey;

    void Render(IDictionary<int, string> textLinesByNumber);
}