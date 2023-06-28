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
                        case TaskTypes.AlignEngineOutput:
                        case TaskTypes.CleanO2Filter:
                        case TaskTypes.ClearAsteroids:
                        case TaskTypes.FuelEngines: // TODO: Fake fuel maybe?
                        case TaskTypes.OpenWaterways: // TODO: Fake waterways maybe?
                        case TaskTypes.PickUpTowels:
                        case TaskTypes.PutAwayPistols:
                        case TaskTypes.PutAwayRifles:
                        case TaskTypes.ResetBreakers:
                        case TaskTypes.SortRecords:
                        case TaskTypes.StartFans:
                            normalTask.Complete();
                            break;

                        default:
                            normalTask.NextStep();
                            break;
                    }

                    MovementHandler.Instance.Wait(3f);
                }
            }
        }
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
