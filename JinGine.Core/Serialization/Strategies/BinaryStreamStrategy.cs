using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JinGine.Core.Serialization.Strategies;

public sealed class BinaryStreamStrategy : ISerializationStrategy, IDisposable
{
    private readonly BinaryFormatter _formatter;
    private readonly Stream _stream;

    public BinaryStreamStrategy(Stream stream)
    {
        _formatter = new BinaryFormatter();
        _stream = stream;
    }

    public T Deserialize<T>() where T : notnull
    {
        // TODO what happens if _stream has an incorrect length ??
        _stream.Seek(0, SeekOrigin.Begin);
        return (T)_formatter.Deserialize(_stream);
    }

    public void Serialize<T>(T data) where T : notnull
    {
        _stream.Seek(0, SeekOrigin.Begin);
        _formatter.Serialize(_stream, data);
        _stream.SetLength(_stream.Position);
    }

    public void Dispose() => _stream.Dispose();
}
