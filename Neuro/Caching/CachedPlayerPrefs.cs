using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Caching;

public static class CachedPlayerPrefs
{
    private static readonly Dictionary<string, float> _floatCache = new();
    private static readonly Dictionary<string, int> _intCache = new();
    private static readonly Dictionary<string, string> _stringCache = new();

    public static void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, _floatCache[key] = value);
    public static void SetInt(string key, int value) => PlayerPrefs.SetInt(key, _intCache[key] = value);
    public static void SetString(string key, string value) => PlayerPrefs.SetString(key, _stringCache[key] = value);
    public static void SetBool(string key, bool value) => SetInt(key, value ? 1 : 0);

    public static float GetFloat(string key) => _floatCache.TryGetValue(key, out float result) ? result : _floatCache[key] = PlayerPrefs.GetFloat(key);
    public static int GetInt(string key) => _intCache.TryGetValue(key, out int result) ? result : _intCache[key] = PlayerPrefs.GetInt(key);
    public static string GetString(string key) => _stringCache.TryGetValue(key, out string result) ? result : _stringCache[key] = PlayerPrefs.GetString(key);
    public static bool GetBool(string key) => GetInt(key) != 0;

    public static float GetFloat(string key, float defaultValue) => _floatCache.TryGetValue(key, out float result) ? result : _floatCache[key] = PlayerPrefs.GetFloat(key, defaultValue);
    public static int GetInt(string key, int defaultValue) => _intCache.TryGetValue(key, out int result) ? result : _intCache[key] = PlayerPrefs.GetInt(key, defaultValue);
    public static string GetString(string key, string defaultValue) => _stringCache.TryGetValue(key, out string result) ? result : _stringCache[key] = PlayerPrefs.GetString(key, defaultValue);
    public static bool GetBool(string key, bool defaultValue) => GetInt(key, defaultValue ? 1 : 0) != 0;
}
