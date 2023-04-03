using System;
using System.Collections.Generic;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Debugging;

[RegisterInIl2Cpp]
public sealed class DebugWindowBehaviour : MonoBehaviour
{
    private struct Tab
    {
        public string Name { get; }
        public Action BuildTabAction { get; }
        public Func<bool> ShouldShow { get; }

        public Tab(string name, Action buildTabAction, Func<bool> shouldShow)
        {
            Name = name;
            BuildTabAction = buildTabAction;
            ShouldShow = shouldShow;
        }

        public Tab(string name, Action buildTabAction) : this(name, buildTabAction, () => true)
        {
        }
    }

    public DebugWindowBehaviour(IntPtr ptr) : base(ptr) { }

    private bool _enabled;
    private readonly List<Tab> _tabs = new();
    private int _activeTab;
    private Rect _rect = new(20, 20, 100, 100);

    public void RegisterTab(string tabName, Action buildTabAction)
    {
        _tabs.Add(new Tab(tabName, buildTabAction));
    }

    public void RegisterTab(string tabName, Action buildTabAction, Func<bool> shouldShow)
    {
        _tabs.Add(new Tab(tabName, buildTabAction, shouldShow));
    }

    private void DrawWindow()
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        if (_tabs.Count == 0) return;

        try
        {
            GUILayout.BeginHorizontal(GUIStyle.none);
        }
        catch
        {
            return;
        }

        for (int i = 0; i < _tabs.Count; i++)
        {
            Tab currentTab = _tabs[i];
            if (!currentTab.ShouldShow.Invoke())
            {
                if (_activeTab == i)
                {
                    _activeTab = (_activeTab + 1) % _tabs.Count;
                }
                continue;
            }
            if (GUILayout.Toggle(_activeTab == i, currentTab.Name, new GUIStyle(GUI.skin.button))) _activeTab = i;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5f);
        _tabs[_activeTab].BuildTabAction?.Invoke();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) _enabled = !_enabled;
    }

    private void OnGUI()
    {
        if (!_enabled) return;
        _rect.height = _rect.width = 20;
        _rect = GUILayout.Window(0, _rect, new Action<int>(_ => DrawWindow()), "Debug Window");

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && _rect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
        {
            Input.ResetInputAxes();
        }
    }
}
