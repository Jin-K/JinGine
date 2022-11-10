using JinGine.App;
using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.App.Handlers;
using JinGine.Infra.Services;
using JinGine.WinForms.Presenters;
using JinGine.WinForms.Properties;
using Unity;

namespace JinGine.WinForms;

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

        var container = new UnityContainer()
            .RegisterInstance(new AppSettings(Settings.Default.FilesPath), InstanceLifetime.Singleton)
            .RegisterInstance(EventAggregator.Instance, InstanceLifetime.Singleton)
            .RegisterType<IFileManager, FileManagerFacade>()
            .RegisterType<ICommandHandler<OpenBinaryFileCommand>, OpenBinaryFileCommandHandler>()
            .RegisterType<ICommandHandler<OpenCSharpFileCommand>, OpenCSharpFileCommandHandler>()
            .RegisterType<ICommandDispatcher, CommandDispatcher>()
            .RegisterSingleton<MainMenuFactory>()
            .AddExtension(new Diagnostic());


        var menuFactory = container.Resolve<MainMenuFactory>();
        view.Tag = new MainPresenter(view, menuFactory);

        Application.Run(view);
    }
}