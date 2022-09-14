namespace JinGine.Core.Serialization;

public class Serializer<T> : ISerializer<T>
{
    private readonly IStrategy _strategy;

    public Serializer(IStrategy strategy)
    {
        _strategy = strategy;
    }

    public T Deserialize() => _strategy.Deserialize<T>();

    public void Serialize(T data) => _strategy.Serialize(data);
}