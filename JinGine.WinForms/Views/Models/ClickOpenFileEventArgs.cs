namespace JinGine.WinForms.Views.Models;

public class ClickOpenFileEventArgs : EventArgs
{
    internal FileType FileType { get; }
    internal string FileName { get; }

    internal ClickOpenFileEventArgs(FileType fileType, string fileName)
    {
        FileType = fileType;
        FileName = fileName;
    }
}