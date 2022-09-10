using JinGine.WinForms.Menu;

namespace JinGine.WinForms
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            SuspendLayout();
            var menuAssembler = new MenuAssembler(statusBar);
            var menuItems = menuAssembler.CreateItems();
            this.MainMenuStrip.Items.AddRange(menuItems.Cast<ToolStripItem>().ToArray());
            ResumeLayout();
        }
    }
}