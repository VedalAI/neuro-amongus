using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Neuro.Caching.Collections;

/// <summary>
/// Represents a <see cref="List{T}"/> in which each value is anchored by an <see cref="UnityEngine.Object"/>.
/// If an anchor object is destroyed, then the list entries associated with it are automatically removed.
/// </summary>
public class AnchoredUnstableList<T> : IReadOnlyList<T>
{
    private readonly List<(Object anchor, T item)> _list = new();

    public int Count
    {
        get
        {
            CleanupList();
            return _list.Count;
        }
    }

    public void Add(Object anchor, T item)
    {
        _list.Add((anchor, item));
        CleanupList();
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(T item, IEqualityComparer<T> equalityComparer)
    {
        CleanupList();

        foreach ((_, T otherItem) in _list)
        {
            if (equalityComparer.Equals(item, otherItem))
            {
                return true;
            }
        }

        return false;
    }

    public bool Contains(T item) => Contains(item, EqualityComparer<T>.Default);

    public bool Remove(T item, IEqualityComparer<T> equalityComparer) => RemoveFirst(otherItem => equalityComparer.Equals(item, otherItem));

    public bool Remove(T item) => Remove(item, EqualityComparer<T>.Default);

    public bool RemoveFirst(Predicate<T> match)
    {
        CleanupList();

        for (int i = 0; i < _list.Count; i++)
        {
            (_, T otherItem) = _list[i];
            if (match(otherItem))
            {
                _list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public int RemoveAll(Predicate<T> match)
    {
        CleanupList();
        return _list.RemoveAll(t => match(t.item));
    }

    public T[] ToArray()
    {
        CleanupList();
        return _list.Select(t => t.item).ToArray();
    }

    public IEnumerator<T> GetEnumerator()
    {
        CleanupList();

        foreach ((_, T item) in _list)
        {
            yield return item;
        }
    }

    public T this[int index]
    {
        get
        {
            CleanupList();
            return _list[index].item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void CleanupList()
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
