namespace JinGine.Infra.Serialization;

public interface ISerializer
{
    object Deserialize();

    T Deserialize<T>() where T : notnull;

    void Serialize(object data);

    void Serialize<T>(T data) where T : notnull;
}
