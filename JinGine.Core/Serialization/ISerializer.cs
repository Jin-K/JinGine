namespace JinGine.Core.Serialization;

public interface ISerializer
{
    T Deserialize<T>() where T : notnull;

    void Serialize<T>(T data) where T : notnull;
}