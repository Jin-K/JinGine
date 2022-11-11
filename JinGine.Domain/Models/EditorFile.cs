namespace JinGine.Domain.Models;

public class EditorFile : Entity<string>
{
    public string? Path { get; }
    public string Content { get; }

    private EditorFile(string? path, string content) : base(path ?? string.Empty)
    {
        Path = path;
        Content = content;
    }

    public static EditorFile CreateFrom(PhysicalTextFile textFile) => new(textFile.Path, textFile.Content);

    public static EditorFile PrepareNew() => new(null, string.Empty);
}