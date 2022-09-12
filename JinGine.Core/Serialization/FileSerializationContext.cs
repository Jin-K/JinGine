using System;

namespace JinGine.Core.Serialization;

public class FileSerializationContext : IDisposable
{
    private readonly IFileSerializationStrategy _strategy;

    public FileSerializationContext(IFileSerializationStrategy strategy)
    {
        _strategy = strategy;
    }

    public T Deserialize<T>() where T : notnull => _strategy.Deserialize<T>();

    public object Deserialize() => Deserialize<object>();

    public void Serialize<T>(T data) where T : notnull => _strategy.Serialize(data);

    public void Serialize(object data) => Serialize<object>(data);

    public void Dispose() => _strategy.Dispose();
}