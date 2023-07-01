using System.Collections;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(StowArms))]
public sealed class PutAwayRiflesSolver : IMinigameSolver<StowArms, NormalPlayerTask>, IMinigameOpener<NormalPlayerTask>
{
    public float CloseTimout => 6;

    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        // If it's not a StoreArmsTaskConsole, then it's a regular Console so it's part 2 which we always want to open
        if (console.TryCast<StoreArmsTaskConsole>() is not { } stowArmsConsole) return true;

        const float pickupDelay = 0.175f;

        // Use task.Data to store our own information
        if (task.Data.Length == 1) task.Data = ToBytes(stowArmsConsole.timesUsed, Time.time);
        FromBytes(task.Data, out int timesUsed, out float lastTime);

        // If timesUsed has changed, that means we picked up a weapon
        if (stowArmsConsole.timesUsed != timesUsed)
        {
            // Reset timer and don't pick up any more weapons
            task.Data = ToBytes(stowArmsConsole.timesUsed, Time.time);
            return false;
        }

        // Pickup weapon if pickupDelay has elapsed since last pickup
        return Time.time >= lastTime + pickupDelay;
    }

    public IEnumerator CompleteMinigame(StowArms minigame, NormalPlayerTask task)
    {
        task.Data = BitConverter.GetBytes(Time.time);

        if (minigame.RifleContent.active) yield return CompleteWeapons(minigame, minigame.RifleColliders, minigame.RifleSlots);
        else yield return CompleteWeapons(minigame, minigame.GunColliders, minigame.GunsSlots);
    }

    private IEnumerator CompleteWeapons(StowArms minigame, Il2CppReferenceArray<Collider2D> weapons, Il2CppReferenceArray<DragSlot> slots)
    {
        foreach (Collider2D weapon in weapons)
        {
            yield return InGameCursor.Instance.CoMoveTo(weapon);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(slots.First(slot => slot.Occupant == null));
            InGameCursor.Instance.StopHoldingLMB();
        }
    }

    private static byte[] ToBytes(int timesUsed, float lastTime)
    {
        return new[] {(byte) timesUsed}.Concat(BitConverter.GetBytes(lastTime)).ToArray();
    }

    private static void FromBytes(byte[] bytes, out int timesUsed, out float lastTime)
    {
        timesUsed = bytes[0];
        lastTime = BitConverter.ToSingle(bytes, 1);
    }
}
