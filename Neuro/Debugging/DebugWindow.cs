using System;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.ImGui;
using UnityEngine;

namespace Neuro.Debugging;

[RegisterInIl2Cpp]
public sealed class DebugWindow : MonoBehaviour
{
    private DebugTab _selectedTab;
    private bool _enabled = true;
    private readonly DragWindow _window;

    public DebugWindow(IntPtr ptr) : base(ptr)
    {
        _window = new DragWindow(new Rect(20, 20, 100, 100), "Debug (F1)", BuildWindow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) _enabled = !_enabled;
        if (DebugTabAttribute.Tabs.Count == 0) _enabled = false;
    }

    private void OnGUI()
    {
        if (!_enabled) return;

        _window.OnGUI();

        if (_selectedTab is { IsEnabled: true })
        {
            _selectedTab.OnGUI();
        }
    }

    private void BuildWindow()
    {
        try
        {
            GUILayout.BeginVertical();

            if (DebugTabAttribute.Tabs.Count <= 1)
            {
                GUILayout.Label(" ", GUILayout.Height(1), GUILayout.Width(75));
            }

            GUILayout.BeginHorizontal();

            foreach (DebugTab tab in DebugTabAttribute.Tabs)
            {
                bool tabHidden = !tab.IsEnabled;
                if (tabHidden)
                {
                    GUI.enabled = false;
                    if (_selectedTab == tab) _selectedTab = null;
                }

                bool isSelected = _selectedTab == tab;
                if (isSelected != GUILayout.Toggle(isSelected, tab.Name, GUI.skin.button))
                {
                    _selectedTab = !isSelected ? tab : null;
                }

                if (tabHidden) GUI.enabled = true;
            }

            GUILayout.EndHorizontal();

            if (_selectedTab is { IsEnabled: true })
            {
                GUILayoutUtils.Divider();
                _selectedTab?.BuildUI();
            }

            GUILayout.EndVertical();
        }
        catch (Exception e)
        {
            Error(e);
        }
    }
}
