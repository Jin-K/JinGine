namespace System.Collections.Generic;

public static class StackExtensions
{
    public static T? PeekOrDefault<T>(this Stack<T> source)
        => source.TryPeek(out var result) ? result : default;
}