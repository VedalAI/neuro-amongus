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

    public static Vector2 TaskTypeToTimeToCompleteTask(TaskTypes taskTypes) => MiniGameTimes.ContainsKey(taskTypes) ? MiniGameTimes[taskTypes] : new Vector2(2f, 4f);

    private static Dictionary<TaskTypes, Vector2> MiniGameTimes = new Dictionary<TaskTypes, Vector2>
    {
        {TaskTypes.AlignEngineOutput, new Vector2(10f, 15f)},
        {TaskTypes.AlignTelescope, new Vector2(2f, 4f)},
        {TaskTypes.AssembleArtifact, new Vector2(2f, 4f)},
        {TaskTypes.BuyBeverage, new Vector2(2f, 4f)},
        {TaskTypes.CalibrateDistributor, new Vector2(2f, 4f)},
        {TaskTypes.ChartCourse, new Vector2(2f, 4f)},
        {TaskTypes.CleanO2Filter, new Vector2(2f, 4f)},
        {TaskTypes.CleanToilet, new Vector2(2f, 4f)},
        {TaskTypes.VentCleaning, new Vector2(2f, 4f)},
        {TaskTypes.ClearAsteroids, new Vector2(10f, 15f)},
        {TaskTypes.Decontaminate, new Vector2(2f, 4f)},
        {TaskTypes.DevelopPhotos, new Vector2(10f, 15f)},
        {TaskTypes.DivertPower, new Vector2(2f, 4f)},
        {TaskTypes.DressMannequin, new Vector2(2f, 4f)},
        {TaskTypes.EmptyChute, new Vector2(10f, 15f)},
        {TaskTypes.EmptyGarbage, new Vector2(10f, 15f)},
        {TaskTypes.EnterIdCode, new Vector2(5f, 8f)},
        {TaskTypes.FillCanisters, new Vector2(2f, 4f)},
        {TaskTypes.FixShower, new Vector2(2f, 4f)},
        {TaskTypes.FixWiring, new Vector2(5f, 8f)},
        {TaskTypes.FuelEngines, new Vector2(10f, 15f)},
        {TaskTypes.InsertKeys, new Vector2(5f, 8f)},
        {TaskTypes.InspectSample, new Vector2(10f, 15f)},
        {TaskTypes.MakeBurger, new Vector2(2f, 4f)},
        {TaskTypes.MeasureWeather, new Vector2(2f, 4f)},
        {TaskTypes.OpenWaterways, new Vector2(10f, 15f)},
        {TaskTypes.PickUpTowels, new Vector2(2f, 4f)},
        {TaskTypes.PolishRuby, new Vector2(2f, 4f)},
        {TaskTypes.PrimeShields, new Vector2(2f, 4f)},
        {TaskTypes.ProcessData, new Vector2(2f, 4f)},
        {TaskTypes.PutAwayPistols, new Vector2(2f, 4f)},
        {TaskTypes.PutAwayRifles, new Vector2(2f, 4f)},
        {TaskTypes.RebootWifi, new Vector2(10f, 15f)},
        {TaskTypes.RecordTemperature, new Vector2(2f, 4f)},
        {TaskTypes.RepairDrill, new Vector2(2f, 4f)},
        {TaskTypes.ReplaceWaterJug, new Vector2(5f, 8f)},
        {TaskTypes.ResetBreakers, new Vector2(10f, 15f)},
        {TaskTypes.RewindTapes, new Vector2(10f, 15f)},
        {TaskTypes.RunDiagnostics, new Vector2(2f, 4f)},
        {TaskTypes.ScanBoardingPass, new Vector2(5f, 8f)},
        {TaskTypes.SortRecords, new Vector2(2f, 4f)},
        {TaskTypes.SortSamples, new Vector2(2f, 4f)},
        {TaskTypes.StabilizeSteering, new Vector2(2f, 4f)},
        {TaskTypes.StartFans, new Vector2(10f, 15f)},
        {TaskTypes.StartReactor, new Vector2(10f, 15f)},
        {TaskTypes.StoreArtifacts, new Vector2(2f, 4f)},
        {TaskTypes.SubmitScan, new Vector2(10f, 15f)},
        {TaskTypes.SwipeCard, new Vector2(5f, 8f)},
        {TaskTypes.UnlockManifolds, new Vector2(2f, 4f)},
        {TaskTypes.UnlockSafe, new Vector2(10f, 15f)},
        {TaskTypes.UploadData, new Vector2(5f, 8f)},
        {TaskTypes.WaterPlants, new Vector2(10f, 15f)}
    };
}
