using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ElecLeverGame))]
public sealed class ResetBreakersSolver : IMinigameSolver<ElecLeverGame, NormalPlayerTask>, IMinigameOpener<NormalPlayerTask>
{
    public float CloseTimout => 5;

    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        // We use bytes 7-13 of task.Data to implement memory about which consoles have been seen
        if (task.Data.Length == 7)
        {
            task.Data = task.Data.Concat(Enumerable.Repeat(byte.MaxValue, 7)).ToArray();
        }

        // We want to open a console if we haven't seen it before or if we have seen it before and it's the next one in the sequence
        int currentConsoleNumber = task.Data[console.ConsoleId + 7];
        return currentConsoleNumber == byte.MaxValue || currentConsoleNumber == task.taskStep;
    }

    public IEnumerator CompleteMinigame(ElecLeverGame minigame, NormalPlayerTask task)
    {
        // Save console number to memory
        task.Data[minigame.ConsoleId + 7] = (byte) task.Data.IndexOf((byte) minigame.ConsoleId);

        if (task.Data[task.taskStep] == minigame.ConsoleId)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Handle);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            Vector3 down = minigame.transform.TransformPoint(minigame.Handle.transform.localPosition + new Vector3(0f, -3f, 0f));
            yield return InGameCursor.Instance.CoMoveTo(down);
            yield return new WaitForSeconds(0.1f);
            InGameCursor.Instance.StopHoldingLMB();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            minigame.Close();
        }
    }
}
