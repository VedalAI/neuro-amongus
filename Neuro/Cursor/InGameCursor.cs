using System;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Cursor;

[RegisterInIl2Cpp]
public class InGameCursor : MonoBehaviour
{
    public static InGameCursor Instance { get; private set; }

    public InGameCursor(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        if (Instance)
        {
            Warning("Tried to create an instance of InGameCursor when it already exists");
            Destroy(this);
            return;
        }

        Instance = this;
    }
}
