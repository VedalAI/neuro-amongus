using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Cursor;

[RegisterInIl2Cpp]
public sealed class InGameCursor : MonoBehaviour
{
    public const float SPEED_MULTIPLER = 15;

    public static InGameCursor Instance { get; private set; }

    public InGameCursor(IntPtr ptr) : base(ptr) { }

    private SpriteRenderer renderer;

    private void Awake()
    {
        if (Instance)
        {
            Warning("Tried to create an instance of InGameCursor when it already exists");
            Destroy(this);
            return;
        }

        Instance = this;

        gameObject.layer = LayerMask.NameToLayer("UI");

        transform.SetParent(Camera.main!.transform, false);
        transform.localPosition = new Vector3(0f, 0f, -650);
        Hide();

        renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = ResourceManager.GetCachedSprite("Cursor");
    }

    public void SnapTo(Vector2 position)
    {
        transform.position = transform.position with {x = position.x, y = position.y};
    }

    public void SnapTo(Component target) => SnapTo(target.transform.position);

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(Vector2 position, float speed = 1f)
    {
        speed *= SPEED_MULTIPLER;

        Vector2 originalPosition = transform.position;

        if (originalPosition.x < -4000)
        {
            SnapTo(position);
            yield break;
        }

        float distance = (position - originalPosition).magnitude;
        float time = distance / speed;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            SnapTo(Vector2.Lerp(originalPosition, position, t / time));
            yield return null;
        }
    }

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(Component target, float speed = 1f) => CoMoveTo(target.transform.position, speed);

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(GameObject target, float speed = 1f) => CoMoveTo(target.transform.position, speed);

    public void Hide() => SnapTo(new Vector2(-5000, -5000));

    [HideFromIl2Cpp]
    public void HideWhen(Func<bool> condition) => this.StartCoroutine(HideWhenCoroutine(condition));

    [HideFromIl2Cpp]
    private IEnumerator HideWhenCoroutine(Func<bool> condition)
    {
        while (!condition()) yield return null;
        Hide();
    }
}
