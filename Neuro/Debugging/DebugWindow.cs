using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.ImGui;
using UnityEngine;

namespace Neuro.Debugging;

[RegisterInIl2Cpp]
public sealed class DebugWindow : MonoBehaviour
{
    static DebugWindow()
    {
        _tabs = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<DebugTabAttribute>() is { })
            .Where(t => t.IsAssignableTo(typeof(DebugTab)))
            .Select(Activator.CreateInstance)
            .OfType<DebugTab>()
            .ToList();
    }

    private static readonly List<DebugTab> _tabs;

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
        if (_tabs.Count == 0) _enabled = false;
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

            GUILayout.BeginHorizontal();

            foreach (DebugTab tab in _tabs)
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
                GUILayout.Label(string.Empty, GUI.skin.horizontalSlider); // This creates a divider
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
