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
            this.MainMenuStrip.Items.AddRange(MenuAssembler.CreateMenuItems(MainWindowMediator.Instance));
            ResumeLayout();
        }
    }
}