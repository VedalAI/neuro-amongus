using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neuro.Minigames;

public class MinigamesHandler
{
    public IEnumerator CompleteMinigame(PlayerTask task, Minigame minigame)
    {
        yield return new WaitForSeconds(GetTimeToComplete(task.TaskType));

        //task.Complete();
        if (task.TryCast<NormalPlayerTask>() is { } normalPlayerTask)
        {
            normalPlayerTask.NextStep();
            Info($"Task {normalPlayerTask} is at step {normalPlayerTask.TaskStep}/{normalPlayerTask.MaxStep}");

            // If NextStep() doesn't create an arrow, then this task does not require moving
            // to a different location and should be completed.
            if (normalPlayerTask.Arrow == null)
            {
                normalPlayerTask.Complete();
            }
        }
        else
        {
            Warning("Not Normal Player Task");
            task.Complete();
        }

        minigame.Close();

        NeuroPlugin.Instance.Tasks.UpdatePathToTask(task);
    }

    private static float GetTimeToComplete(TaskTypes task)
    {
        (float min, float max) = GetMinMaxTimeToComplete(task);
        return Random.RandomRange(min, max);
    }

    private static (float min, float max) GetMinMaxTimeToComplete(TaskTypes taskTypes) => MiniGameTimes.GetValueOrDefault(taskTypes, (2f, 4f));

    private static readonly Dictionary<TaskTypes, (float min, float max)> MiniGameTimes = new()
    {
        {TaskTypes.AlignEngineOutput, (10f, 15f)},
        {TaskTypes.AlignTelescope, (2f, 4f)},
        {TaskTypes.AssembleArtifact, (2f, 4f)},
        {TaskTypes.BuyBeverage, (2f, 4f)},
        {TaskTypes.CalibrateDistributor, (2f, 4f)},
        {TaskTypes.ChartCourse, (2f, 4f)},
        {TaskTypes.CleanO2Filter, (2f, 4f)},
        {TaskTypes.CleanToilet, (2f, 4f)},
        {TaskTypes.VentCleaning, (2f, 4f)},
        {TaskTypes.ClearAsteroids, (10f, 15f)},
        {TaskTypes.Decontaminate, (2f, 4f)},
        {TaskTypes.DevelopPhotos, (10f, 15f)},
        {TaskTypes.DivertPower, (2f, 4f)},
        {TaskTypes.DressMannequin, (2f, 4f)},
        {TaskTypes.EmptyChute, (10f, 15f)},
        {TaskTypes.EmptyGarbage, (10f, 15f)},
        {TaskTypes.EnterIdCode, (5f, 8f)},
        {TaskTypes.FillCanisters, (2f, 4f)},
        {TaskTypes.FixShower, (2f, 4f)},
        {TaskTypes.FixWiring, (5f, 8f)},
        {TaskTypes.FuelEngines, (10f, 15f)},
        {TaskTypes.InsertKeys, (5f, 8f)},
        {TaskTypes.InspectSample, (10f, 15f)},
        {TaskTypes.MakeBurger, (2f, 4f)},
        {TaskTypes.MeasureWeather, (2f, 4f)},
        {TaskTypes.OpenWaterways, (10f, 15f)},
        {TaskTypes.PickUpTowels, (2f, 4f)},
        {TaskTypes.PolishRuby, (2f, 4f)},
        {TaskTypes.PrimeShields, (2f, 4f)},
        {TaskTypes.ProcessData, (2f, 4f)},
        {TaskTypes.PutAwayPistols, (2f, 4f)},
        {TaskTypes.PutAwayRifles, (2f, 4f)},
        {TaskTypes.RebootWifi, (10f, 15f)},
        {TaskTypes.RecordTemperature, (2f, 4f)},
        {TaskTypes.RepairDrill, (2f, 4f)},
        {TaskTypes.ReplaceWaterJug, (5f, 8f)},
        {TaskTypes.ResetBreakers, (10f, 15f)},
        {TaskTypes.RewindTapes, (10f, 15f)},
        {TaskTypes.RunDiagnostics, (2f, 4f)},
        {TaskTypes.ScanBoardingPass, (5f, 8f)},
        {TaskTypes.SortRecords, (2f, 4f)},
        {TaskTypes.SortSamples, (2f, 4f)},
        {TaskTypes.StabilizeSteering, (2f, 4f)},
        {TaskTypes.StartFans, (10f, 15f)},
        {TaskTypes.StartReactor, (10f, 15f)},
        {TaskTypes.StoreArtifacts, (2f, 4f)},
        {TaskTypes.SubmitScan, (10f, 15f)},
        {TaskTypes.SwipeCard, (5f, 8f)},
        {TaskTypes.UnlockManifolds, (2f, 4f)},
        {TaskTypes.UnlockSafe, (10f, 15f)},
        {TaskTypes.UploadData, (5f, 8f)},
        {TaskTypes.WaterPlants, (10f, 15f)}
    };
}
