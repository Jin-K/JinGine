namespace JinGine.App;

public class AppSettings
{
    public string FilesPath { get; }

    public AppSettings(string filesPath)
    {
        FilesPath = filesPath;
    }
}