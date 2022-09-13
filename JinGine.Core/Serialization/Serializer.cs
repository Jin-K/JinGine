namespace JinGine.Core.Serialization;

public class Serializer : ISerializer
{
    private readonly ISerializationStrategy _strategy;

    public Serializer(ISerializationStrategy strategy)
    {
        _strategy = strategy;
    }

    public T Deserialize<T>() where T : notnull => _strategy.Deserialize<T>();

    public object Deserialize() => Deserialize<object>();

    public void Serialize<T>(T data) where T : notnull => _strategy.Serialize(data);
}