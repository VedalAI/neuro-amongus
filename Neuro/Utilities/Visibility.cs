using Neuro.Utilities.Convertors;
using UnityEngine;

namespace Neuro.Utilities;

public static class Visibility
{
    private static bool IsVisible(Vector2 rayStart, Vector2 rayEnd)
    {
        Vector3 viewport = Camera.main.WorldToViewportPoint(rayEnd);
        //Summon neurosama to check if stuff is visible
        bool visible = viewport.x > 0 && viewport.x < 1 && viewport.y > 0 && viewport.y < 1;

        //no point doing a raycast if item isn't on screen
        if (!visible) return false;

        Vector2 ray = rayEnd - rayStart;
        //fire towards shadowmask, if we hit the objects causing shadows, it's not visible.
        RaycastHit2D hit = Physics2D.Raycast(rayStart, ray.normalized, ray.magnitude, Constants.ShadowMask);

        return !hit;
    }

    public static bool IsVisible(PositionProvider target) => IsVisible(PlayerControl.LocalPlayer.GetTruePosition(), target);
}
