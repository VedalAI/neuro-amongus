using System.Collections;
using System.Collections.Generic;
using Neuro.DependencyInjection;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames;

public class MinigamesHandler : IMinigamesHandler
{
    public IContextProvider Context { get; set; }

    public IEnumerator CompleteMinigame(PlayerTask task, Minigame minigame)
    {
        yield return new WaitForSeconds(GetTimeToComplete(task.TaskType));

        //task.Complete();
        if (task.TryCast<NormalPlayerTask>() is { } normalPlayerTask)
        {
            normalPlayerTask.NextStep();
        }
        else
        {
            Debug.Log("Not Normal Player Task");
            task.Complete();
        }

        minigame.Close();

        GetPathToNextTask(task);
    }

    private static void GetPathToNextTask(PlayerTask lastTask)
    {
        // TODO: This method should be somewhere else
        PlayerTask nextTask = null;
        if (lastTask.IsComplete)
        {
            Debug.Log("Task is complete");
            foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
            {
                if (!t.IsComplete && t.HasLocation)
                {
                    nextTask = t;
                    Debug.Log(nextTask.name);
                    break;
                }
            }
        }
        else
        {
            nextTask = lastTask;
        }

        if (nextTask != null)
        {
            Debug.Log("Next task isn't null");
            NeuroPlugin.Instance.MainContext.TasksHandler.CurrentPath = NeuroPlugin.Instance.MainContext.PathfindingHandler.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations.At(0));
            NeuroPlugin.Instance.MainContext.TasksHandler.PathIndex = 0;
        }
    }

    private static float GetTimeToComplete(TaskTypes task)
    {
        (float min, float max) = GetMinMaxTimeToComplete(task);
        return Random.RandomRange(min, max);
    }

    private static (float min, float max) GetMinMaxTimeToComplete(TaskTypes taskTypes) => MiniGameTimes.GetValueOrDefault(taskTypes, (2f, 4f));

    private static readonly Dictionary<TaskTypes, (float min, float max)> MiniGameTimes = new Dictionary<TaskTypes, (float min, float max)>
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
