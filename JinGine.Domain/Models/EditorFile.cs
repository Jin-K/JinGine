namespace JinGine.Domain.Models;

public class EditorFile : Entity<string?>
{
    public string Content { get; }

    private EditorFile(string? id, string text) : base(id) => Content = text;

    public static EditorFile OpenFromPath(string path, string content) => new(path, content);

    public static EditorFile PrepareNew() => new(null, string.Empty);
}