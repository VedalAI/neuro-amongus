using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Neuro.Caching;
using Neuro.Communication.AmongUsAI;
using UnityEngine;

namespace Neuro.Debugging.Tabs;

[DebugTab]
public sealed class GeneralDebugTab : DebugTab
{
    public override string Name => "​General";

    private bool _unityExplorerLoaded = false;

    private bool _enableWebsocket
    {
        get => CachedPlayerPrefs.GetBool(nameof(_enableWebsocket), true);
        set => CachedPlayerPrefs.SetBool(nameof(_enableWebsocket), value);
    }

    public override void BuildUI()
    {
        _enableWebsocket = GUILayout.Toggle(_enableWebsocket, "Enable Python Websocket Connection");

        if (!_unityExplorerLoaded)
        {
            string path = Path.Combine(Paths.PluginPath, "sinai-dev-UnityExplorer", "UnityExplorer.plugin");
            if (File.Exists(path) && GUILayout.Button("Load Unity Explorer"))
            {
                _unityExplorerLoaded = true;
                Assembly unityExplorerAssembly = Assembly.LoadFile(path);
                ((BasePlugin) Activator.CreateInstance(unityExplorerAssembly.GetType("UnityExplorer.ExplorerBepInPlugin")!)!).Load();
            }
        }

        if (PlayerControl.LocalPlayer)
        {
            GUILayoutUtils.GUILayoutDivider();
            if (GUILayout.Button("Murder Self")) PlayerControl.LocalPlayer.MurderPlayer(PlayerControl.LocalPlayer);
        }
    }

    public override void Update()
    {
        CommunicationHandler.Instance.enabled = _enableWebsocket;
    }
}