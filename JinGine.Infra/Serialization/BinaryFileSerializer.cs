using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JinGine.App.Serialization;

namespace JinGine.Infra.Serialization;

public class BinaryFileSerializer : IBinaryFileSerializer
{
    // TODO find alternative to BinaryFormatter or maybe don't even deserialize binary objects (could be dangerous)
    [Obsolete("BinaryFormatter.Deserialize throws SYSLIB0011 as error now :'(")]
    public object Deserialize(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var formatter = new BinaryFormatter();
        var data = formatter.Deserialize(fs);
        fs.Close();
        return data;
    }

    public T Deserialize<T>(string filePath) where T : notnull => (T)Deserialize(filePath);
}