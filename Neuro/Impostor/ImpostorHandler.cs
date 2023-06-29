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
            if (PlayerTask.TaskIsEmergency(task)) continue;

            foreach (Console console in task.FindConsoles())
            {
                if (Vector2.Distance(console.transform.position, PlayerControl.LocalPlayer.GetTruePosition()) < 0.8f)
                {
                    // Minigame minigame = GameObject.Instantiate<Minigame>(task.GetMinigamePrefab());
                    // minigame.Console = console;
                    // minigame.Begin(task);

                    // TODO: we can't just do task.NextStep because whoever coded minigames did a very bad job and it would be broken
                    // unless we can just ignore the tasks we don't want to fake

                    // i cant be arsed to explain
                    // just trust me when i say its not working
                    // https://cdn.7tv.app/emote/62af89d111218d43c4aae647/4x.webp

                    // Set the task as complete
                    task.Cast<NormalPlayerTask>().NextStep();

                    // Stand still for a bit
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
            top: ; // this code had a severe lack of gotos

            yield return CoTryMoveToVent();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer) continue;
                if (player.Data.IsDead) continue;
                if (player.Data.Role.IsImpostor) continue;

                if (Visibility.IsVisible(player.GetTruePosition()))
                {
                    Info("Spotted a crewmate, trying another vent");
                    goto top; //LMAO
                }
            }

            foreach (DeadBody body in ComponentCache<DeadBody>.Cached)
            {
                if (Visibility.IsVisible(body.TruePosition))
                {
                    Info("Spotted a body, trying another vent");
                    goto top; //LMAO
                }
            }

            break;
        }

        HudManager.Instance.ImpostorVentButton.DoClick();
        InGameCursor.Instance.Hide();
        previousVent = null;
        yield break;
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
