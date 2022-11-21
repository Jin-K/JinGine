using JinGine.WinForms.ViewModels;
using JinGine.WinForms.Views.Models;

namespace JinGine.WinForms.Views;

internal interface IEditorView
{
    TextSelectionRange TextSelection { get; }

    event EventHandler<char> KeyPressed;
    event EventHandler<Point> CaretPointChanged;

    void SetViewModel(EditorFileViewModel viewModel);
}