using System.IO;
using JinGine.Domain.Models;
using JinGine.Domain.Repositories;

namespace JinGine.Infra.Repositories;

public class EditorFileRepository : IEditorFileRepository
{
    public EditorFile Get(string path)
    {
        using var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return EditorFile.OpenFromPath(path, fileStream.ReadTextToEnd());
    }

    public void Save(EditorFile file)
    {
        throw new System.NotImplementedException();
    }
}