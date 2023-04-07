using System;
using Object = UnityEngine.Object;

namespace Neuro.Utilities.Collections;

public class SelfUnstableDictionary<TKey, TValue> : UnstableDictionary<TKey, TValue> where TKey : Object
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
