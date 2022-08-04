namespace System.Collections.Generic
{
    public static class StackExtensions
    {
        public static T? PeekOrDefault<T>(this Stack<T> source)
        {
            if (source.TryPeek(out var result)) return result;
            return default;
        }
    }
}