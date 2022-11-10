using JinGine.App.Commands;

namespace JinGine.App.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    void Handle(TCommand command);
}