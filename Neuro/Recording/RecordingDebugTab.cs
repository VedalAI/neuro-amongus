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
        IndentLabel(0, "Yellow values not reset after each frame like they're supposed to!", Color.yellow);
        NeuroUtilities.GUILayoutDivider();
        BuildDeadBodiesRecorderUI();
        BuildLocalPlayerRecorderUI();
        BuildMapRecorderUI();
        BuildOtherPlayersRecorderUI();
    }

    private void BuildDeadBodiesRecorderUI()
    {
        GUILayout.Label(nameof(DeadBodiesFrame));
        IndentLabel(1, $"{nameof(DeadBodiesRecorder.Frame.DeadBodies)} ({DeadBodiesRecorder.Instance.Frame.DeadBodies.Count})");
        foreach (DeadBodyData body in DeadBodiesRecorder.Instance.Frame.DeadBodies)
        {
            IndentLabel(2, $"- ID({body.ParentId}), P{body.Position}, W({body.NearbyPlayers.Length})");
        }
    }

    private void BuildLocalPlayerRecorderUI()
    {
        GUILayout.Label(nameof(LocalPlayerRecorder));
        IndentLabel(1, $"{nameof(LocalPlayerRecorder.Frame.DidReport)}: {LocalPlayerRecorder.Instance.Frame.DidReport}", Color.yellow);
        IndentLabel(1, $"{nameof(LocalPlayerRecorder.Frame.DidVent)}: {LocalPlayerRecorder.Instance.Frame.DidVent}", Color.yellow);
        IndentLabel(1, $"{nameof(LocalPlayerRecorder.Frame.DidKill)}: {LocalPlayerRecorder.Instance.Frame.DidKill}", Color.yellow);
        IndentLabel(1, $"{nameof(LocalPlayerRecorder.Frame.SabotageUsed)}: {LocalPlayerRecorder.Instance.Frame.SabotageUsed}", Color.yellow);
        IndentLabel(1, $"{nameof(LocalPlayerRecorder.Frame.DoorsUsed)}: {LocalPlayerRecorder.Instance.Frame.DoorsUsed}", Color.yellow);
    }

    private void BuildMapRecorderUI()
    {
        GUILayout.Label(nameof(MapRecorder));
        IndentLabel(1, $"{nameof(MapRecorder.NearbyDoors)} ({MapRecorder.Instance.NearbyDoors.Count})");
        foreach (DoorData door in MapRecorder.Instance.NearbyDoors)
        {
            IndentLabel(2, $"- D({door.Position.TotalDistance:F2}), O({door.IsOpen})");
        }
        IndentLabel(1, $"{nameof(MapRecorder.NearbyVents)} ({MapRecorder.Instance.NearbyVents.Count})");
        foreach (VentData vent in MapRecorder.Instance.NearbyVents)
        {
            IndentLabel(2, $"- D({vent.Position.TotalDistance:F2})");
        }
    }

    private void BuildOtherPlayersRecorderUI()
    {
        GUILayout.Label(nameof(OtherPlayersRecorder));
        IndentLabel(1, $"{nameof(OtherPlayersRecorder.LastSeen)} ({OtherPlayersRecorder.Instance.LastSeen.Count})", Color.red);
        foreach (OtherPlayerData player in OtherPlayersRecorder.Instance.LastSeen.Values.OrderBy(d => d.Id))
        {
            IndentLabel(2, $"- ID({player.Id}), P{player.LastSeenPosition}, T({player.RoundTimeVisible:F2})", Color.red);
        }
        IndentLabel(0, "This doesn't work properly, please fix! (Visibility.IsVisible)", Color.red);
    }

    private void IndentLabel(int indent, string label) => IndentLabel(indent, label, GUI.contentColor);

    private void IndentLabel(int indent, string label, Color color)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20 * indent);
        Color originalColor = GUI.contentColor;
        GUI.contentColor = color;
        GUILayout.Label(label);
        GUI.contentColor = originalColor;
        GUILayout.EndHorizontal();
    }
}
