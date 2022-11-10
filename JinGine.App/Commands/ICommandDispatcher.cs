namespace JinGine.App.Commands;

public interface ICommandDispatcher
{
    void Dispatch<TCommand>(TCommand command) where TCommand : ICommand;
}