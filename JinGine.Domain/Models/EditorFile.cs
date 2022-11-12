namespace JinGine.Domain.Models;

public class EditorFile : Entity<string>
{
    public string? Path { get; }
    public FileContent Content { get; }

    private EditorFile(string? path, string content) : base(path ?? string.Empty)
    {
        Path = path;
        Content = new FileContent(content);
    }

    public static EditorFile OpenFrom(PhysicalFile textFile) => new(textFile.Path, textFile.TextContent);

    public static EditorFile PrepareNew() => new(null, string.Empty);
}