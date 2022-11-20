using System.ComponentModel;
using JinGine.WinForms.ViewModels;
using JinGine.WinForms.Views;
using JinGine.WinForms.Views.Models;

namespace JinGine.WinForms.Controls;

/// <summary>
/// Represents our custom code-editor control.
/// </summary>
public partial class Editor : UserControl, IEditorView
{
    private EditorFileViewModel _viewModel = EditorFileViewModel.Default;

    private int Line { set => _lineLabel.Text = Convert.ToString(value); }
    private int Column { set => _columnLabel.Text = Convert.ToString(value); }
    private int Offset { set => _offsetLabel.Text = Convert.ToString(value); }

    [Bindable(true)]
    public TextSelectionRange TextSelection
    {
        get => _editorTextViewer.TextSelection;
        set => _editorTextViewer.TextSelection = value;
    }

    public event EventHandler<Point> CaretPointChanged
    {
        add => _editorTextViewer.CaretPointChanged += value;
        remove => _editorTextViewer.CaretPointChanged -= value;
    }

    public event EventHandler<char> KeyPressed
    {
        add => _editorTextViewer.KeyPressed += value;
        remove => _editorTextViewer.KeyPressed -= value;
    }

    public Editor()
    {
        InitializeComponent();
        base.Dock = DockStyle.Fill;
    }
    
    public void SetViewModel(EditorFileViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _editorTextViewer.SetViewModel(viewModel);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_viewModel.ColumnNumber):
                Column = _viewModel.ColumnNumber;
                break;
            case nameof(_viewModel.LineNumber):
                Line = _viewModel.LineNumber;
                break;
            case nameof(_viewModel.Offset):
                Offset = _viewModel.Offset;
                break;
        }
    }
}
