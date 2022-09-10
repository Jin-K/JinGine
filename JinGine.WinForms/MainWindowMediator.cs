namespace JinGine.WinForms;

internal class MainWindowMediator : IInfoMediator
{
    private TextBox? _textBox;
    private static MainWindowMediator? _instance;

    internal static MainWindowMediator Instance => _instance ??= new MainWindowMediator();

    private MainWindowMediator() {}

    internal void RegisterInfoTextBox(TextBox textBox)
    {
        _textBox = textBox;
    }

    public void ShowInfo(string info)
    {
        if (_textBox is null)
        {
            throw new InvalidOperationException("Target control needs to be registered.");
        }

        _textBox.Text = info;
    }
}