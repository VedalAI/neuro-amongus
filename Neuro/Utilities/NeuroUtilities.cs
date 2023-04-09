using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Neuro.Utilities;

public static class NeuroUtilities
{
    private static readonly Dictionary<(string cls, string method, object[] param), object> _memoized = new();

    public static T Memoize<T>(T value, object[] param, [CallerFilePath] string file = null, [CallerMemberName] string method = null)
    {
        _memoized[(Path.GetFileNameWithoutExtension(file), method, param)] = value;
        return value;
    }

    public static bool IsMemoized<T>(out T result, object[] param, [CallerFilePath] string file = null, [CallerMemberName] string method = null)
    {
        if (_memoized.TryGetValue((Path.GetFileNameWithoutExtension(file), method, param), out object value))
        {
            result = (T) value;
            return true;
        }

        result = default;
        return false;
    }

    public static void WarnDoubleSingletonInstance([CallerFilePath] string file = null)
    {
        Warning($"Tried to create an instance of {Path.GetFileNameWithoutExtension(file)} when it already exists");
    }

    public static void GUILayoutDivider()
    {
        GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
    }
}
