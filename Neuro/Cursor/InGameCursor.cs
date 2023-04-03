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

    private SpriteRenderer _renderer;
    private bool _isInMovingCoroutine;
    private Transform _followTarget;
    private Func<bool> _followCondition;
    private float _followSpeed;

    public Vector2 Position => transform.position;
    public bool IsDoingContinuousMovement => _followTarget || _isInMovingCoroutine;
    public bool IsMouseDown = false;
    public bool IsHidden => transform.position.x < -4000;
    public float DistanceToTarget => _followTarget ? (Position - (Vector2) _followTarget.position).magnitude : -1;

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

        _renderer = gameObject.AddComponent<SpriteRenderer>();
        _renderer.sprite = ResourceManager.GetCachedSprite("Cursor");
    }

    private void Update()
    {
        if (!_followTarget) return;

        float speed = _followSpeed * SPEED_MULTIPLER;

        transform.position = (Vector3) Vector2.MoveTowards(Position, _followTarget.position, speed * Time.deltaTime) with {z = transform.position.z};

        if (!_followCondition())
        {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        _followTarget = null;
        _isInMovingCoroutine = false;
    }

    public void SnapTo(Vector2 position, bool stopMovement = true)
    {
        if (stopMovement) StopMovement();
        transform.position = transform.position with {x = position.x, y = position.y};
    }

    public void SnapTo(Component target, bool stopMovement = true) => SnapTo(target.transform.position, stopMovement);

    public void SnapToCenter(bool stopMovement = true) => SnapTo(transform.parent.position, stopMovement);

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(Vector2 targetPosition, float speed = 1f)
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

        _isInMovingCoroutine = true;

        Vector2 originalPosition = Position;
        float distance = (targetPosition - originalPosition).magnitude;
        float time = distance / speed;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            if (!_isInMovingCoroutine) yield break;

            SnapTo(Vector2.Lerp(originalPosition, targetPosition, t / time), false);
            yield return null;
        }

        SnapTo(targetPosition);
    }

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(Component target, float speed = 1f) => CoMoveTo(target.transform.position, speed);

    [HideFromIl2Cpp]
    public IEnumerator CoMoveTo(GameObject target, float speed = 1f) => CoMoveTo(target.transform.position, speed);

    public void Hide() => SnapTo(new Vector2(-5000, -5000));

    [HideFromIl2Cpp]
    public void HideWhen(Func<bool> condition) => this.StartCoroutine(HideWhenCoroutine(condition));

    [HideFromIl2Cpp]
    public void StartFollowing(Component target, Func<bool> whileCondition = null, float speed = 1f)
    {
        StopMovement();
        _followTarget = target.transform;
        _followCondition = whileCondition ?? (() => true);
        _followSpeed = speed;

        if (IsHidden) SnapToCenter();
    }

    [HideFromIl2Cpp]
    public void StartFollowing(GameObject target, Func<bool> whileCondition = null, float speed = 1f) => StartFollowing(target.transform, whileCondition, speed);

    [HideFromIl2Cpp]
    private IEnumerator HideWhenCoroutine(Func<bool> condition)
    {
        while (!condition()) yield return null;
        Hide();
    }
}
