using System;
using UnityEngine;

namespace Neuro.Utilities;

public sealed class DisabledScope : IDisposable
{
    private readonly bool _oldEnabled;

    public DisabledScope(bool disabled)
    {
        _oldEnabled = GUI.enabled;
        GUI.enabled = !disabled;
    }

    public void Dispose()
    {
        GUI.enabled = _oldEnabled;
    }
}

public sealed class HorizontalScope : IDisposable
{
    public HorizontalScope()
    {
        GUILayout.BeginHorizontal();
    }

    public void Dispose()
    {
        GUILayout.EndHorizontal();
    }
}

public sealed class VerticalScope : IDisposable
{
    public VerticalScope()
    {
        GUILayout.BeginVertical();
    }

    public void Dispose()
    {
        GUILayout.EndVertical();
    }
}
