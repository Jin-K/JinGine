namespace JinGine.Core.Serialization;

public abstract class BaseSerializer : ISerializer
{
    public abstract object Deserialize();

    public virtual T Deserialize<T>() where T : notnull => (T)Deserialize();

    public abstract void Serialize(object data);

    public virtual void Serialize<T>(T data) where T : notnull => Serialize(data as object);
}