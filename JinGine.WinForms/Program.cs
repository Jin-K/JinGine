using JinGine.WinForms.Presenters;

namespace JinGine.WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var view = new MainForm();

            //var container = new UnityContainer()
            //    .RegisterInstance<IMainView>(view, InstanceLifetime.Singleton)
            //    .RegisterType<MainPresenter>(TypeLifetime.Singleton);

            //var _ = container.Resolve<MainPresenter>();

            var _ = new MainPresenter(view);

            Application.Run(view);
        }
    }
}
