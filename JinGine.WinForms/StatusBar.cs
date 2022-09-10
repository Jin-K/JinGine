namespace JinGine.WinForms
{
    public partial class StatusBar : UserControl, IDescriber
    {
        public string Description { set => textBox.Text = value; }

        public StatusBar()
        {
            InitializeComponent();
        }
    }
}
