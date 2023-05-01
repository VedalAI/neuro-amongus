using System.Collections;
using System.Collections.Generic;
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

        Dictionary<Vector2, GameObject> things = new();
        HashSet<Vector2> positionsToSearch = new HashSet<Vector2>();
        for (int x = 0; x < Screen.width; x += 50)
        {
            for (int y = 0; y < Screen.height; y += 50)
            {
                Vector2 position = NeuroUtilities.MainCamera.ScreenToWorldPoint(new Vector2(x, y));
                positionsToSearch.Add(position);

                GameObject nodeVisualPoint = new("Gizmo (Visual Point)")
                {
                    transform =
                    {
                        parent = minigame.transform,
                        position = position
                    }
                };
                nodeVisualPoint.transform.localPosition = nodeVisualPoint.transform.localPosition with {z = -20};

                LineRenderer renderer = nodeVisualPoint.AddComponent<LineRenderer>();
                renderer.SetPosition(0, nodeVisualPoint.transform.position);
                renderer.SetPosition(1, nodeVisualPoint.transform.position + new Vector3(0, 0.2f));
                renderer.widthMultiplier = 0.2f;
                renderer.positionCount = 2;
                renderer.material = NeuroUtilities.MaskShaderMat;
                renderer.startColor = Color.red;
                renderer.endColor = Color.red;

                things[position] = nodeVisualPoint;
            }
        }

        while (!minigame.Perfect.IsTouching(minigame.Hotspot))
        {
            // Pick a random position that can have an upgrading signal
            Vector2 target = positionsToSearch.Random();
            if (target == Vector2.zero)
            {
                yield return CompleteMinigame(minigame, task);
                yield break;
            }

            // Move to the target position
            float distance = Vector2.Distance(minigame.Phone.transform.position, target);
            float time = distance / 6f;

            Vector2 lastPosition = minigame.Phone.transform.position;
            bool isPoor = minigame.Poor.IsTouching(minigame.Hotspot);
            bool isGood = minigame.Good.IsTouching(minigame.Hotspot);

            Vector2 originalPosition = minigame.Phone.transform.position;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

                bool isDowngrade = (isPoor && !minigame.Poor.IsTouching(minigame.Hotspot)) ||
                                   (isGood && !minigame.Good.IsTouching(minigame.Hotspot));
                if (isDowngrade)
                {
                    for (;; t -= Time.deltaTime)
                    {
                        InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

                        isDowngrade = (isPoor && !minigame.Poor.IsTouching(minigame.Hotspot)) ||
                                           (isGood && !minigame.Good.IsTouching(minigame.Hotspot));
                        if (!isDowngrade)
                        {
                            break;
                        }

                        yield return null;
                    }
                }

                // Remove positions based on the current signal
                positionsToSearch.RemoveWhere(p =>
                {
                    float dist = Vector2.Distance(minigame.Phone.transform.position, p);
                    if (!minigame.Poor.IsTouching(minigame.Hotspot) && minigame.Poor.bounds.Contains(p))
                    {
                        things[p].Destroy();
                        return true;
                    }
                    if (minigame.Poor.IsTouching(minigame.Hotspot) && !minigame.Poor.bounds.Contains(p))
                    {
                        things[p].Destroy();
                        return true;
                    }
                    if (minigame.Good.IsTouching(minigame.Hotspot) && !minigame.Good.bounds.Contains(p))
                    {
                        things[p].Destroy();
                        return true;
                    }
                    return false;
                });

                // If we have reached Perfect, break the loop and stop the function
                if (minigame.Perfect.IsTouching(minigame.Hotspot))
                {
                    break;
                }

                yield return null;
            }
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}
