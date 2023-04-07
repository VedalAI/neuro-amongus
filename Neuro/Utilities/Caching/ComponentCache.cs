using UnityEngine;

namespace Neuro.Utilities.Caching;

/// <summary>
/// Caches all components of type T in the scene (including disabled ones)
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ComponentCache<T> : ObjectCache<T> where T : Component
{
    public static ComponentCache<T> Cached { get; } = new();

    public static ComponentCache<T> FindObjects()
    {
        Cached._list.Clear();
        Cached._list.AddRange(GameObject.FindObjectsOfType<T>(true));
        return Cached;
    }

    protected override void CleanupList()
    {
        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (!_list[i])
            {
                _list.RemoveAt(i);
            }
        }
    }
}
