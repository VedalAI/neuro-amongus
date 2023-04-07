using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Neuro.Utilities;

public sealed class UnstableSet<T> : IReadOnlyCollection<T>
{
    private readonly List<(Object anchor, T item)> _list = new();
    private readonly IEqualityComparer<T> _equalityComparer;

    public UnstableSet()
    {
        _equalityComparer = EqualityComparer<T>.Default;
    }

    public UnstableSet(IEqualityComparer<T> equalityComparer)
    {
        _equalityComparer = equalityComparer;
    }

    public int Count => _list.Count;

    public void Add(Object anchor, T item)
    {
        _list.Add((anchor, item));
        CleanupSet();
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(T item)
    {
        CleanupSet();

        foreach ((_, T otherItem) in _list)
        {
            if (_equalityComparer.Equals(item, otherItem))
            {
                return true;
            }
        }

        return false;
    }

    public bool Remove(T item)
    {
        CleanupSet();

        for (int i = 0; i < _list.Count; i++)
        {
            (_, T otherItem) = _list[i];
            if (_equalityComparer.Equals(item, otherItem))
            {
                _list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public int RemoveAll(Predicate<T> match)
    {
        CleanupSet();
        return _list.RemoveAll(t => match(t.item));
    }

    public T[] ToArray()
    {
        return _list.Select(t => t.item).ToArray();
    }

    public IEnumerator<T> GetEnumerator()
    {
        CleanupSet();

        foreach ((_, T item) in _list)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void CleanupSet()
    {
        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (!_list[i].anchor)
            {
                _list.RemoveAt(i);
            }
        }
    }
}
