namespace JinGine.App.Serialization;

public interface IBinaryFileSerializer
{
    object Deserialize(string filePath);

    T Deserialize<T>(string filePath) where T : notnull;
}