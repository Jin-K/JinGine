namespace JinGine.Domain.Models;

public class EditorFile : Entity<string>
{
    public string? Path { get; }
    public FileContent Content { get; private set; }

    private EditorFile(string? path, string textContent) : base(path ?? string.Empty)
    {
        Path = path;
        Content = new FileContent(textContent);
    }

    public void ResetTextContent(string textContent) => Content = new FileContent(textContent);

    public static EditorFile OpenFromPhysicalFile(string path, string textContent) => new(path, textContent);

    public static EditorFile PrepareNew() => new(null, string.Empty);
}