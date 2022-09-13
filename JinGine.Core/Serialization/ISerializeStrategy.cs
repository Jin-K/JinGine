namespace JinGine.Core.Serialization;

public interface ISerializationStrategy
{
    T Deserialize<T>() where T : notnull;

    void Serialize<T>(T data) where T : notnull;
}
