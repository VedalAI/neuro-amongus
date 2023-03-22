using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Utils;

public static class Methods
{
    public static string GetLocationFromPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        PlainShipRoom closestLocation = null;
        string nearPrefix = "outside near "; // If we're not in any rooms/hallways, we're "outside"

        if (!ShipStatus.Instance) // In case this is called from the lobby
            return "the lobby";

        foreach (PlainShipRoom room in ShipStatus.Instance.AllRooms)
        {
            Collider2D collider = room.roomArea;
            if (collider.OverlapPoint(position))
            {
                if (room.RoomId == SystemTypes.Hallway)
                    nearPrefix = "a hallway near "; // keep looking for the nearest room
                else
                    return room.DisplayName(); // If we're inside a proper room, ignore the nearPrefix
            }
            else if (room.RoomId != SystemTypes.Hallway)
            {
                float distance = Vector2.Distance(position, collider.ClosestPoint(position));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = room;
                }
            }
        }

        if (!closestLocation)
            return "";

        // We're not in an actual room, so say which room we're nearest to
        return nearPrefix + closestLocation.DisplayName();
    }

    // Gets the name of a room, as displayed by the game's user interface
    public static string DisplayName (this PlainShipRoom room)
    {
        return TranslationController.Instance.GetString(room.RoomId);
    }

    public static (float min, float max) TaskTypeToTimeToCompleteTask(TaskTypes taskTypes) => MiniGameTimes.ContainsKey(taskTypes) ? MiniGameTimes[taskTypes] : new (2f,4f); 

    
    private static Dictionary<TaskTypes, (float min, float max)> MiniGameTimes = new Dictionary<TaskTypes, (float min, float max)>
    {
        {TaskTypes.AlignEngineOutput, new (10f,15f)},
        {TaskTypes.AlignTelescope, new (2f, 4f)},
        {TaskTypes.AssembleArtifact, new (2f, 4f)},
        {TaskTypes.BuyBeverage, new (2f, 4f)},
        {TaskTypes.CalibrateDistributor, new (2f, 4f)},
        {TaskTypes.ChartCourse, new (2f, 4f)},
        {TaskTypes.CleanO2Filter, new (2f, 4f)},
        {TaskTypes.CleanToilet, new (2f, 4f)},
        {TaskTypes.VentCleaning, new (2f, 4f)},
        {TaskTypes.ClearAsteroids, new (10f, 15f)},
        {TaskTypes.Decontaminate, new (2f, 4f)},
        {TaskTypes.DevelopPhotos, new (10f, 15f)},
        {TaskTypes.DivertPower, new (2f, 4f)},
        {TaskTypes.DressMannequin, new (2f, 4f)},
        {TaskTypes.EmptyChute, new (10f, 15f)},
        {TaskTypes.EmptyGarbage, new (10f, 15f)},
        {TaskTypes.EnterIdCode, new (5f, 8f)},
        {TaskTypes.FillCanisters, new (2f, 4f)},
        {TaskTypes.FixShower, new (2f, 4f)},
        {TaskTypes.FixWiring, new (5f, 8f)},
        {TaskTypes.FuelEngines, new (10f, 15f)},
        {TaskTypes.InsertKeys, new (5f, 8f)},
        {TaskTypes.InspectSample, new (10f, 15f)},
        {TaskTypes.MakeBurger, new (2f, 4f)},
        {TaskTypes.MeasureWeather, new (2f, 4f)},
        {TaskTypes.OpenWaterways, new (10f, 15f)},
        {TaskTypes.PickUpTowels, new (2f, 4f)},
        {TaskTypes.PolishRuby, new (2f, 4f)},
        {TaskTypes.PrimeShields, new (2f, 4f)},
        {TaskTypes.ProcessData, new (2f, 4f)},
        {TaskTypes.PutAwayPistols, new (2f, 4f)},
        {TaskTypes.PutAwayRifles, new (2f, 4f)},
        {TaskTypes.RebootWifi, new (10f, 15f)},
        {TaskTypes.RecordTemperature, new (2f, 4f)},
        {TaskTypes.RepairDrill, new (2f, 4f)},
        {TaskTypes.ReplaceWaterJug, new (5f, 8f)},
        {TaskTypes.ResetBreakers, new (10f, 15f)},
        {TaskTypes.RewindTapes, new (10f, 15f)},
        {TaskTypes.RunDiagnostics, new (2f, 4f)},
        {TaskTypes.ScanBoardingPass, new (5f, 8f)},
        {TaskTypes.SortRecords, new (2f, 4f)},
        {TaskTypes.SortSamples, new (2f, 4f)},
        {TaskTypes.StabilizeSteering, new (2f, 4f)},
        {TaskTypes.StartFans, new (10f, 15f)},
        {TaskTypes.StartReactor, new (10f, 15f)},
        {TaskTypes.StoreArtifacts, new (2f, 4f)},
        {TaskTypes.SubmitScan, new (10f, 15f)},
        {TaskTypes.SwipeCard, new (5f, 8f)},
        {TaskTypes.UnlockManifolds, new (2f, 4f)},
        {TaskTypes.UnlockSafe, new (10f, 15f)},
        {TaskTypes.UploadData, new (5f, 8f)},
        {TaskTypes.WaterPlants, new (10f, 15f)}
    };
}
