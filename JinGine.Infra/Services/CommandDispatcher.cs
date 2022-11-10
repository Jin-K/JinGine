using JinGine.App.Commands;
using JinGine.App.Handlers;
using Unity;

namespace JinGine.Infra.Services;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IUnityContainer _container;

    public CommandDispatcher(IUnityContainer container)
    {
        _container = container;
    }

    public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = _container.Resolve<ICommandHandler<TCommand>>();
        handler.Handle(command);
    }
}