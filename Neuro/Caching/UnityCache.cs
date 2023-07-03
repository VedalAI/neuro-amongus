using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Caching;

public static class UnityCache
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
}