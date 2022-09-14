namespace JinGine.Core.Serialization;

public interface IStrategy
{
    T Deserialize<T>();

    void Serialize<T>(T data);
}
