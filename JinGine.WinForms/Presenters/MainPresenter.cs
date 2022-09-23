using JinGine.Core.Models;
using JinGine.Core.Serialization;
using JinGine.Core.Serialization.Strategies;
using JinGine.WinForms.Properties;
using JinGine.WinForms.Views;

namespace JinGine.WinForms.Presenters;

internal class MainPresenter
{
    private readonly IMainView _mainView;

    internal MainPresenter(IMainView mainView)
    {
        _mainView = mainView;
        _mainView.ClickedOpenFile += OnClickOpenFile;
    }

    private void OnClickOpenFile(object? sender, ClickOpenFileEventArgs args)
    {
        using var fs = new FileStream(
            Path.Combine(Settings.Default.FilesPath, args.FileName),
            FileMode.Open,
            FileAccess.Read);
        var serializer = new StrategySerializer(new BinaryStreamStrategy(fs));
        var data = serializer.Deserialize();

        UserControl userControl;
        switch (data)
        {
            case System.Data.DataTable dt:
            {
                var view = new DataGrid();
                var model = new DataGridModel(dt);
                var _ = new DataGridPresenter(view, model);
                userControl = view;
                break;
            }
            case string content:
            {
                var view = new Editor();
                var fileType = args.FileName.EndsWith(".cs")
                    ? EditorModel.FileType.CSharp
                    : EditorModel.FileType.Text;

                var model = new EditorModel(fileType, content);
                var _ = new EditorPresenter(view, model);
                userControl = view;
                break;
            }
            default:
                var message = string.Format(ExceptionMessages.MainPresenter_Cannot_Handle_Type, data.GetType().FullName);
                throw new NotSupportedException(message);
        }

        _mainView.ShowInNewTab(args.FileName, userControl);
    }
}
