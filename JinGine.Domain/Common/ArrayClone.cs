using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace JinGine.Domain.Common;

// TODO should be used & moved to a separate project (Common or Runtime ?)
public readonly struct ArrayClone<T> : IEnumerable<T> where T : notnull
{
    private readonly T[] _array;

    public ArrayClone(T[] array) => _array = (T[])array.Clone();

    public ref readonly T this[int index]
    {
        get
        {
            Debug.Assert(index >= 0 && index < Length);
            return ref _array[index];
        }
    }

    public int Length => _array.Length;

    public IEnumerator<T> GetEnumerator() => new Enumerator(this, 0, _array.Length);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator ArrayClone<T>(T[] array) => new(array);

    public readonly struct Segment : IEnumerable<T>
    {
        private readonly ArrayClone<T> _arrayClone;

        public int Offset { get; }

        public int Length { get; }

        public Segment(ArrayClone<T> arrayClone) : this(arrayClone, 0) { }

        public Segment(ArrayClone<T> arrayClone, int offset)
            : this(arrayClone, offset, arrayClone.Length - offset) { }

        public Segment(ArrayClone<T> arrayClone, int offset, int length)
        {
            if (offset < 0 || offset > arrayClone.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || length > arrayClone.Length - offset) throw new ArgumentOutOfRangeException(nameof(length));

            _arrayClone = arrayClone;
            Offset = offset;
            Length = length;
        }

        public ref readonly T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < Length);
                return ref _arrayClone[Offset + index];
            }
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(_arrayClone, Offset, Offset + Length);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private struct Enumerator : IEnumerator<T>
    {
        private readonly ArrayClone<T> _arrayClone;
        private readonly int _offset;
        private readonly int _end;
        private int _pos;

        internal Enumerator(ArrayClone<T> arrayClone, int offset, int end)
        {
            _arrayClone = arrayClone;
            _offset = offset;
            _end = end;
            _pos = offset - 1;
        }

        public bool MoveNext() => ++_pos < _end;

        public void Reset() => _pos = _offset - 1;

        object IEnumerator.Current => Current;

        public T Current => this._arrayClone[this._pos];

        public void Dispose() {}
    }
}