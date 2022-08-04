namespace JinGine.WinForms.Menu
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class MenuItemAttribute : Attribute
    {
        internal string TagId { get; }

        internal string Title { get; }

        internal string? Description { get; }

        internal string? HotKey { get; }

        internal MenuItemAttribute(string tagId, string title, string? description = null, string? hotKey = null)
        {
            TagId = tagId;
            Title = title;
            Description = description;
            HotKey = hotKey;
        }
    }
}
