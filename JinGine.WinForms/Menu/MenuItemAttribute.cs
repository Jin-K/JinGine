namespace JinGine.WinForms.Menu
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class MenuItemAttribute : Attribute
    {
        internal string Level { get; }

        internal string Title { get; }

        internal string? Description { get; }

        internal string? HotKey { get; }

        internal MenuItemAttribute(string level, string title, string? description = null, string? hotKey = null)
        {
            Level = level;
            Title = title;
            Description = description;
            HotKey = hotKey;
        }
    }
}
