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

            var items = MenuAssembler.CreateToolStripItems(MenuDefinitions.Items, statusBar);
            MainMenuStrip.Items.AddRange(items);

            ResumeLayout();
        }
    }
}
