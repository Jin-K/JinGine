using System.Collections.Generic;

namespace JinGine.Core.Collections
{
    // TODO really implement the interfaces ourselves ? because this inner hashset can be out of sync right now
    public sealed class UniqueStack<T> : Stack<T>
    {
        private readonly HashSet<T> _set = new();

        public new T Pop()
        {
            var result = base.Pop();
            _set.Remove(result);
            return result;
        }

        public new bool Push(T item)
        {
            if (_set.Add(item))
            {
                base.Push(item);
                return true;
            }

            return false;
        }
    }
}
