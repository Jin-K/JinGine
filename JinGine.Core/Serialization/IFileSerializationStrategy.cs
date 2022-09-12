using System;

namespace JinGine.Core.Serialization;

public interface IFileSerializationStrategy : IDisposable
{
    T Deserialize<T>() where T : notnull;

    void Serialize<T>(T data) where T : notnull;
}