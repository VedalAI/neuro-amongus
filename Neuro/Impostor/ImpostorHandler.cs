using UnityEngine;
using Reactor.Utilities.Attributes;
using System;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Utilities;
using Neuro.Events;
using Neuro.Cursor;

namespace Neuro.Impostor;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class ImpostorHandler : MonoBehaviour
{
    public ImpostorHandler(IntPtr ptr) : base(ptr) { }

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

    [HideFromIl2Cpp]
    public IEnumerator CoStartVentOut()
    {
        Vent original = Vent.currentVent;
        previousVent = original;
        while (true)
        {
            yield return CoTryMoveToVent();
            bool crewmateSpotted = false;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer) continue;
                if (player.Data.IsDead) continue;
                if (player.Data.Role.IsImpostor) continue;

                // TODO: implement pathfinding to improve decision making
                if (Visibility.IsVisible(player.GetTruePosition()))
                {
                    Info("Spotted a crewmate, trying another vent");
                    crewmateSpotted = true;
                    yield return CoTryMoveToVent();
                    break;
                }
            }

            // avoid exiting from the vent we entered
            if (!crewmateSpotted && Vent.currentVent != original)
            {
                HudManager.Instance.ImpostorVentButton.DoClick();
                InGameCursor.Instance.Hide();
                previousVent = null;
                yield break;
            }
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator CoTryMoveToVent()
    {
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.5f, 0.8f));
        while (true)
        {
            Vent target = GetNextAvailableVent(Vent.currentVent, out int buttonIndex);
            if (buttonIndex == -1)
            {
                Info($"No available vents, trying again.");
                yield return new WaitForFixedUpdate();
                continue;
            }
            previousVent = Vent.currentVent;
            yield return InGameCursor.Instance.CoMoveTo(Vent.currentVent.Buttons[buttonIndex]);
            yield return InGameCursor.Instance.CoPressLMB();
            yield break;
        }
    }

    [HideFromIl2Cpp]
    // since some vents can have a variable amount of neighbors, use this helper method to get available ones
    private static Vent GetNextAvailableVent(Vent current, out int buttonIndex)
    {
        Vent[] nearby = current.NearbyVents;
        VentilationSystem system = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();
        for (int i = 0; i < nearby.Length; i++)
        {
            Vent vent = nearby[i];
            if (!vent ||
                vent == previousVent ||
                system.IsVentCurrentlyBeingCleaned(vent.Id) ||
                system.IsImpostorInsideVent(vent.Id)) continue;
            buttonIndex = i;
            return vent;
        }
        buttonIndex = -1;
        return current;
    }
}
