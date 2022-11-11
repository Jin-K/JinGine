namespace JinGine.Domain.Models;

public class EditorFile : Entity<string>
{
    public string? Path { get; }
    public string Content { get; }

    public EditorFile() : base(string.Empty)
    {
        Path = null;
        Content = string.Empty;
    }

    public EditorFile(string path, string content) : base(path)
    {
        Path = path;
        Content = content;
    }
}