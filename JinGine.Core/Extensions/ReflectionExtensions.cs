namespace System.Reflection
{
    public static class ReflectionExtensions
    {
        public static T OrThrowNullReference<T>(this T? attribute) where T : Attribute
        {
            return attribute.OrThrowNullReference("Attribute is null");
        }

        internal static T OrThrowNullReference<T>(this T? source, string message)
        {
            return source ?? throw new NullReferenceException(message);
        }
    }
}