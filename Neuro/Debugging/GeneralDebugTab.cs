using Neuro.Communication.AmongUsAI;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Debugging;

[DebugTab]
public sealed class GeneralDebugTab : DebugTab
{
    public override string Name => "​General";

    private const string ENABLE_WEBSOCKET = "EnablePythonWebsocket";
    private bool? _enableWebsocket;
    private bool enableWebsocket
    {
        get => _enableWebsocket ?? (_enableWebsocket = PlayerPrefs.GetInt(ENABLE_WEBSOCKET, 1) == 1).Value;
        set
        {
            if (_enableWebsocket == value) return;
            PlayerPrefs.SetInt(ENABLE_WEBSOCKET, (_enableWebsocket = value).Value ? 1 : 0);
        }
    }

    public override void BuildUI()
    {
        GUILayout.Label("Configuration (saved locally)");
        enableWebsocket = GUILayout.Toggle(enableWebsocket, "Enable Python Websocket Connection");

        if (PlayerControl.LocalPlayer)
        {
            NeuroUtilities.GUILayoutDivider();
            if (GUILayout.Button("Murder Self")) PlayerControl.LocalPlayer.MurderPlayer(PlayerControl.LocalPlayer);
        }
    }

    public override void Update()
    {
        CommunicationHandler.Instance.enabled = enableWebsocket;
    }
}
