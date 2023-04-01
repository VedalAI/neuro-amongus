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
    private bool isInMovingCoroutine;
    private Transform followTarget;
    private Func<bool> followWhileCondition;
    private float followSpeed;

    public bool IsDoingContinuousMovement => followTarget || isInMovingCoroutine;
    public bool IsHidden => transform.position.x < -4000;

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

    private void Update()
    {
        Warning(transform.position);

        if (!followTarget) return;

        float speed = followSpeed * SPEED_MULTIPLER;

        transform.position = (Vector3) Vector2.MoveTowards(transform.position, followTarget.position, speed * Time.deltaTime) with {z = transform.position.z};
        if (!followWhileCondition?.Invoke() ?? true)
        {
            followTarget = null;
        }
    }

    public void StopMovement()
    {
        followTarget = null;
        isInMovingCoroutine = false;
    }

    public void SnapTo(Vector2 position, bool stopMovement = true)
    {
        if (stopMovement) StopMovement();
        transform.position = transform.position with {x = position.x, y = position.y};
    }

    public void SnapTo(Component target, bool stopMovement = true) => SnapTo(target.transform.position, stopMovement);

    public void SnapToCenter(bool stopMovement = true) => SnapTo(transform.parent.position, stopMovement);

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(Vector2 position, float speed = 1f)
    {
        StopMovement();

        speed *= SPEED_MULTIPLER;

        // if (IsHidden)
        // {
        //     SnapTo(position);
        //     yield break;
        // }

        if (IsHidden) SnapToCenter();
        yield return null;

        isInMovingCoroutine = true;

        Vector2 originalPosition = transform.position;
        float distance = (position - originalPosition).magnitude;
        float time = distance / speed;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            if (!isInMovingCoroutine) yield break;

            SnapTo(Vector2.Lerp(originalPosition, position, t / time), false);
            yield return null;
        }

        SnapTo(position);
    }

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(Component target, float speed = 1f) => CoMoveTo(target.transform.position, speed);

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(GameObject target, float speed = 1f) => CoMoveTo(target.transform.position, speed);

    public void Hide() => SnapTo(new Vector2(-5000, -5000));

    [HideFromIl2Cpp]
    public void HideWhen(Func<bool> condition) => this.StartCoroutine(HideWhenCoroutine(condition));

    public void StartFollowing(Component target, Func<bool> whileCondition = null, float speed = 1f)
    {
        StopMovement();
        followTarget = target.transform;
        followWhileCondition = whileCondition ?? (() => true);
        followSpeed = speed;

        if (IsHidden) SnapToCenter();
    }

    public void StartFollowing(GameObject target, Func<bool> whileCondition = null, float speed = 1f) => StartFollowing(target.transform, whileCondition, speed);

    [HideFromIl2Cpp]
    private IEnumerator HideWhenCoroutine(Func<bool> condition)
    {
        while (!condition()) yield return null;
        Hide();
    }
}
