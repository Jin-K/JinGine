using System.Collections;
using System.Collections.Concurrent;

namespace JinGine.App.Events;

public sealed class EventAggregator : IEventAggregator, IDisposable
{
    // TODO compare/benchmark IList & IList<object>
    private readonly ConcurrentDictionary<Type, IList> _subscriptions = new();

    public static readonly EventAggregator Instance = new();

    private EventAggregator() { }

    public void Publish<T>(T @event)
    {
        if (_subscriptions.TryGetValue(typeof(T), out var subscribers) is false) return;

        // TODO compare/benchmark ArrayList.GetEnumerator() vs List<object>.GetEnumerator()
        foreach (Action<T> subscriber in subscribers)
            subscriber(@event);
    }

    public void Subscribe<T>(Action<T> action)
    {
        // TODO compare/benchmark new ArrayList() & new List<object>()
        var subscribers = _subscriptions.GetOrAdd(typeof(T), _ => new ArrayList());
        
        // TODO compare/benchmark IList.Add vs List<object>.Add
        lock (subscribers) subscribers.Add(action);
    }

    public void Unsubscribe<T>(Action<T> action)
    {
        if (_subscriptions.TryGetValue(typeof(T), out var subscribers) is false) return;

        // TODO compare/benchmark IList.Remove vs List<object>.Remove
        lock (subscribers) subscribers.Remove(action);
    }

    public void Dispose()
    {
        _subscriptions.Clear();
    }
}
