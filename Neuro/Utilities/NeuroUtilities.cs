using System.IO;
using System.Runtime.CompilerServices;
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

    public static void WarnDoubleSingletonInstance([CallerFilePath] string file = null)
    {
        Warning($"Tried to create an instance of {Path.GetFileNameWithoutExtension(file)} when it already exists");
    }

    public static void GUILayoutDivider()
    {
        GUILayout.Label(string.Empty, GUI.skin.horizontalSlider);
    }

    private static Camera cache;

    public static Camera CameraMain
    {
        get
        {
            if (cache == null)
            {
                cache = Camera.main;
            }
            return cache;
        }
    }
}
