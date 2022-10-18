using JinGine.WinForms.Views;

namespace JinGine.WinForms
{
    public partial class MainForm : Form, IMainView
    {
        public event EventHandler<ClickOpenFileEventArgs>? ClickedOpenFile;

        public MainForm()
        {
            InitializeComponent();
            InitializeMenuItems();
        }

        private void InitializeMenuItems()
        {
            // level 3
            var openFile1AItem = new ToolStripMenuItem("Open file 1 A")
                .OnMouseHover((_, _) => statusBar.Info = "Open file 1 A for real")
                .OnClick((_, _) => ClickedOpenFile?.Invoke(
                    this,
                    new ClickOpenFileEventArgs(FileType.DataTable, "File1A.bin")));
            var openFile1BItem = new ToolStripMenuItem("Open file 1 B")
                .OnMouseHover((_, _) => statusBar.Info = "Open file 1 B for real")
                .OnClick((_, _) => ClickedOpenFile?.Invoke(
                    this,
                    new ClickOpenFileEventArgs(FileType.CSharp, "File1A.bin")));

            // level 2
            var openFile1Item = new ToolStripMenuItem("Open file 1")
                .OnMouseHover((_, _) => statusBar.Info = "Open file 1 operations")
                .WithChildren(openFile1AItem, openFile1BItem);
            var openFile2Item = new ToolStripMenuItem("Open file 2")
                .OnMouseHover((_, _) => statusBar.Info = "Open file 2 operations");

            // level 1
            var fileItem = new ToolStripMenuItem("File")
                .WithChildren(openFile1Item, openFile2Item);

            MainMenuStrip.Items.Add(fileItem);
        }

        public void ShowInNewTab(string name, Control control)
        {
            var newTab = new TabPage(name);
            tabsControl.TabPages.Add(newTab);
            newTab.Controls.Add(control);
            control.Focus();
        }
    }
}
