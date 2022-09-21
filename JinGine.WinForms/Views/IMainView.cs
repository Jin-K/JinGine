namespace JinGine.WinForms.Views;

internal interface IMainView
{
    event EventHandler<ClickOpenFileEventArgs> ClickedOpenFile;

    void OpenInTab(string name, Control control);
}