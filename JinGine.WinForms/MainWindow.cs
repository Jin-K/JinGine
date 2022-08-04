using JinGine.Core.Collections;
using JinGine.WinForms.Menu;
using System.Reflection;

namespace JinGine.WinForms
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMenu();
        }

        // TODO instead, create delegates dynamically or statically bind them to improve perf
        private void InitializeMenu()
        {
            var orderedMenuItemsInfos = typeof(MenuItems)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Select(m => (Method: m, m.GetCustomAttribute<MenuItemAttribute>()))
                .Where(t => t.Item2 is not null)
                .Select(t => (t.Method, Attribute: t.Item2.OrThrowNullReference()))
                .OrderBy(t => t.Attribute.TagId);

            var lastTagIdPrefix = string.Empty;
            var parentMenuItem = null as ToolStripMenuItem;
            var subMenuTagIdPrefixes = new UniqueStack<string>();

            foreach (var (method, attribute) in orderedMenuItemsInfos)
            {
                while (!attribute.TagId.StartsWith(lastTagIdPrefix) && parentMenuItem is not null)
                {
                    lastTagIdPrefix = subMenuTagIdPrefixes.Pop();
                    parentMenuItem = parentMenuItem.OwnerItem as ToolStripMenuItem;
                }

                var currMenuItem = new ToolStripMenuItem
                {
                    Text = attribute.Title,
                    Tag = attribute.TagId,
                };

                // TODO this creates scopes for each menu item I guess, and that increases memory
                // maybe use 1 generic delegate that gets method & attribute on execution
                currMenuItem.Click += (sender, e) => BeginInvoke(() => { method.Invoke(null, null); });
                currMenuItem.MouseHover += (sender, e) => {
                    statusBar.TextBox.Text = attribute.Description;
                };

                if (parentMenuItem is not null)
                {
                    parentMenuItem.DropDownItems.Add(currMenuItem);
                }
                else
                {
                    MainMenuStrip.Items.Add(currMenuItem);
                }

                var currTagIdPrefix = attribute.TagId[..attribute.TagId.LastIndexOf('_')];

                if (subMenuTagIdPrefixes.Push(currTagIdPrefix))
                {
                    lastTagIdPrefix = currTagIdPrefix;
                    parentMenuItem = currMenuItem;
                }
            }
        }
    }
}