using JinGine.Core.BusinessLogic;
using JinGine.Core.Models;
using JinGine.WinForms.Views;
using LegacyFwk;

namespace JinGine.WinForms.Presenters;

internal class EditorPresenter
{
    private readonly IEditorView _view;
    private readonly Editor2DText _model;
    private readonly IEditor2DTextWriter _writer;
    private readonly IEditor2DTextReader _reader;

    internal EditorPresenter(IEditorView view, string fileName)
    {
        _view = view;
        _model = new Editor2DText(FileManager.GetTextContent(fileName));
        _writer = new Editor2DTextWriter(_model);
        _reader = new Editor2DTextReader(_model);

        view.KeyPressed += OnKeyPressed;

        _view.SetLines(_reader.ReadLines());
        SetCaretPositionInView();
    }

    private void OnKeyPressed(object? sender, char e)
    {
        _writer.Write(e);
        _view.SetLines(_reader.ReadLines());
        SetCaretPositionInView();
    }

    private void SetCaretPositionInView()
    {
        var offset = _writer.PositionInText;
        var lineIndex = _model.TakeWhile(ls => ls.OffsetInText <= offset).Skip(1).Count();
        var textLine = _model[lineIndex];
        var line = lineIndex + 1;
        var column = offset - textLine.OffsetInText + 1;
        _view.SetCaretPosition(line, column, offset);
    }
}