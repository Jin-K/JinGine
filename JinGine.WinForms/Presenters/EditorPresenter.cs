using JinGine.Core.Models;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

internal class EditorPresenter
{
    private readonly IEditorView _view;
    private readonly EditorModel _model;

    internal EditorPresenter(IEditorView view, EditorModel model)
    {
        _view = view;
        _model = model;
        
        view.Render(model.EditorText);
        view.ScrollTo(model.EditorText.Line, model.EditorText.Column);

        view.PressedKey += OnPressedKey;
    }

    private void OnPressedKey(object? sender, char e)
    {
        _model.EditorText.Write(e);
        _view.Render(_model.EditorText);
    }
}