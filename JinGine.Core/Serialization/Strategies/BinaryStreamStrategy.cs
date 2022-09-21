using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JinGine.Core.Serialization.Strategies;

public sealed class BinaryStreamStrategy : IStrategy, IDisposable
{
    private readonly BinaryFormatter _formatter;
    private readonly Stream _stream;

    public BinaryStreamStrategy(Stream stream)
    {
        _formatter = new BinaryFormatter();
        _stream = stream;
    }

    public object Deserialize()
    {
        _stream.Seek(0, SeekOrigin.Begin);
        return _formatter.Deserialize(_stream); // TODO what happens if _stream has an incorrect length ??
    }

    public void Serialize(object data)
    {
        _stream.Seek(0, SeekOrigin.Begin);
        _formatter.Serialize(_stream, data);
        _stream.Truncate();
    }

    public void Dispose() => _stream.Dispose();
}
