namespace JinGine.Core.Serialization;

public interface ISerializer<T>
{
    T Deserialize();

    void Serialize(T data);
}
