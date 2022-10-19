using System.Text;

namespace JinGine.Core.Models;

public class EditorModel
{
    private readonly string? _fileName;

    public EditorText EditorText { get; }

    public EditorModel(EditorText editorText, string? fileName)
    {
        EditorText = editorText;
        _fileName = fileName;
    }

    public string GetDescriptionTitle()
    {
        var sb = new StringBuilder()
            .Append(_fileName)
            .AppendSpace(2).Append('L')
            .Append($"{EditorText.Line:d6}")
            .AppendSpace().Append('C')
            .Append($"{EditorText.Column:d4}")
            .AppendSpace().Append('P')
            .Append($"{EditorText.Position:d7}");

        if (EditorText.CurrentChar is null) return sb.ToString().TrimStart();

        var charInt = (int)EditorText.CurrentChar;
        sb.AppendSpace().Append('D');
        sb.Append($"{charInt:d}");
        sb.AppendSpace().Append('H');
        sb.Append($"{charInt:x}");

        return sb.ToString().TrimStart();
    }
}