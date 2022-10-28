namespace JinGine.Core.BusinessLogic;

public interface IEditor2DTextWriter
{
    int PositionInText { get; }
    void Write(char value);
    void Write(string value);
    void WriteLine();
}