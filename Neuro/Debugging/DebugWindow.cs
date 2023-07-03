using System;
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
    // private Vector2 positionOnScrollbar = Vector2.zero;

    private int _frameCount;
    private float _fpsUpdateTime;
    private float _fps;

    public DebugWindow(IntPtr ptr) : base(ptr)
    {
        _window = new DragWindow(new Rect(20, 20, 100, 100), "Debug (F1)", BuildWindow);
    }

    private void Update()
    {
        _frameCount++;
        _fpsUpdateTime += Time.deltaTime;
        if (_fpsUpdateTime > 0.25f)
        {
            _fps = _frameCount / _fpsUpdateTime;
            _frameCount = 0;
            _fpsUpdateTime -= 0.25f;
        }

        if (Input.GetKeyDown(KeyCode.F1)) _enabled = !_enabled;
        if (DebugTabAttribute.Tabs.Count == 0) _enabled = false;

        foreach (DebugTab tab in DebugTabAttribute.Tabs)
        {
            bool isEnabled = tab.IsEnabled;

            if (isEnabled && !tab.LastEnabled) tab.OnEnable();
            if (!isEnabled && tab.LastEnabled) tab.OnDisable();
            if (isEnabled) tab.Update();

            tab.LastEnabled = isEnabled;
        }
    }

    private void Awake()
    {
        foreach (DebugTab tab in DebugTabAttribute.Tabs)
        {
            tab.Awake();
        }
    }

    private void OnGUI()
    {
        if (!_enabled) return;
        _window.OnGUI();
    }

    private void BuildWindow()
    {
        try
        {
            using (new VerticalScope())
            {
                GUILayout.Label($"FPS: {_fps:F2}", GUILayout.Width(75));

                using (new HorizontalScope())
                {
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
                            //New tab selected, reset the scrollbar position.
                            // positionOnScrollbar = Vector2.zero;
                        }

                        if (tabHidden) GUI.enabled = true;
                    }
                }

                if (_selectedTab is {IsEnabled: true})
                {
                    GUILayoutUtils.HorizontalDivider();

                    //Create a scrollbarview for all tabs that suddenly become bigger than what fits.
                    // positionOnScrollbar = GUILayout.BeginScrollView(positionOnScrollbar, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(Screen.height/2));
                    _selectedTab?.BuildUI(); //Build UI.
                    // GUILayout.EndScrollView();
                }
            }
        }
        catch (Exception e)
        {
            Error(e);
        }
    }
}