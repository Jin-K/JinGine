// ReSharper disable once CheckNamespace
namespace System.Text;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendSpace(this StringBuilder @this, int repeatCount = 1)
        => @this.Append(' ', repeatCount);
}