using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JinGine.Core.Serialization.Strategies;

public sealed class BinaryFileSerializer : IFileSerializationStrategy
{
    private readonly Stream _stream;
    private readonly BinaryFormatter _formatter;

    public BinaryFileSerializer(string path) : this(new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
    {
    }

    public BinaryFileSerializer(byte[] bytes) : this(new MemoryStream(bytes, true))
    {
    }

    private BinaryFileSerializer(Stream stream)
    {
        _stream = stream;
        _formatter = new BinaryFormatter();
    }

    public T Deserialize<T>() where T : notnull => (T)_formatter.Deserialize(_stream);

    public void Serialize<T>(T data) where T : notnull => _formatter.Serialize(_stream, data);

    public void Dispose() => _stream.Dispose();
}