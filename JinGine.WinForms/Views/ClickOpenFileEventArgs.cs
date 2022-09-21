namespace JinGine.WinForms.Views;

public class ClickOpenFileEventArgs : EventArgs
{
    internal string FileName { get; }

    internal ClickOpenFileEventArgs(string fileName)
    {
        FileName = fileName;
    }
}