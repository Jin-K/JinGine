using System.ComponentModel;
using System.Reactive.Linq;
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

        _subscriptions = new List<IDisposable>();
        _viewModel = EditorFileViewModel.Default;

        base.Dock = DockStyle.Fill;
        Disposed += ClearSubscriptions;
    }
    
    public void SetViewModel(EditorFileViewModel viewModel)
    {
        _viewModel = viewModel;
        _editorTextViewer.SetViewModel(viewModel);
        
        ClearSubscriptions();

        _subscriptions.Add(_viewModel
            .ObserveChanges(vm => vm.ColumnNumber)
            .Subscribe(column => _colTextBox.Text = column.ToString()));
        _subscriptions.Add(_viewModel
            .ObserveChanges(vm => vm.LineNumber)
            .Subscribe(line => _lineTextBox.Text = line.ToString()));
        _subscriptions.Add(_viewModel
            .ObserveChanges(vm => vm.Offset)
            .Select(offset => offset + 1)
            .Subscribe(position => _posTextBox.Text = position.ToString()));
    }

    private void ClearSubscriptions(object? sender = null, EventArgs? args = null)
    {
        _subscriptions.ForEach(sub => sub.Dispose());
        _subscriptions.Clear();
    }
}
