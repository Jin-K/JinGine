using System;

namespace JinGine.Domain.Models;

public abstract class Entity<T> : IEquatable<Entity<T>>
{
    public T Id { get; init; }

    protected Entity(T id) => Id = id;

    public override bool Equals(object? obj) => Equals(obj as Entity<T>);

    public override int GetHashCode() => (GetType().Name, Id).GetHashCode();
    
    public bool Equals(Entity<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Id is null || other.Id is null || Id.Equals(default(T)) || other.Id.Equals(default(T)))
            return false;
        return Id.Equals(other.Id);
    }

    public static bool operator ==(Entity<T>? left, Entity<T>? right) => left?.Equals(right) ?? false;

    public static bool operator !=(Entity<T>? left, Entity<T>? right) => !(left == right);
}