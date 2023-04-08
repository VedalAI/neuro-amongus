using System.IO;
using System.Runtime.CompilerServices;
using Neuro.Pathfinding;
using UnityEngine;

namespace Neuro.Utilities;

public static class NeuroUtilities
{
    public static void WarnDoubleSingletonInstance([CallerFilePath] string file = null)
    {
        Warning($"Tried to create an instance of {Path.GetFileNameWithoutExtension(file)} when it already exists");
    }

    public static void GUILayoutDivider()
    {
        GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
    }

    public static float DistanceToPlayerStraight(PositionProvider position) => Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), position);
}
