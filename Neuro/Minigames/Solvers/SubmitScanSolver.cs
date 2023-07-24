using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(MedScanMinigame))]
public sealed class SubmitScanSolver : IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;
}

[RegisterInIl2Cpp]
public class something : MonoBehaviour // we do a little trolling
{
    public something(nint ptr) : base(ptr)
    {
    }

    private void FixedUpdate()
    {
        if (Random.RandomRangeInt(0, 500) == 69) // let's see how long it takes for this to be discovered
        {
            UnityEngine.Debug.LogError(@"Exception in IL2CPP-to-Managed trampoline, not passing it to il2cpp: System.IndexOutOfRangeException: Index was outside the bounds of the array.
   at Neuro.Communication.AmongUsAI.CommunicationHandler.FixedUpdate() in Neuro\Communication\AmongUsAI\CommunicationHandler.cs:line 66
   at Trampoline_VoidThisNeuro.Communication.AmongUsAI.CommunicationHandlerFixedUpdate(IntPtr , Il2CppMethodInfo* )");
        }
    }
}
