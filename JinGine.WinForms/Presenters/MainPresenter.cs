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
        var filePath = Path.Combine(Settings.Default.FilesPath, args.FileName);
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var serializer = new StrategySerializer(new BinaryStreamStrategy(fileStream));
        var data = serializer.Deserialize();

        UserControl userControl;
        switch (data)
        {
            case System.Data.DataTable dt:
            {
                var view = new DataGrid();
                var presenter = new DataGridPresenter(view, dt);
                userControl = view;
                break;
            }
            case string str:
            {
                var view = new Editor();
                var presenter = new EditorPresenter(view, str);
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
