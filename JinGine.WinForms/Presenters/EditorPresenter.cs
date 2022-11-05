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
        view.CaretLocationChanged += OnCaretPointChanged;

        _view.SetLines(_reader.ReadLines());
        _view.SetProjector(GridProjector.Create(FontDescriptor.DefaultFixed));
        SetCaretPositionInView();
    }

    private void OnCaretPointChanged(object? sender, Point e)
    {
        var segment = _model[e.Y];
        _writer.GoTo(segment.OffsetInText + e.X);
        _view.SetCaret(e.Y + 1, e.X + 1, _writer.PositionInText);
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
        var segmentIndex = _model.TakeWhile(ls => ls.OffsetInText <= offset).Skip(1).Count();
        var segment = _model[segmentIndex];
        var line = segmentIndex + 1;
        var column = offset - segment.OffsetInText + 1;
        _view.SetCaret(line, column, offset);
    }
}