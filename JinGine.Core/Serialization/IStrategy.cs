namespace JinGine.Core.Serialization;

public interface IStrategy
{
    object Deserialize();

    void Serialize(object data);
}
