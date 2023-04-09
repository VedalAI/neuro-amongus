using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using Neuro.Utilities.Convertors;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Utilities;

public static class NeuroUtilities
{
    private static readonly MethodInfo _castMethod = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.Cast));

    public static void WarnDoubleSingletonInstance([CallerFilePath] string file = null)
    {
        Warning($"Tried to create an instance of {Path.GetFileNameWithoutExtension(file)} when it already exists");
    }

    public static void GUILayoutDivider()
    {
        GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
    }

    public static T Il2CppCastToTopLevel<T>(this T obj) where T : Il2CppObjectBase
    {
        return (T)_castMethod.MakeGenericMethod(obj.Cast<Il2CppSystem.Object>().GetIl2CppType().ToSystemType()).Invoke(obj, null);
    }

    public static float DistanceToPlayerStraight(PositionProvider position) => Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), position);
}
