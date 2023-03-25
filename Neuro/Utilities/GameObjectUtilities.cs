using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Utilities;

public static class GameObjectUtilities
{
    public static T CreatePermanentSingleton<T>() where T : MonoBehaviour
    {
        GameObject gameObject = new(typeof(T).Name);
        gameObject.DontDestroyOnLoad();
        return gameObject.AddComponent<T>();
    }
}
