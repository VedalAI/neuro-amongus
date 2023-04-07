using UnityEngine;

namespace Neuro.Utilities.Collections;

public class AutoUnstableDictionary<TKey, TValue> : UnstableDictionary<TKey, TValue> where TKey : Object
{
    public void Add(TKey key, TValue value) => Add(key, key, value);

    public new TValue this[TKey key]
    {
        get => base[key];
        set => base[key, key] = value;
    }
}
