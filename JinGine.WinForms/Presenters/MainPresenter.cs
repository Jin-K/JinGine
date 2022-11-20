using System.Data;
using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.Domain.Models;
using JinGine.WinForms.Controls;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

internal class MainPresenter
{
    private readonly IMainView _mainView;
    //private readonly ISet<IDisposable> _disposablePresenters;

    internal MainPresenter(
        IMainView mainView,
        MainMenuFactory menuFactory,
        IEventAggregator eventAggregator)
    {
        _mainView = mainView;
        //_disposablePresenters = new HashSet<IDisposable>();

        mainView.TabClosed += OnTabClosed;

        eventAggregator.Subscribe<UpdateStatusBarInfoEvent>(@event => _mainView.StatusBar.Info = @event.Info);
        eventAggregator.Subscribe<LoadFileDataEvent>(OnLoadFileDataEvent);
        
        CreateAndSetMenuItems(menuFactory);
    }

    private void CreateAndSetMenuItems(MainMenuFactory menuFactory)
    {
        var menuDefs = new MenuItemDef[]
        {
            ("File", null, null, new MenuItemDef[]
            {
                ("Open file 1", "Open file 1 operations", null, new MenuItemDef[]
                {
                    ("Open file 1 A", "Open file 1 A for real", new OpenBinaryFileCommand("File1A.bin"), null),
                    ("Open file 1 B", "Open file 1 B for real", new OpenCSharpFileCommand("File1A.bin"), null)
                }),
                ("Open file 2", "Open file 2 operations", null, null)
            })
        };
        _mainView.SetMenuItems(menuFactory.CreateItems(menuDefs));
    }

    private void OnLoadFileDataEvent(LoadFileDataEvent @event)
    {
        switch (@event.FileData)
        {
            case DataTable dt:
            {
                var model = new DataGridModel(dt);
                var view = new DataGrid();
                _ = new DataGridPresenter(view, model);
                _mainView.ShowInNewTab(@event.FileName, view);
                break;
            }
            case EditorFile editorFile:
            {
                var view = new Editor();
                _ = new EditorPresenter(view, editorFile);
                _mainView.ShowInNewTab(@event.FileName, view);
                break;
            }
            default: throw new SystemException($"Unknown type {@event.FileData.GetType().FullName}");
        }
    }

    private void OnTabClosed(object? sender, EventArgs e)
    {
        //if (sender is IDisposable disposable && _disposablePresenters.Contains(disposable))
        //{
        //    disposable.Dispose();
        //    _disposablePresenters.Remove(disposable);
        //}
    }
}
