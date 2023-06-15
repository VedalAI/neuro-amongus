using UnityEngine;
using Reactor.Utilities.Attributes;
using System;
using System.Collections;
using System.Linq;
using Neuro.Recording;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Utilities;
using Neuro.Events;

namespace Neuro.Impostor;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class ImpostorHandler : MonoBehaviour
{
    public ImpostorHandler(IntPtr ptr) : base(ptr)
    {
    }

    public static ImpostorHandler Instance { get; private set; }

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
    public IEnumerator CoStartVentOut(Vent original)
    {
        Vent[] possibleVents = GetAvailableNearbyVents(original);
        Vent current = possibleVents[^1];
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
        if (!original.TryMoveToVent(current, out string error))
        {
            Error($"Failed to move to vent {current.Id}, reason: {error}");
        }

        while (true)
        {
            bool playerSpotted = false;
            yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
            Frame now = Frame.Now(true);
            foreach (var player in now.OtherPlayers.LastSeenPlayers)
            {
                // if we see a crewmate in our radius, try a different vent
                if (player.IsVisible && !now.Header.OtherImpostors.Contains(player.Id))
                {
                    Info($"Spotted a crewmate, trying a different exit vent...");
                    Vent next = GetAvailableNearbyVents(current)[^1];
                    if (!current.TryMoveToVent(next, out error))
                    {
                        Error($"Failed to move to vent {next.Id}, reason: {error}");
                    }

                    current = next;
                    playerSpotted = true;
                    break;
                }
            }

            if (!playerSpotted)
            {
                HudManager.Instance.ImpostorVentButton.DoClick();
                yield break;
            }
        }
    }

    [HideFromIl2Cpp]
    // since some vents can have a variable amount of neighbors, use this helper method to get available ones
    private static Vent[] GetAvailableNearbyVents(Vent vent) => vent.NearbyVents.Where(v => v).ToArray();
}
