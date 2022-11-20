using JinGine.App;
using JinGine.App.Commands;
using JinGine.App.Events;
using JinGine.App.Handlers;
using JinGine.App.Serialization;
using JinGine.Domain.Repositories;
using JinGine.Infra;
using JinGine.Infra.Repositories;
using JinGine.Infra.Serialization;
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
            .RegisterInstance<IEventAggregator>(EventAggregator.Instance, InstanceLifetime.Singleton)
            .RegisterSingleton<ICommandHandler<OpenBinaryFileCommand>, OpenBinaryFileCommandHandler>()
            .RegisterSingleton<ICommandHandler<OpenCSharpFileCommand>, OpenCSharpFileCommandHandler>()
            .RegisterSingleton<ICommandDispatcher, CommandDispatcher>()
            .RegisterSingleton<MainMenuFactory>()
            .RegisterSingleton<IBinaryFileSerializer, BinaryFileSerializer>()
            .RegisterSingleton<IEditorFileRepository, EditorFileRepository>()
            .AddExtension(new Diagnostic());

        var menuFactory = container.Resolve<MainMenuFactory>();
        var eventAggregator = container.Resolve<IEventAggregator>();
        view.Tag = new MainPresenter(view, menuFactory, eventAggregator);

        Application.Run(view);
    }
}