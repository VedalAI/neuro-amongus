using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AlignGame))]
public class AlignEngineOutputSolver : GeneralMinigameSolver<AlignGame>
{
    public override IEnumerator CompleteMinigame(AlignGame minigame, NormalPlayerTask task)
    {
        Collider2D slider = minigame.col;
        yield return InGameCursor.Instance.CoMoveTo(slider);

        for (float t = 0; t < 1f; t += Time.deltaTime) {
            InGameCursor.Instance.SnapTo(slider);

            // taken from AlignGame.Update()
            Vector3 localPosition = slider.transform.localPosition;
            localPosition.y = minigame.YRange.Clamp(Mathf.Lerp(localPosition.y, 0, t));
            float num = minigame.YRange.ReverseLerp(localPosition.y);
            localPosition.x = minigame.curve.Evaluate(num);
            slider.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(20f, -20f, num));
            minigame.engine.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(45f, -45f, num));
            minigame.MyNormTask.Data[minigame.ConsoleId] = AlignGame.ToByte(num);
            minigame.centerline.material.SetColor("_Color", AlignGame.ShouldComplete(minigame.MyNormTask.Data[minigame.ConsoleId]) ? Color.green : Color.red);
            slider.transform.localPosition = localPosition;
            yield return null;
        }

        // since we aren't actually clicking anything, handle the completion logic ourselves
        minigame.MyNormTask.NextStep();
        minigame.MyNormTask.Data[minigame.ConsoleId + 2] = 1;
        minigame.StartCoroutine(minigame.LockEngine());
        minigame.StartCoroutine(minigame.CoStartClose());
    }
}
