namespace JinGine.WinForms.Views;

internal interface IMainView
{
    IStatusBarView StatusBar { get; }

    event EventHandler TabClosed; 

    void SetMenuItems(ToolStripItem[] items);

    void ShowInNewTab(string name, Control control);
}
