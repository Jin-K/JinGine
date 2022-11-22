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
    private readonly List<IDisposable> _subscriptions;
    private EditorFileViewModel _viewModel;

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

        _viewModel = EditorFileViewModel.Default;
        _subscriptions = new List<IDisposable>();

        base.Dock = DockStyle.Fill;
        Disposed += OnDisposed;
    }
    
    public void SetViewModel(EditorFileViewModel viewModel)
    {
        _viewModel = viewModel;
        
        _subscriptions.Add(_viewModel.ObserveChanges(vm => vm.ColumnNumber).Subscribe(column => Column = column));
        _subscriptions.Add(_viewModel.ObserveChanges(vm => vm.LineNumber).Subscribe(line => Line = line));
        _subscriptions.Add(_viewModel.ObserveChanges(vm => vm.Offset).Subscribe(offset => Offset = offset));
        
        _editorTextViewer.SetViewModel(viewModel);
    }

    private void OnDisposed(object? sender, EventArgs e)
    {
        _subscriptions.ForEach(sub => sub.Dispose());
        _subscriptions.Clear();
    }
}
