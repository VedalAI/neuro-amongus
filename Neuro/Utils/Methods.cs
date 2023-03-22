using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Utils;

public static class Methods
{
    private static Dictionary<Vector2, string> locationNames = new Dictionary<Vector2, string>()
    {
        { new Vector2 { x = 0, y = 0 }, "Cafeteria" },
        { new Vector2 { x = 9, y = 1 }, "Weapons" },
        { new Vector2 { x = 6.6f, y = -3.7f }, "O2" },
        { new Vector2 { x = 17, y = -5 }, "Navigation" },
        { new Vector2 { x = 9, y = -12.5f }, "Shields" },
        { new Vector2 { x = 4, y = 15.6f }, "Communications" },
        { new Vector2 { x = 0, y = -17 }, "Trash Chute" },
        { new Vector2 { x = 0, y = -12 }, "Storage" },
        { new Vector2 { x = -9, y = -10f }, "Electrical" },
        { new Vector2 { x = -15.6f, y = -10.6f }, "Lower Engine" },
        { new Vector2 { x = -13f, y = -4.4f }, "Security" },
        { new Vector2 { x = -21f, y = -5.5f }, "Reactor" },
        { new Vector2 { x = -15.4f, y = 1 }, "Upper Engine" },
        { new Vector2 { x = -8f, y = -3.5f }, "MedBay" },
        { new Vector2 { x = 5f, y = -8 }, "Admin" },
    };

    public static string GetLocationFromPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        string closestLocation = "";

        foreach (KeyValuePair<Vector2, string> keyValuePair in locationNames)
        {
            float distance = Vector2.Distance(keyValuePair.Key, position);
            if (distance < 2f)
            {
                return keyValuePair.Value;
            }
            else
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = keyValuePair.Value;
                }
            }
        }

        return closestLocation;
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
