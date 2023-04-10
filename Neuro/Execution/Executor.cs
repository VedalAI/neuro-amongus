using Reactor.Utilities.Attributes;
using UnityEngine;
using Neuro.Recording.DataStructures;

namespace Neuro.Execution;

[RegisterInIl2Cpp]
public sealed class Executor : MonoBehaviour
{
    public float mapStayDurationSeconds = 1f;

    public Vector2 movementDirection;

    public void ProcessFrame(Frame frame)
    {
        movementDirection = new Vector2(frame.Direction.x, frame.Direction.y);

        if (frame.Report)
        {
            HudManager.Instance.ReportButton.DoClick();
        }

        if (frame.Kill)
        {
            HudManager.Instance.KillButton.DoClick();
        }

        if (frame.Sabotage)
        {
            Sabotage();
        }

        if (frame.Doors)
        {
            SabotageDoors();
        }

        if (frame.Vent)
        {
            HudManager.Instance.ImpostorVentButton.DoClick();
        }
    }


    private void Sabotage()
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            return;
        }

        OpenSabotageMap();
        Invoke("StartSabotage", mapStayDurationSeconds);
        Invoke("CloseMap", mapStayDurationSeconds * 3);
    }


    private void SabotageDoors()
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            return;
        }

        OpenSabotageMap();
        Invoke("StartDoorsSabotage", mapStayDurationSeconds);
        Invoke("CloseMap", mapStayDurationSeconds * 3);
    }


    private void OpenSabotageMap()
    {
        MapOptions mapOptions = new MapOptions();
        mapOptions.Mode = MapOptions.Modes.Sabotage;
        HudManager.Instance.ToggleMapVisible(mapOptions);
    }


    private void StartSabotage()
    {
        MapRoom mapRoom = GameObject.FindObjectOfType<MapRoom>();
        if (mapRoom)
        {
            // TODO: randomly select sabotage or let AI choose
            mapRoom.SabotageOxygen();
        }
    }


    private void StartDoorsSabotage()
    {
        MapRoom[] mapRooms = GameObject.FindObjectsOfType<MapRoom>();
        foreach (MapRoom mapRoom in mapRooms)
        {
            // TODO: maybe let AI decide where to sabotage doors
            mapRoom.SabotageDoors();
        }
    }


    private void CloseMap()
    {
        MapOptions mapOptions = new MapOptions();
        mapOptions.Mode = MapOptions.Modes.None;
        HudManager.Instance.ToggleMapVisible(mapOptions);
    }
}