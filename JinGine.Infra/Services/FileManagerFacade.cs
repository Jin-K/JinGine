using System.Text;
using JinGine.App;

namespace JinGine.Infra.Services;

public class FileManagerFacade : IFileManager
{
    void IFileManager.AskCreateFileIfNotFound(string filePath) => FileManager.AskCreateFileIfNotFound(filePath);
    
    string IFileManager.ExpandPath(string path) => FileManager.ExpandPath(path);

    string IFileManager.GetTextContent(string path, Encoding? encoding) => FileManager.GetTextContent(path, encoding);

    bool IFileManager.IsUrl(string path) => FileManager.IsUrl(path);
}