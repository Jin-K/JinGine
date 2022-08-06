using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JinGine.Core.Serialization;

public sealed class FileSerializer : IDisposable
{
    private readonly FileStream _fileStream;
    private readonly BinaryFormatter _formatter;

    public FileSerializer(string path)
    {
        _fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
        _formatter = new BinaryFormatter();
    }

    public object Deserialize() => _formatter.Deserialize(_fileStream);

    public T Deserialize<T>() => (T)Deserialize();

    public void Dispose() => _fileStream.Dispose();
}