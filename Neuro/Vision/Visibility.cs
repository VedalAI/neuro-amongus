using UnityEngine;

namespace Neuro.Vision;

public static class Visibility
{
    private static readonly int _shadowLayer = LayerMask.GetMask("Shadow");

    public static bool IsVisible(Vector2 rayStart, Vector2 rayEnd)
    {
        Vector2 ray = rayEnd - rayStart;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, ray.normalized, ray.magnitude, _shadowLayer);
        return !hit;
    }

    public static bool IsVisible(Vector2 target) => IsVisible(PlayerControl.LocalPlayer.GetTruePosition(), target);

    public static bool IsVisible(PlayerControl player) => IsVisible(player.GetTruePosition());

    public static bool IsVisible(DeadBody deadBody) => IsVisible(deadBody.TruePosition);
}
