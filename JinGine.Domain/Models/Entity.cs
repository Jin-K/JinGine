namespace JinGine.Domain.Models;

public abstract class Entity<T> where T : notnull
{
    public T Id { get; init; }

    protected Entity(T id) => Id = id;
}