namespace JinGine.Domain.Models;

public class EditorFile : Entity<string>
{
    public string? Path { get; }
    public FileContent Content { get; private set; }

    private EditorFile(string? path, string text) : base(path ?? string.Empty)
    {
        Path = path;
        Content = new FileContent(text);
    }

    public void ResetText(string text) => Content = new FileContent(text);

    public static EditorFile OpenFromPhysicalFile(string path, string text) => new(path, text);

    public static EditorFile PrepareNew() => new(null, string.Empty);
}