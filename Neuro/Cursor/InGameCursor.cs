using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Cursor;

[RegisterInIl2Cpp]
public class InGameCursor : MonoBehaviour
{
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

    public void MoveTo(Vector2 position)
    {
        transform.position = transform.position with {x = position.x, y = position.y};
    }

    public void MoveTo(Component target) => MoveTo(target.transform.position);

    public void Hide() => MoveTo(new Vector2(-5000, -5000));

    public void HideWhen(Func<bool> condition) => this.StartCoroutine(HideWhenCoroutine(condition));
    private IEnumerator HideWhenCoroutine(Func<bool> condition)
    {
        while (!condition()) yield return null;
        Hide();
    }
}
