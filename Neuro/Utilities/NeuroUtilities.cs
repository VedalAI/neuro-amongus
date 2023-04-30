using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Neuro.Minigames;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Utilities;

public static class NeuroUtilities
{
    public static Material MaskShaderMat
    {
        get
        {
            if (!_maskShaderMat)
            {
                _maskShaderMat = new Material(Shader.Find("Unlit/MaskShader"));
                _maskShaderMat.DontDestroy();
            }

            return _maskShaderMat;
        }
    }
    private static Material _maskShaderMat;

    public static Camera MainCamera => _mainCamera ? _mainCamera : _mainCamera = Camera.main;
    private static Camera _mainCamera;

    public static void WarnDoubleSingletonInstance([CallerFilePath] string file = null)
    {
        Warning($"Tried to create an instance of {Path.GetFileNameWithoutExtension(file)} when it already exists");
    }

    public static void GUILayoutDivider()
    {
        GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
    }

    public static List<Console> GetOpenableConsoles(bool includeSabotages)
    {
        return PlayerControl.LocalPlayer.myTasks._items.Where(t => t && !t.IsComplete && !t.TryCast<ImportantTextTask>())
            .Where(t => includeSabotages || !PlayerTask.TaskIsEmergency(t))
            .SelectMany(t => t.FindConsoles()._items.Where(c => c).Select(c => (t, c)))
            .Where(e => MinigameHandler.ShouldOpenConsole(e.c, e.t.MinigamePrefab, e.t))
            .Select(e => e.c).ToList();
    }
}
