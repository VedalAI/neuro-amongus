using System.Linq;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.Tasks;

[RegisterInIl2Cpp, FullShipStatusComponent]
public sealed class MovementSuggestion : MonoBehaviour
{
    public static MovementSuggestion Instance { get; private set; }

    public MovementSuggestion(nint ptr) : base(ptr)
    {
    }

    public bool Enabled { get; private set; }
    public Component Target { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void SuggestTarget(Component target)
    {
        Target = target;
        Enabled = true;
    }

    public void SuggestMeetingButton()
        => SuggestTarget(ShipStatus.Instance.GetComponentsInChildren<SystemConsole>().First(c => c.MinigamePrefab.name.Contains("Emergency")));

    public void ClearSuggestion()
    {
        Enabled = false;
    }
}
