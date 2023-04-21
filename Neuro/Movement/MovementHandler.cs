using System;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Movement;

[RegisterInIl2Cpp]
public sealed class MovementHandler : MonoBehaviour
{
    private const float OBSTACLE_PADDING = 0.5f; // The minimum distance betweeen the agent an obstacle
    private const float PADDING_STRENGTH = 0.5f; // The speed at which the agent will be pushed away from the obstacle. 1 is max.

    public static MovementHandler Instance { get; private set; }

    public MovementHandler(IntPtr ptr) : base(ptr)
    {
    }

    public Vector2 ForcedMoveDirection { get; set; }

    private LineRenderer _arrow;

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;

        CreateArrow();
    }

    public void GetForcedMoveDirection(ref Vector2 direction)
    {
        if (direction != Vector2.zero) return;
        direction = ForcedMoveDirection.normalized; // TODO: We need to adjust this based on player speed setting

        /*RepeatedField<float> raycastResults = LocalPlayerRecorder.Instance.Frame.RaycastObstacleDistances;
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i] < OBSTACLE_PADDING)
            {
                direction -= LocalPlayerRecorder.RaycastDirections[i] * (OBSTACLE_PADDING - raycastResults[i]) * 1 / OBSTACLE_PADDING * PADDING_STRENGTH;
            }
        }*/

        /* TODO: I tried to fix the issues with the obstacle padding but it didn't work for tight corridors
        // if we're close to a wall, add bias to move away from it
        RepeatedField<float> raycastResults = LocalPlayerRecorder.Instance.Frame.RaycastObstacleDistances;
        for (int i = 0; i < raycastResults.Count; i++)
        {
            // We need to multiply the padding distance for diagonal rays by sqrt(2)
            float myPadding = OBSTACLE_PADDING * LocalPlayerRecorder.RaycastDirections[i].magnitude;

            // If we are in a tight corridor, reduce the padding so we have a tunnel
            float myEncroaching = Math.Max(0, myPadding - raycastResults[i]);
            float oppositeEncroaching = Math.Max(0, myPadding - raycastResults[(i + 4) % raycastResults.Count]);
            float calculatedPadding = Math.Clamp(0, (myEncroaching + oppositeEncroaching) / 2f - 0.1f, myPadding);

            // If two consecutive diagonal raycasts hit something, we don't apply their effect because it will be applied by the straight raycast
            if (i % 2 != 0)
            {
                float otherDiagonal1 = raycastResults[(i + 2) % raycastResults.Count];
                float otherDiagonal2 = raycastResults[(i + 6) % raycastResults.Count];

                if (otherDiagonal1 < calculatedPadding) continue;
                if (otherDiagonal2 < calculatedPadding) continue;
            }

            if (raycastResults[i] < calculatedPadding)
            {
                Warning($"Pushing player from direction {i} by {(calculatedPadding - raycastResults[i]) * 1 / calculatedPadding * PADDING_STRENGTH} units");
                direction -= LocalPlayerRecorder.RaycastDirections[i] * (calculatedPadding - raycastResults[i]) * 1/calculatedPadding * PADDING_STRENGTH;
            }
        }
        */

        LineRenderer renderer = _arrow;
        renderer.SetPosition(0, PlayerControl.LocalPlayer.GetTruePosition());
        renderer.SetPosition(1, PlayerControl.LocalPlayer.GetTruePosition() + direction);
    }

    private void CreateArrow()
    {
        GameObject arrowObject = new("Arrow");
        arrowObject.transform.parent = transform;
        _arrow = arrowObject.AddComponent<LineRenderer>();

        _arrow.startWidth = 0.4f;
        _arrow.endWidth = 0.05f;
        _arrow.positionCount = 2;
        _arrow.material = new Material(Shader.Find("Sprites/Default"));
        _arrow.startColor = Color.blue;
        _arrow.endColor = Color.cyan;
    }

    [EventHandler(EventTypes.GameStarted)]
    public static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<MovementHandler>();
    }
}
