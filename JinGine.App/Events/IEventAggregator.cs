namespace JinGine.App.Events;

public interface IEventAggregator
{
    void Publish<T>(T @event);
    void Subscribe<T>(Action<T> action);
    void Unsubscribe<T>(Action<T> action);
}