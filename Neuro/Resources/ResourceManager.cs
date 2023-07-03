using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Resources;

public static class ResourceManager
{
    private static readonly Dictionary<string, Sprite> _spriteCache = new();

    private static readonly Assembly _assembly = typeof(ResourceManager).Assembly;

    private static Il2CppStructArray<byte> GetEmbeddedBytes(string name)
    {
        string path = _assembly.GetManifestResourceNames().FirstOrDefault(n => n.Contains(name));
        if (path == null) return null;

        Stream manifestResourceStream = _assembly.GetManifestResourceStream(path)!;
        return manifestResourceStream.ReadFully();
    }

    private static Texture2D LoadTexture(string name)
    {
        Il2CppStructArray<byte> buffer = GetEmbeddedBytes(name);
        if (buffer == null) return null;

        Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
        ImageConversion.LoadImage(tex, buffer, false);
        return tex;
    }

    public static AssetBundle LoadAssetBundle(string name)
    {
        Il2CppStructArray<byte> buffer = GetEmbeddedBytes(name);
        if (buffer == null) return null;

        AssetBundle assetBundle = AssetBundle.LoadFromMemory(buffer);
        return assetBundle;
    }

    public static Sprite LoadSprite(string name, float ppu = 100)
    {
        Texture2D tex = LoadTexture(name);
        if (tex == null) return null;

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
    }

    public static void CacheSprite(string name, float ppu = 100, string cacheName = null)
    {
        cacheName ??= name;
        Sprite sprite = LoadSprite(name, ppu);
        sprite.DontUnload();
        _spriteCache[cacheName] = sprite;
    }

    public static Sprite GetCachedSprite(string name)
    {
        return _spriteCache[name];
    }
}