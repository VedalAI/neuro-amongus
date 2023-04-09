using System;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

[RegisterInIl2Cpp]
public sealed class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    public Recorder(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
        // EventManager.RegisterHandler(this);
    }

    private int _fixedUpdateCalls = 0;

    private void FixedUpdate()
    {
        // TODO: We should record meeting data!
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        // TODO: Record map id
        // TODO: Record all of the tasks
        // TODO: Record 11th task as emergency
        // TODO: Record fellow impostors
        // TODO: Record localplayer velocity
        // TODO: Raycast for obstacles

        _fixedUpdateCalls++;
        if (_fixedUpdateCalls < 9) return;
        _fixedUpdateCalls = 0;

        /* TODO: Serialize data for writing to file
        BinaryWriter writer = new(new MemoryStream());

        DeadBodiesRecorder.Instance.Serialize(writer);
        ImpostorRecorder.Instance.Serialize(writer);
        LocalPlayerRecorder.Instance.Serialize(writer);
        MapRecorder.Instance.Serialize(writer);
        OtherPlayersRecorder.Instance.Serialize(writer);

        writer.Close();
        */
    }

    /*[EventHandler(EventTypes.MeetingStarted)]
    public void WriteData()
    {
        // If uncommenting this also uncomment EventManager.RegisterHandler(this); in Awake
        string frameString = JsonSerializer.Serialize(Frames);
        File.WriteAllText(Path.Combine(BepInEx.Paths.PluginPath, "output.json"), frameString);
        Info(Path.Combine(BepInEx.Paths.PluginPath, "output.json"));
    }*/

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<Recorder>();
    }
}
