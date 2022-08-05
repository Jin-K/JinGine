using System.Reflection;
using JinGine.Core.Collections;
using JinGine.WinForms.Menu;

namespace JinGine.WinForms
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMenu();
        }
        
        private void InitializeMenu()
        {
            // TODO have a registrar to bind delegates instead of using reflection to find them
            var menuItemInfos = typeof(MenuItems)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Select(m => (Method: m, m.GetCustomAttribute<MenuItemAttribute>()))
                .Where(t => t.Item2 is not null)
                .Select(t => (t.Method, Attribute: t.Item2.OrThrowNullReference()));

            BuildMenuItemsTree(menuItemInfos);
        }

        private void BuildMenuItemsTree(IEnumerable<(MethodInfo Method, MenuItemAttribute Attribute)> menuItemInfos)
        {
            var lastMenuLevelPrefix = string.Empty;
            var menuLevelPrefixes = new UniqueStack<string>();
            var rootMenuItem = null as ToolStripMenuItem;
            var orderedInfos = menuItemInfos.OrderBy(t => t.Attribute.Level);

            foreach (var (method, attribute) in orderedInfos)
            {
                while (!attribute.Level.StartsWith(lastMenuLevelPrefix))
                {
                    lastMenuLevelPrefix = menuLevelPrefixes.Pop();
                    rootMenuItem = rootMenuItem?.OwnerItem as ToolStripMenuItem;
                }

                var clickAction = method.CreateDelegate<Action>();
                var menuItem = new MenuItemTag(clickAction, attribute, MenuItemTag_Click, MenuItemTag_MouseHover)
                    .ToMenuItem();

                if (rootMenuItem is not null)
                {
                    rootMenuItem.DropDownItems.Add(menuItem);
                }
                else
                {
                    MainMenuStrip.Items.Add(menuItem);
                }

                var levelPrefix = attribute.Level[..attribute.Level.LastIndexOf('_')];

                if (!menuLevelPrefixes.Push(levelPrefix)) continue;

                lastMenuLevelPrefix = levelPrefix;
                rootMenuItem = menuItem;
            }
        }

        // For 1 million calls
        // Best with BeginInvoke(action): 1359ms
        // Best with Invoke(action): 383ms
        // Best with direct delegate call: 3ms
        private void MenuItemTag_Click(Action action)
        {
            BeginInvoke(action);
            //Invoke(action);
            //action();
        }

        private void MenuItemTag_MouseHover(string? description)
        {
            statusBar.TextBox.Text = description;
        }
    }
}