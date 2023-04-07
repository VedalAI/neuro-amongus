using UnityEngine;

namespace Neuro.Utilities;

/// <summary>
/// Caches all components of type T in the scene (including disabled ones)
/// </summary>
/// <typeparam name="T"></typeparam>
public static class ComponentCache<T> where T : Component
{
    public static UnstableSet<T> Cached { get; } = new();

    public static UnstableSet<T> FindObjects()
    {
        Cached.Clear();

        foreach (T comp in GameObject.FindObjectsOfType<T>(true))
        {
            Cached.Add(comp, comp);
        }

        return Cached;
    }
}
