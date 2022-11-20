using JinGine.Domain.Models;

namespace JinGine.Domain.Repositories;

// why not actually ?
// they can be persisted, to a file system instead of a database
public interface IEditorFileRepository
{
    EditorFile Get(string path);

    void Save(EditorFile file);
}