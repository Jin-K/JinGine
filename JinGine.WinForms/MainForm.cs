using JinGine.WinForms.Views;

namespace JinGine.WinForms
{
    public partial class MainForm : Form, IMainView
    {
        public event EventHandler? TabClosed;

        public IStatusBarView StatusBar => _statusBar;
        
        public MainForm() => InitializeComponent();

        public void SetMenuItems(ToolStripItem[] items) => MainMenuStrip.Items.AddRange(items);

        public void ShowInNewTab(string name, Control control)
        {
            var newTab = new TabPage(name);
            _tabsControl.TabPages.Add(newTab);
            newTab.Controls.Add(control);
            control.Focus();
        }
    }
}
