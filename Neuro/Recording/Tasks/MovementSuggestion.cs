using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.Tasks;

[RegisterInIl2Cpp, FullShipStatusComponent]
public sealed class MovementSuggestion : MonoBehaviour
{
    private class SuggestionTarget
    {
        public Type type { get; init; }
        public Component target { get; set; }
        public int priority { get; set; }
    }

    public static MovementSuggestion Instance { get; private set; }

    public MovementSuggestion(nint ptr) : base(ptr)
    {
    }

    public Component CurrentTarget => _targets.FirstOrDefault()?.target;

    private readonly List<SuggestionTarget> _targets = new();

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void SuggestTarget<T>(Component target, int priority = Priority.Normal)
    {
        if (_targets.Find(t => t.type == typeof(T)) is { } suggestion)
        {
            suggestion.target = target;
            suggestion.priority = priority;
        }
        else
        {
            SuggestionTarget newSuggestion = new()
            {
                type = typeof(T),
                target = target,
                priority = 0,
            };

            _targets.Add(newSuggestion);
            _targets.Sort((a, b) => b.priority.CompareTo(a.priority));
        }
    }

    public void SuggestMeetingButton<T>(int priority = Priority.Normal)
        => SuggestTarget<T>(ShipStatus.Instance.GetComponentsInChildren<SystemConsole>().First(c => c.MinigamePrefab.name.Contains("Emergency")));

    public void ClearSuggestion<T>()
    {
        if (_targets.Find(t => t.type == typeof(T)) is { } suggestion)
        {
            _targets.Remove(suggestion);
        }
    }
}
