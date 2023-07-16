using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Neuro.Caching;
using Neuro.Communication.AmongUsAI;
using Neuro.Recording.Tasks;
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

        if (!PlayerControl.LocalPlayer) return;
        GUILayoutUtils.HorizontalDivider();

        if (GUILayout.Button("Murder Self")) PlayerControl.LocalPlayer.MurderPlayer(PlayerControl.LocalPlayer);

        if (_enableWebsocket && CommunicationHandler.Instance.IsConnected && MovementSuggestion.Instance)
        {
            if (GUILayout.Button("Suggest Meeting Button")) MovementSuggestion.Instance.SuggestTarget(ShipStatus.Instance.GetComponentsInChildren<SystemConsole>().First(c => c.MinigamePrefab.name.Contains("Emergency")));
            if (GUILayout.Button("Clear Suggestion")) MovementSuggestion.Instance.ClearSuggestion();
        }
    }

    public override void Update()
    {
        CommunicationHandler.Instance.enabled = _enableWebsocket;
    }
}
