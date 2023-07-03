using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using Reactor.Utilities.Extensions;

namespace Neuro.Extensions;

public static class Il2CppTypeExtensions
{
    private static readonly MethodInfo _castMethod = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.Cast));

    public static IEnumerable<T> OfIl2CppType<T>(this IEnumerable collection) where T : Il2CppObjectBase
    {
        foreach (object obj in collection)
        {
            if (obj is not Il2CppObjectBase il2cppObject) continue;

            T newObj = il2cppObject.TryCast<T>();
            if (newObj != null) yield return newObj;
        }
    }

    public static T Il2CppCastToTopLevel<T>(this T obj) where T : Il2CppSystem.Object
    {
        if (obj == null) return null;
        return (T) _castMethod.MakeGenericMethod(obj.GetIl2CppType().ToSystemType()).Invoke(obj, null);
    }

    public static Il2CppSystem.Object Il2CppCastToTopLevel(this Il2CppObjectBase obj)
        => Il2CppCastToTopLevel(obj.Cast<Il2CppSystem.Object>());
}