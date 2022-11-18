using JinGine.Domain.Models;

namespace JinGine.App;

public interface IFileManager
{
    void AskCreateFileIfNotFound(string filePath);
    string ExpandPath(string path);
    bool IsUrl(string path);
    EditorFile CreateEditorFile(string path);
}