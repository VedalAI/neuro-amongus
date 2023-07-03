using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Cursor;
using Neuro.Events;
using Neuro.Extensions;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Impostor;

[RegisterInIl2Cpp, FullShipStatusComponent]
public sealed class SabotageHandler : MonoBehaviour
{
    private record struct Sabotage(bool isDoor, SystemTypes room);

    private static RunTimer DoorSystem => ShipStatus.Instance.Systems[SystemTypes.Doors].Cast<RunTimer>();

    public SabotageHandler(IntPtr ptr) : base(ptr)
    {
    }

    public static SabotageHandler Instance { get; private set; }

    // TODO: Implement some kind of queue size limit or decay over time (unless neuro is actually smart enough to know what and when to sabotage)
    private readonly Queue<Sabotage> _sabotageQueue = new();
    private Coroutine _sabotageCoroutine;

    public void SabotageSystem(SystemTypes system)
    {
        if (!ShouldEnqueueSabotage()) return;
        _sabotageQueue.Enqueue(new Sabotage(false, system));
    }

    public void SabotageDoors(SystemTypes system)
    {
        if (!ShouldEnqueueSabotage()) return;
        _sabotageQueue.Enqueue(new Sabotage(true, system));
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        EventManager.RegisterHandler(this);
    }

    private void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
        if (!GameManager.Instance.IsNormal()) return;

        if (_sabotageCoroutine == null && _sabotageQueue.Count > 0 && _sabotageQueue.Any(CanTriggerSabotage))
        {
            _sabotageCoroutine = this.StartCoroutine(SabotageCoroutine());
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator SabotageCoroutine()
    {
        while (!ShouldTriggerSabotage()) yield return null;

        HudManager.Instance.ToggleMapVisible(GameManager.Instance.GetMapOptions());

        yield return new WaitForSeconds(0.3f);

        while (_sabotageQueue.TryDequeue(out Sabotage item))
        {
            if (!CanTriggerSabotage(item))
            {
                _sabotageQueue.Enqueue(item);
            }
            else
            {
                bool satisfiesDoorCondition(ButtonBehavior b) => item.isDoor
                    ? b.OnClick.m_PersistentCalls.m_Calls.At(0).methodName == "SabotageDoors"
                    : b.OnClick.m_PersistentCalls.m_Calls.At(0).methodName != "SabotageDoors";

                ButtonBehavior button;
                try
                {
                    button = MapBehaviour.Instance.infectedOverlay.rooms.Single(r => r.room == item.room)
                        .GetComponentsInChildren<ButtonBehavior>().First(satisfiesDoorCondition);
                }
                catch (Exception)
                {
                    continue; // Sabotage doesn't exist on this map
                }

                yield return InGameCursor.Instance.CoMoveTo(button);
                button.OnClick.Invoke();
                yield return new WaitForSeconds(0.25f);
            }

            if (!_sabotageQueue.Any(CanTriggerSabotage)) break;
        }

        yield return new WaitForSeconds(0.2f);

        InGameCursor.Instance.Hide();
        HudManager.Instance.ToggleMapVisible(GameManager.Instance.GetMapOptions());

        _sabotageCoroutine = null;
    }

    public static bool ShouldEnqueueSabotage()
    {
        return PlayerControl.LocalPlayer && !MeetingHud.Instance && PlayerControl.LocalPlayer.Data.Role.IsImpostor && GameManager.Instance.IsNormal();
    }

    private static bool ShouldTriggerSabotage()
    {
        return ShouldEnqueueSabotage() && !Minigame.Instance && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpen);
    }

    private static bool CanTriggerSabotage(Sabotage sabotage)
    {
        bool canUseDoors, canUseSabotages;

        try
        {
            canUseDoors = MapBehaviour.Instance.infectedOverlay.CanUseDoors;
            canUseSabotages = MapBehaviour.Instance.infectedOverlay.CanUseSpecial;
        }
        catch
        {
            return true;
        }

        return (sabotage.isDoor && canUseDoors && DoorSystem.GetTimer(sabotage.room) <= 0) ||
               (!sabotage.isDoor && canUseSabotages);
    }
}