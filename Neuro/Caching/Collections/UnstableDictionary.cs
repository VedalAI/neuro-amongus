using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Neuro.Caching.Collections;

/// <summary>
/// Represents a <see cref="Dictionary{TKey,TValue}"/> in which each <see cref="KeyValuePair{TKey,TValue}"/> is anchored by its <see cref="TKey"/>.
/// If an key object is destroyed, then the dictionary entry is automatically removed.
/// </summary>
public class UnstableDictionary<TKey, TValue> : AnchoredUnstableDictionary<TKey, TValue> where TKey : Object
{
    public void Add(TKey key, TValue value) => base.Add(key, key, value);

    public new TValue this[TKey key]
    {
        get => base[key];
        set => base[key, key] = value;
    }

    [Obsolete("Use .Add(TKey, TValue) instead.", true)]
    public new void Add(Object anchor, TKey key, TValue value) => base.Add(anchor, key, value);

    [Obsolete("Use .this[TKey] instead.", true)]
    public new TValue this[Object anchor, TKey key]
    {
        set => base[anchor, key] = value;
    }
}