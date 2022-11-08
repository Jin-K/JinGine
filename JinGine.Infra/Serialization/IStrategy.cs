namespace JinGine.Infra.Serialization;

public interface IStrategy
{
    object Deserialize();

    void Serialize(object data);
}
