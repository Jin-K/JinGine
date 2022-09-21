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

        switch (data)
        {
            case System.Data.DataTable dt:
                var gridView = new DataGrid();
                var _ = new DataGridPresenter(gridView, dt);
                _mainView.OpenInTab(args.FileName, gridView);
                break;
            default:
                var message = string.Format(ExceptionMessages.MainPresenter_Cannot_Handle_Type, data.GetType().FullName);
                throw new NotSupportedException(message);
        }
    }
}