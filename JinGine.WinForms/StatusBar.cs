namespace JinGine.WinForms
{
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
            MainWindowMediator.Instance.RegisterInfoTextBox(textBox);
        }
    }
}
