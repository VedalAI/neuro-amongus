using Neuro.Caching.Collections;
using UnityEngine;

namespace Neuro.Caching;

/// <summary>
/// Caches all components of type T in the scene (including disabled ones)
/// </summary>
/// <typeparam name="T"></typeparam>
public static class ComponentCache<T> where T : Component
{
    public static UnstableList<T> Cached { get; } = new();

    public static UnstableList<T> FetchObjects()
    {
        Cached.Clear();
        Cached.AddRange(GameObject.FindObjectsOfType<T>(true));
        return Cached;
    }
}