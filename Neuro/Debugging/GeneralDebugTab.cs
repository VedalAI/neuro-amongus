using Neuro.Communication.AmongUsAI;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Debugging;

[DebugTab]
public sealed class GeneralDebugTab : DebugTab
{
    public override string Name => "​General";

    private bool _enableWebsocket
    {
        get => CachedPlayerPrefs.GetBool(nameof(_enableWebsocket), true);
        set => CachedPlayerPrefs.SetBool(nameof(_enableWebsocket), value);
    }

    public override void BuildUI()
    {
        _enableWebsocket = GUILayout.Toggle(_enableWebsocket, "Enable Python Websocket Connection");

        if (PlayerControl.LocalPlayer)
        {
            NeuroUtilities.GUILayoutDivider();
            if (GUILayout.Button("Murder Self")) PlayerControl.LocalPlayer.MurderPlayer(PlayerControl.LocalPlayer);
        }
    }

    public override void Update()
    {
        CommunicationHandler.Instance.enabled = _enableWebsocket;
    }
}
