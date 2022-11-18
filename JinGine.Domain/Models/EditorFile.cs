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

    public static EditorFile OpenFromPath(string path, System.Func<string> textResolver) => new(path, textResolver());

    public static EditorFile PrepareNew() => new(null, string.Empty);
}