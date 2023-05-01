using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Neuro.Cursor;
using Neuro.Utilities;
using Reactor.Utilities.Extensions;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipUploadGame), false)]
[MinigameOpener(typeof(AutoMultistageMinigame))]
public sealed class UploadDataAirshipSolver : IMinigameSolver<AirshipUploadGame, NormalPlayerTask>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task)
    {
        return task.TaskType == TaskTypes.UploadData;
    }

    public IEnumerator CompleteMinigame(AirshipUploadGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Phone);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        HashSet<Vector2> allPositions = new();

        Dictionary<Vector2, LineRenderer> things = new();
        for (int x = 0; x < Screen.width; x += 25)
        {
            for (int y = 0; y < Screen.height; y += 25)
            {
                Vector2 position = NeuroUtilities.MainCamera.ScreenToWorldPoint(new Vector2(x, y));
                allPositions.Add(position);

                GameObject nodeVisualPoint = new("Gizmo (Visual Point)")
                {
                    transform =
                    {
                        parent = minigame.transform,
                        position = position
                    },
                    layer = LayerMask.NameToLayer("UI")
                };
                nodeVisualPoint.transform.localPosition = nodeVisualPoint.transform.localPosition with {z = -200};

                LineRenderer renderer = nodeVisualPoint.AddComponent<LineRenderer>();
                renderer.SetPosition(0, nodeVisualPoint.transform.position);
                renderer.SetPosition(1, nodeVisualPoint.transform.position + new Vector3(0, 0.1f));
                renderer.widthMultiplier = 0.1f;
                renderer.positionCount = 2;
                renderer.material = NeuroUtilities.MaskShaderMat;
                renderer.startColor = Color.red;
                renderer.endColor = Color.red;

                things[position] = renderer;
            }
        }

        yield return new WaitForSeconds(0.1f);

        HashSet<Vector2> poorPositions = new(allPositions);

        while (!minigame.Poor.IsTouching(minigame.Hotspot))
        {
            // Pick a random position that can have an upgrading signal
            Vector2 target = poorPositions.Random();
            Warning($"Picked random position for poor {target} out of {poorPositions.Count} positions");

            // Move to the target position
            float distance = Vector2.Distance(minigame.Phone.transform.position, target);
            float time = distance / 6f;

            Vector2 originalPosition = minigame.Phone.transform.position;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

                if (minigame.Poor.IsTouching(minigame.Hotspot)) break;

                int oldCount = poorPositions.Count;
                // Remove positions based on the current signal
                poorPositions.RemoveWhere(p =>
                {
                    if (!minigame.Poor.IsTouching(minigame.Hotspot) && minigame.Poor.OverlapPoint(p))
                    {
                        things[p].startColor = things[p].endColor = Color.gray;
                        return true;
                    }

                    return false;
                });
                Warning($"None: {oldCount} -> {poorPositions.Count} ({minigame.Poor.IsTouching(minigame.Hotspot)})");

                yield return null;
            }

            yield return null;
        }

        things.Values.Do(v => v.startColor = v.endColor = Color.gray);
        HashSet<Vector2> goodPositions = allPositions.Where(minigame.Poor.OverlapPoint).ToHashSet();
        things.Where(kvp => goodPositions.Contains(kvp.Key)).Do(kvp => kvp.Value.startColor = kvp.Value.endColor = Color.green);

        while (!minigame.Good.IsTouching(minigame.Hotspot))
        {
            bool ok = minigame.Poor.IsTouching(minigame.Hotspot);

            // Pick a random position that can have an upgrading signal
            Vector2 target = goodPositions.Random();

            // Move to the target position
            float distance = Vector2.Distance(minigame.Phone.transform.position, target);
            float time = distance / 6f;

            Vector2 originalPosition = minigame.Phone.transform.position;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

                // If we downgrade in signal, go back.
                if (ok && !minigame.Poor.IsTouching(minigame.Hotspot))
                {
                    for (;; t -= Time.deltaTime)
                    {
                        InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

                        if (minigame.Poor.IsTouching(minigame.Hotspot) || t < 0)
                        {
                            break;
                        }

                        yield return null;
                    }

                    break;
                }

                int oldCount = goodPositions.Count;
                // Remove positions based on the current signal
                goodPositions.RemoveWhere(p =>
                {
                    if (minigame.Poor.IsTouching(minigame.Hotspot) && !minigame.Poor.OverlapPoint(p))
                    {
                        things[p].startColor = things[p].endColor = Color.gray;
                        return true;
                    }

                    if (minigame.Perfect.OverlapPoint(p))
                    {
                        things[p].startColor = things[p].endColor = Color.gray;
                        return true;
                    }

                    return false;
                });
                Warning($"Poor: {oldCount} -> {goodPositions.Count} ({minigame.Poor.IsTouching(minigame.Hotspot)})");

                yield return null;
            }

            yield return null;
        }

        things.Values.Do(v => v.startColor = v.endColor = Color.gray);
        HashSet<Vector2> perfectPositions = allPositions.Where(minigame.Good.OverlapPoint).ToHashSet();
        things.Where(kvp => perfectPositions.Contains(kvp.Key)).Do(kvp => kvp.Value.startColor = kvp.Value.endColor = Color.yellow);

        while (!minigame.Perfect.IsTouching(minigame.Hotspot))
        {
            bool ok = minigame.Good.IsTouching(minigame.Hotspot);

            // Pick a random position that can have an upgrading signal
            Vector2 target = perfectPositions.Random();

            // Move to the target position
            float distance = Vector2.Distance(minigame.Phone.transform.position, target);
            float time = distance / 6f;

            Vector2 originalPosition = minigame.Phone.transform.position;

            for (float t = 0; t < time; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

                // If we downgrade in signal, go back.
                if (ok && !minigame.Good.IsTouching(minigame.Hotspot))
                {
                    for (;; t -= Time.deltaTime)
                    {
                        InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

                        if (minigame.Good.IsTouching(minigame.Hotspot) || t < 0)
                        {
                            break;
                        }

                        yield return null;
                    }

                    break;
                }

                int oldCount = perfectPositions.Count;
                // Remove positions based on the current signal
                perfectPositions.RemoveWhere(p =>
                {
                    if (minigame.Good.IsTouching(minigame.Hotspot) && !minigame.Good.OverlapPoint(p))
                    {
                        things[p].startColor = things[p].endColor = Color.gray;
                        return true;
                    }

                    if (minigame.Perfect.OverlapPoint(p))
                    {
                        things[p].startColor = things[p].endColor = Color.gray;
                        return true;
                    }

                    return false;
                });
                Warning($"Good: {oldCount} -> {perfectPositions.Count} ({minigame.Poor.IsTouching(minigame.Hotspot)}, {minigame.Good.IsTouching(minigame.Hotspot)})");

                yield return null;
            }

            yield return null;
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}
