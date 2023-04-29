using UnityEngine;

namespace Neuro.Utilities;

public static class Visibility
{
    public static bool IsVisible(Vector2 target) => IsVisible(PlayerControl.LocalPlayer.GetTruePosition(), target);

    // TODO: Improve (idea: send raycast from camera to target)
    private static bool IsVisible(Vector2 rayStart, Vector2 rayEnd)
    {
        Vector3 viewport = NeuroUtilities.MainCamera.WorldToViewportPoint(rayEnd);
        bool visible = viewport.x is > 0 and < 1 && viewport.y is > 0 and < 1;
        if (!visible) return false;

        Vector2 ray = rayEnd - rayStart;
        //fire towards target, if we hit shadowmask, target is not visible.
        RaycastHit2D hit = Physics2D.Raycast(rayStart, ray.normalized, ray.magnitude, Constants.ShadowMask);

        return !hit;
    }
}
