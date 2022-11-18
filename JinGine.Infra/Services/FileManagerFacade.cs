using JinGine.App;
using JinGine.Domain.Models;

namespace JinGine.Infra.Services;

public class FileManagerFacade : IFileManager
{
    void IFileManager.AskCreateFileIfNotFound(string filePath) => FileManager.AskCreateFileIfNotFound(filePath);
    
    string IFileManager.ExpandPath(string path) => FileManager.ExpandPath(path);

    bool IFileManager.IsUrl(string path) => FileManager.IsUrl(path);

    EditorFile IFileManager.CreateEditorFile(string path) =>
        EditorFile.OpenFromPhysicalFile(path, FileManager.GetText(path));
}