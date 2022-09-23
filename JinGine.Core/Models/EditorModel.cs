namespace JinGine.Core.Models;

public class EditorModel
{
    public FileType Type { get; }
    
    public string Content { get; }

    public EditorModel(FileType type, string content)
    {
        Type = type;
        Content = content;
    }

    public enum FileType
    {
        Text,
        CSharp,
    }
}