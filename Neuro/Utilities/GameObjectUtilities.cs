using Neuro.DependencyInjection;
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

    public static T CreatePermanentSingleton<T>(IContextProvider context) where T : MonoBehaviour, IContextAccepter
    {
        T component = CreatePermanentSingleton<T>();
        component.Context = context;
        return component;
    }
}
