using System.Text;

namespace JinGine.App;

public interface IFileManager
{
    void AskCreateFileIfNotFound(string filePath);
    string ExpandPath(string path);
    string GetTextContent(string path, Encoding? encoding = null);
    bool IsUrl(string path);
}