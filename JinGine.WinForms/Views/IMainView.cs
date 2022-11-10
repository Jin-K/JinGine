using JinGine.WinForms.Views.Models;

namespace JinGine.WinForms.Views;

internal interface IMainView
{
    IStatusBarView StatusBar { get; }

    void SetMenuItems(ToolStripItem[] items);

    void ShowInNewTab(string name, Control control);
}
