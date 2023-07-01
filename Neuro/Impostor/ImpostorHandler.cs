using UnityEngine;
using Reactor.Utilities.Attributes;
using System;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Utilities;
using Neuro.Events;
using Neuro.Cursor;
using Neuro.Movement;

namespace Neuro.Impostor;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class ImpostorHandler : MonoBehaviour
{
    public ImpostorHandler(IntPtr ptr) : base(ptr)
    {
    }

    public static ImpostorHandler Instance { get; private set; }

    private static Vent previousVent = null;

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
        if (Minigame.Instance) return;

        foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
        {
            if (task == null || task.Locations == null || task.IsComplete || !task.MinigamePrefab) continue;
            if (PlayerTask.TaskIsEmergency(task)) continue; // Don't fake sabotages

            foreach (Console console in task.FindConsoles())
            {
                if (Vector2.Distance(console.transform.position, PlayerControl.LocalPlayer.GetTruePosition()) < console.UsableDistance)
                {
                    NormalPlayerTask normalTask = task.Cast<NormalPlayerTask>();

                    switch (task.TaskType)
                    {
                        // These tasks can't be faked properly as impostor using the current system, so instead we pretend they are done after the first step
                        case TaskTypes.AlignEngineOutput: // Only fakes one engine
                        case TaskTypes.CleanO2Filter:
                        case TaskTypes.ClearAsteroids:
                        case TaskTypes.FuelEngines: // Only fakes grabbing the gas can one time
                        case TaskTypes.OpenWaterways: // Only fakes one of the wheels
                        case TaskTypes.PickUpTowels: // Essentially ignored
                        case TaskTypes.ResetBreakers: // Only fakes one of the breakers
                        case TaskTypes.SortRecords: // Only fakes the first step
                            normalTask.Complete();
                            break;

                        default:
                            normalTask.NextStep(); // Timer tasks won't be faked properly
                            break;
                    }

                    MovementHandler.Instance.Wait(GetWaitTime(normalTask));
                }
            }
        }
    }

    private static float GetWaitTime(NormalPlayerTask task)
    {
        return task.TaskType switch
        {
            TaskTypes.SubmitScan => 0, // Don't fake medscan
            TaskTypes.PrimeShields => 2,
            TaskTypes.FuelEngines => 4.1f,
            TaskTypes.ChartCourse => 3.2f,
            TaskTypes.StartReactor => 14,
            TaskTypes.SwipeCard => 3.9f,
            TaskTypes.ClearAsteroids => 13,
            TaskTypes.UploadData => 8f,
            TaskTypes.InspectSample => 2,
            TaskTypes.EmptyChute => 3.2f,
            TaskTypes.EmptyGarbage => 3.2f,
            TaskTypes.AlignEngineOutput => 1.9f,
            TaskTypes.FixWiring => 3.5f,
            TaskTypes.CalibrateDistributor => 5,
            TaskTypes.DivertPower => 1.9f,
            TaskTypes.UnlockManifolds => 4.2f,
            TaskTypes.CleanO2Filter => 5.5f,
            TaskTypes.StabilizeSteering => 2.4f,
            TaskTypes.AssembleArtifact => 5,
            TaskTypes.SortSamples => 5,
            TaskTypes.MeasureWeather => 5,
            TaskTypes.EnterIdCode => 5,
            TaskTypes.BuyBeverage => 4.8f,
            TaskTypes.ProcessData => 10,
            TaskTypes.RunDiagnostics => 1.8f,
            TaskTypes.WaterPlants => 2,
            TaskTypes.MonitorOxygen => 3.5f,
            TaskTypes.StoreArtifacts => 4.4f,
            TaskTypes.FillCanisters => 8,
            TaskTypes.ActivateWeatherNodes when task.TaskStep == 0 => 5.5f,
            TaskTypes.ActivateWeatherNodes => 1.5f,
            TaskTypes.InsertKeys => 2.5f,
            TaskTypes.ScanBoardingPass => 4,
            TaskTypes.OpenWaterways => 7.5f,
            TaskTypes.ReplaceWaterJug => 6,
            TaskTypes.RepairDrill => 3.4f,
            TaskTypes.AlignTelescope => 5,
            TaskTypes.RecordTemperature => 6,
            TaskTypes.RebootWifi when task.TimerStarted == NormalPlayerTask.TimerState.NotStarted => 1.5f,
            TaskTypes.RebootWifi => 4.7f,
            TaskTypes.PolishRuby => 5,
            TaskTypes.ResetBreakers => 1.4f,
            TaskTypes.Decontaminate => 7.5f,
            TaskTypes.MakeBurger => 6.5f,
            TaskTypes.UnlockSafe => 10,
            TaskTypes.SortRecords => 1.6f,
            TaskTypes.PutAwayPistols when task.TaskStep == 0 => 0.5f,
            TaskTypes.PutAwayPistols => 3,
            TaskTypes.FixShower => 4,
            TaskTypes.CleanToilet => 6,
            TaskTypes.DressMannequin => 4.5f,
            TaskTypes.PickUpTowels => 0, // Don't fake towels (actual timing of last step is 14s)
            TaskTypes.RewindTapes => 13f,
            TaskTypes.StartFans => 3.5f,
            TaskTypes.DevelopPhotos => 3.5f,
            TaskTypes.PutAwayRifles when task.TaskStep == 0 => 0.5f,
            TaskTypes.PutAwayRifles => 2.5f,
            TaskTypes.VentCleaning => 3.3f,
            _ => throw new ArgumentOutOfRangeException(nameof(task))
        };
    }

    [HideFromIl2Cpp]
    public IEnumerator CoStartVentOut()
    {
        // TODO: make sure this doesnt get stuck in an infinite loop

        previousVent = Vent.currentVent;
        while (true)
        {
            yield return CoTryMoveToVent();

            bool spottedCrewmateOrBody = false;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer) continue;
                if (player.Data.IsDead) continue;
                if (player.Data.Role.IsImpostor) continue;

                if (Visibility.IsVisible(player.GetTruePosition()))
                {
                    Info("Spotted a crewmate, trying another vent");
                    spottedCrewmateOrBody = true;
                    break;
                }
            }

            if (spottedCrewmateOrBody) continue;

            foreach (DeadBody body in ComponentCache<DeadBody>.Cached)
            {
                if (Visibility.IsVisible(body.TruePosition))
                {
                    Info("Spotted a body, trying another vent");
                    spottedCrewmateOrBody = true;
                    break;
                }
            }

            if (spottedCrewmateOrBody) continue;

            break;
        }

        HudManager.Instance.ImpostorVentButton.DoClick();
        InGameCursor.Instance.Hide();
        previousVent = null;
    }

    [HideFromIl2Cpp]
    private IEnumerator CoTryMoveToVent()
    {
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.5f, 0.8f));

        int targetButtonIndex = GetNextVent(Vent.currentVent);
        if (targetButtonIndex == -1)
        {
            Info($"No available vents, skipping vent move.");
            yield break;
        }

        previousVent = Vent.currentVent;
        yield return InGameCursor.Instance.CoMoveTo(Vent.currentVent.Buttons[targetButtonIndex]);
        yield return InGameCursor.Instance.CoPressLMB();
        yield break;
    }

    [HideFromIl2Cpp]
    // since some vents can have a variable amount of neighbors, use this helper method to get available ones
    private static int GetNextVent(Vent current)
    {
        Vent[] nearby = current.NearbyVents;
        VentilationSystem system = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();
        for (int i = 0; i < nearby.Length; i++)
        {
            Vent vent = nearby[i];
            if (!vent ||
                (vent == previousVent && nearby.Length > 1) ||
                system.IsVentCurrentlyBeingCleaned(vent.Id) ||
                system.IsImpostorInsideVent(vent.Id)) continue;
            return i;
        }

        return -1;
    }

    [EventHandler(EventTypes.MeetingStarted)]
    private static void HideCursor()
    {
        InGameCursor.Instance.Hide();
    }
}
