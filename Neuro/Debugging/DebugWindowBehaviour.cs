using System;
using System.Collections.Generic;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Debugging;

[RegisterInIl2Cpp]
public sealed class DebugWindowBehaviour : MonoBehaviour
{
    public DebugWindowBehaviour(IntPtr ptr) : base(ptr) { }

    private bool _enabled;
    private int _activeTab;
    private Rect _rect = new(20, 20, 100, 100);

    private void DrawWindow()
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        if (DebugWindow.Tabs.Count == 0) return;

        try
        {
            GUILayout.BeginHorizontal(GUIStyle.none);
        }
        catch
        {
            return;
        }

        for (int i = 0; i < DebugWindow.Tabs.Count; i++)
        {
            DebugWindow currentTab = DebugWindow.Tabs[i];
            if (!currentTab.ShouldShow())
            {
                if (_activeTab == i)
                {
                    _activeTab = (_activeTab + 1) % DebugWindow.Tabs.Count;
                }
                continue;
            }
            if (GUILayout.Toggle(_activeTab == i, currentTab.Name, new GUIStyle(GUI.skin.button))) _activeTab = i;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5f);

        if (_activeTab == 0 && !DebugWindow.Tabs[_activeTab].ShouldShow())
        {
            GUILayout.Label("Empty... just like my heart :(", GUILayout.Width(180));
        }
        else
        {
            DebugWindow.Tabs[_activeTab].BuildWindow();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) _enabled = !_enabled;
    }

    private void OnGUI()
    {
        if (!_enabled) return;
        _rect.height = 20;
        _rect.width = 100;
        _rect = GUILayout.Window(0, _rect, new Action<int>(_ => DrawWindow()), "Debug Window");

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && _rect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
        {
            Input.ResetInputAxes();
        }
    }
}
