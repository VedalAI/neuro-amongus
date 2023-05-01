using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        HashSet<Vector2> allPositions = GenerateAllPositions();
        Dictionary<Vector2, LineRenderer> positionVisuals = CreatePositionVisuals(minigame, allPositions);

        yield return new WaitForSeconds(0.1f);

        HashSet<Vector2> poorPositions = new(allPositions);

        while (!minigame.Poor.IsTouching(minigame.Hotspot))
        {
            yield return MoveToUpgradingSignal(minigame, poorPositions, positionVisuals, minigame.Poor);
        }

        HashSet<Vector2> goodPositions = FilterPositionsByCollider(allPositions, minigame.Poor);
        UpdateVisualsColor(positionVisuals, goodPositions, Color.green);

        while (!minigame.Good.IsTouching(minigame.Hotspot))
        {
            yield return MoveToUpgradingSignal(minigame, goodPositions, positionVisuals, minigame.Good);
        }

        HashSet<Vector2> perfectPositions = FilterPositionsByCollider(allPositions, minigame.Good);
        UpdateVisualsColor(positionVisuals, perfectPositions, Color.yellow);

        while (!minigame.Perfect.IsTouching(minigame.Hotspot))
        {
            yield return MoveToUpgradingSignal(minigame, perfectPositions, positionVisuals, minigame.Perfect);
        }

        InGameCursor.Instance.StopHoldingLMB();
    }

    private HashSet<Vector2> GenerateAllPositions()
    {
        HashSet<Vector2> allPositions = new();

        for (int x = 0; x < Screen.width; x += 25)
        {
            for (int y = 0; y < Screen.height; y += 25)
            {
                Vector2 position = NeuroUtilities.MainCamera.ScreenToWorldPoint(new Vector2(x, y));
                allPositions.Add(position);
            }
        }

        return allPositions;
    }

    private Dictionary<Vector2, LineRenderer> CreatePositionVisuals(AirshipUploadGame minigame, HashSet<Vector2> allPositions)
    {
        Dictionary<Vector2, LineRenderer> positionVisuals = new();

        foreach (Vector2 position in allPositions)
        {
            GameObject nodeVisualPoint = new("Gizmo (Visual Point)")
            {
                transform =
                {
                    parent = minigame.transform,
                    position = position
                },
                layer = LayerMask.NameToLayer("UI")
            };
            nodeVisualPoint.transform.localPosition = nodeVisualPoint.transform.localPosition with
            {
                z = -200
            };
            LineRenderer renderer = nodeVisualPoint.AddComponent<LineRenderer>();
            renderer.SetPosition(0, nodeVisualPoint.transform.position);
            renderer.SetPosition(1, nodeVisualPoint.transform.position + new Vector3(0, 0.1f));
            renderer.widthMultiplier = 0.1f;
            renderer.positionCount = 2;
            renderer.material = NeuroUtilities.MaskShaderMat;
            renderer.startColor = Color.red;
            renderer.endColor = Color.red;

            positionVisuals[position] = renderer;
        }

        return positionVisuals;
    }

    private IEnumerator MoveToUpgradingSignal(AirshipUploadGame minigame, HashSet<Vector2> targetPositions, Dictionary<Vector2, LineRenderer> positionVisuals, Collider2D targetCollider)
    {
        Vector2 target = targetPositions.Random();
        float distance = Vector2.Distance(minigame.Phone.transform.position, target);
        float time = distance / 6f;

        Vector2 originalPosition = minigame.Phone.transform.position;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            InGameCursor.Instance.SnapTo(Vector2.Lerp(originalPosition, target, t / time));

            if (targetCollider.IsTouching(minigame.Hotspot)) break;

            targetPositions.RemoveWhere(p =>
            {
                if (!targetCollider.IsTouching(minigame.Hotspot) && targetCollider.OverlapPoint(p))
                {
                    positionVisuals[p].startColor = positionVisuals[p].endColor = Color.gray;
                    return true;
                }

                return false;
            });

            yield return null;
        }

        yield return null;
    }

    private HashSet<Vector2> FilterPositionsByCollider(HashSet<Vector2> allPositions, Collider2D targetCollider)
    {
        return allPositions.Where(targetCollider.OverlapPoint).ToHashSet();
    }

    private void UpdateVisualsColor(Dictionary<Vector2, LineRenderer> positionVisuals, HashSet<Vector2> positions, Color color)
    {
        foreach (Vector2 position in positions)
        {
            positionVisuals[position].startColor = color;
            positionVisuals[position].endColor = color;
        }
    }
}
