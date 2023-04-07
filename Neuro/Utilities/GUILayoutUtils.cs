using UnityEngine;

namespace Neuro.Utilities;

public static class GUILayoutUtils
{
    public static void Divider()
    {
        GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
    }
}
