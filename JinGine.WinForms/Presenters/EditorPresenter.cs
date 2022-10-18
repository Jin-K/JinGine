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
        
        var lineContentsByNumber = model.EditorText.Lines.ToDictionary(
            line => line.LineNumber,
            line => line.Content);
        view.Render(lineContentsByNumber);

        // TODO NOOOOOOOOOOO !!!, let the model do it by itself, ask the view to go to resulting coordinates
        model.EditorText.Navigate(EditorText.NavigationDestination.End);

        // TODO v-scroll if not visible line !!
        // TODO h-scroll if not visible column !!

        view.PressedKey += OnPressedKey;
    }

    private void OnPressedKey(object? sender, char e)
    {
        _model.EditorText.Write(e);
        var lines = _model.EditorText.Lines.ToDictionary(l => l.LineNumber, l => l.Content);
        _view.Render(lines);
    }
}