using System.Data;
using JinGine.Core.Models;
using JinGine.Core.Serialization;
using JinGine.Core.Serialization.Strategies;
using JinGine.WinForms.Properties;
using JinGine.WinForms.Views;
using LegacyFwk;

namespace JinGine.WinForms.Presenters;

internal class MainPresenter
{
    private readonly IMainView _mainView;

    internal MainPresenter(IMainView mainView)
    {
        _mainView = mainView;
        _mainView.ClickedOpenFile += OnClickOpenFile;
    }

    // ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806 // Do not ignore method results
    private void OnClickOpenFile(object? sender, ClickOpenFileEventArgs args)
    {
        var fileName = FileManager.ExpandPath(Path.Combine(Settings.Default.FilesPath, args.FileName));

        switch (args.FileType)
        {
            case FileType.DataTable:
            {
                using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var serializer = new StrategySerializer(new BinaryStreamStrategy(fs));
                var data = serializer.Deserialize<DataTable>();
                var model = new DataGridModel(data);

                var view = new DataGrid();
                new DataGridPresenter(view, model);

                _mainView.ShowInNewTab(fileName, view);
                return;
            }
            case FileType.CSharp:
            {
                if (FileManager.IsUrl(fileName) is not true)
                    FileManager.AskCreateFileIfNotFound(fileName);
                
                var model = new EditorModel(
                    new EditorText(FileManager.GetTextContent(fileName)),
                    fileName);

                var view = new Editor { Dock = DockStyle.Fill };
                new EditorPresenter(view, model);

                _mainView.ShowInNewTab(fileName, view);
                return;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(args), string.Format(ExceptionMessages.MainPresenter_FileType_is_not_supported_, nameof(args.FileType)));
        }
    }
#pragma warning restore CA1806 // Do not ignore method results
    // ReSharper restore ObjectCreationAsStatement
}
