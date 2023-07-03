using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Neuro.Caching.Collections;

/// <summary>
/// Represents a <see cref="Dictionary{TKey,TValue}"/> in which each <see cref="KeyValuePair{TKey,TValue}"/> is anchored by an <see cref="UnityEngine.Object"/>.
/// If an anchor object is destroyed, then the dictionary entries associated with it are automatically removed.
/// </summary>
public class AnchoredUnstableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
{
    private readonly AnchoredUnstableList<(TKey key, TValue value)> _list = new();
    private readonly IEqualityComparer<TKey> _equalityComparer;

    public AnchoredUnstableDictionary(IEqualityComparer<TKey> equalityComparer)
    {
        _equalityComparer = equalityComparer;
    }

    public AnchoredUnstableDictionary() : this(EqualityComparer<TKey>.Default)
    {
    }

    public int Count => _list.Count;

    public IEnumerable<TKey> Keys
    {
        get
        {
            foreach ((TKey key, _) in _list)
            {
                yield return key;
            }
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            foreach ((_, TValue value) in _list)
            {
                yield return value;
            }
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(key, out TValue value)) throw new KeyNotFoundException("The given key was not present in the dictionary");
            return value;
        }
    }

    public TValue this[Object anchor, TKey key]
    {
        set
        {
            Remove(key);
            Add(anchor, key, value);
        }
    }

    public void Add(Object anchor, TKey key, TValue value)
    {
        if (ContainsKey(key)) throw new ArgumentException("An element with the same key already exists in the dictionary");
        _list.Add(anchor, (key, value));
    }

    public bool Remove(TKey key)
    {
        return _list.RemoveFirst(item => _equalityComparer.Equals(key, item.key));
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        foreach ((TKey otherKey, _) in _list)
        {
            if (_equalityComparer.Equals(key, otherKey))
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsValue(TValue value, IEqualityComparer<TValue> equalityComparer)
    {
        foreach ((_, TValue otherValue) in _list)
        {
            if (equalityComparer.Equals(value, otherValue))
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsValue(TValue value) => ContainsValue(value, EqualityComparer<TValue>.Default);

    public bool TryGetValue(TKey key, out TValue value)
    {
        foreach ((TKey key, TValue value) item in _list)
        {
            if (_equalityComparer.Equals(key, item.key))
            {
                value = item.value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach ((TKey key, TValue value) in _list)
        {
            yield return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}