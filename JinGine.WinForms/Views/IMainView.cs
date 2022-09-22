namespace JinGine.WinForms.Views;

internal interface IMainView
{
    event EventHandler<ClickOpenFileEventArgs> ClickedOpenFile;

    void ShowInNewTab(string name, Control control);
}
