using System.Linq;
using Neuro.Debugging;
using Neuro.Recording.DeadBodies;
using Neuro.Recording.LocalPlayer;
using Neuro.Recording.Map;
using Neuro.Recording.OtherPlayers;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Recording;

[DebugTab]
public sealed class RecordingDebugTab : DebugTab
{
    public override string Name => "Recording";

    public override bool IsEnabled => Recorder.Instance;

    public override void BuildUI()
    {
        Label(0, "Yellow values not reset after each frame like they're supposed to!", Color.yellow);
        NeuroUtilities.GUILayoutDivider();
        BuildDeadBodiesRecorderUI();
        BuildLocalPlayerRecorderUI();
        BuildMapRecorderUI();
        BuildOtherPlayersRecorderUI();
    }

    private void BuildDeadBodiesRecorderUI()
    {
        GUILayout.Label(nameof(DeadBodiesFrame));
        Label(1, $"{nameof(DeadBodiesFrame.DeadBodies)} ({DeadBodiesRecorder.Instance.Frame.DeadBodies.Count})");
        foreach (DeadBodyData body in DeadBodiesRecorder.Instance.Frame.DeadBodies)
        {
            Label(2, $"- ID({body.ParentId}), P{body.Position}, W({body.NearbyPlayers.Length})");
        }
    }

    private void BuildLocalPlayerRecorderUI()
    {
        GUILayout.Label(nameof(LocalPlayerFrame));
        Label(1, $"{nameof(LocalPlayerFrame.DidReport)}: {LocalPlayerRecorder.Instance.Frame.DidReport}", Color.yellow);
        Label(1, $"{nameof(LocalPlayerFrame.DidVent)}: {LocalPlayerRecorder.Instance.Frame.DidVent}", Color.yellow);
        Label(1, $"{nameof(LocalPlayerFrame.DidKill)}: {LocalPlayerRecorder.Instance.Frame.DidKill}", Color.yellow);
        Label(1, $"{nameof(LocalPlayerFrame.SabotageUsed)}: {LocalPlayerRecorder.Instance.Frame.SabotageUsed}", Color.yellow);
        Label(1, $"{nameof(LocalPlayerFrame.DoorsUsed)}: {LocalPlayerRecorder.Instance.Frame.DoorsUsed}", Color.yellow);
    }

    private void BuildMapRecorderUI()
    {
        GUILayout.Label(nameof(MapFrame));
        Label(1, $"{nameof(MapFrame.NearbyDoors)} ({MapRecorder.Instance.Frame.NearbyDoors.Count})");
        foreach (DoorData door in MapRecorder.Instance.Frame.NearbyDoors)
        {
            Label(2, $"- D({door.Position.TotalDistance:F2}), O({door.IsOpen})");
        }

        Label(1, $"{nameof(MapFrame.NearbyVents)} ({MapRecorder.Instance.Frame.NearbyVents.Count})");
        foreach (VentData vent in MapRecorder.Instance.Frame.NearbyVents)
        {
            Label(2, $"- D({vent.Position.TotalDistance:F2})");
        }
    }

    private void BuildOtherPlayersRecorderUI()
    {
        GUILayout.Label(nameof(OtherPlayersFrame));
        Label(1, $"{nameof(OtherPlayersFrame.LastSeenPlayers)} ({OtherPlayersRecorder.Instance.Frame.LastSeenPlayers.Count})");
        foreach (OtherPlayerData player in OtherPlayersRecorder.Instance.Frame.LastSeenPlayers.OrderBy(d => d.Id))
        {
            Label(2, $"- ID({player.Id}), P{player.LastSeenPosition}, T({player.RoundTimeVisible:F2})");
        }
    }

    private void Label(int indent, string label) => Label(indent, label, GUI.contentColor);

    private void Label(int indent, string label, Color color)
    {
        using (new HorizontalScope())
        {
            GUILayout.Space(20 * indent);
            Color originalColor = GUI.contentColor;
            GUI.contentColor = color;
            GUILayout.Label(label);
            GUI.contentColor = originalColor;
        }
    }
}
