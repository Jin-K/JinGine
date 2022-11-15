using System.Text;
using JinGine.App;
using JinGine.Domain.Models;

namespace JinGine.Infra.Services;

public class FileManagerFacade : IFileManager
{
    void IFileManager.AskCreateFileIfNotFound(string filePath) => FileManager.AskCreateFileIfNotFound(filePath);
    
    string IFileManager.ExpandPath(string path) => FileManager.ExpandPath(path);

    string IFileManager.GetTextContent(string path, Encoding? encoding) => FileManager.GetTextContent(path, encoding);

    bool IFileManager.IsUrl(string path) => FileManager.IsUrl(path);

    EditorFile IFileManager.CreateEditorFile(string path) =>
        EditorFile.OpenFromPhysicalFile(path, FileManager.GetTextContent(path));
}