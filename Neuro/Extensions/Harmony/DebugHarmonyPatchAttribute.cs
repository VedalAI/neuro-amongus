using System;
using System.Diagnostics;
using HarmonyLib;

namespace Neuro.Extensions.Harmony;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = true)]
[Conditional("DEBUG")]
public sealed class DebugHarmonyPatchAttribute : HarmonyPatch
{
    public DebugHarmonyPatchAttribute()
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType) : base(declaringType)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, Type[] argumentTypes) : base(declaringType, argumentTypes)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, string methodName) : base(declaringType, methodName)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, string methodName, params Type[] argumentTypes) : base(declaringType, methodName, argumentTypes)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, methodName, argumentTypes, argumentVariations)
    {
    }

    public DebugHarmonyPatchAttribute(string typeName, string methodName) : base(typeName, methodName)
    {
    }

    public DebugHarmonyPatchAttribute(string typeName, string methodName, MethodType methodType, Type[] argumentTypes = null, ArgumentType[] argumentVariations = null) : base(typeName, methodName, methodType, argumentTypes, argumentVariations)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, MethodType methodType) : base(declaringType, methodType)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, MethodType methodType, params Type[] argumentTypes) : base(declaringType, methodType, argumentTypes)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, methodType, argumentTypes, argumentVariations)
    {
    }

    public DebugHarmonyPatchAttribute(Type declaringType, string methodName, MethodType methodType) : base(declaringType, methodName, methodType)
    {
    }

    public DebugHarmonyPatchAttribute(string methodName) : base(methodName)
    {
    }

    public DebugHarmonyPatchAttribute(string methodName, params Type[] argumentTypes) : base(methodName, argumentTypes)
    {
    }

    public DebugHarmonyPatchAttribute(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(methodName, argumentTypes, argumentVariations)
    {
    }

    public DebugHarmonyPatchAttribute(string methodName, MethodType methodType) : base(methodName, methodType)
    {
    }

    public DebugHarmonyPatchAttribute(MethodType methodType) : base(methodType)
    {
    }

    public DebugHarmonyPatchAttribute(MethodType methodType, params Type[] argumentTypes) : base(methodType, argumentTypes)
    {
    }

    public DebugHarmonyPatchAttribute(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(methodType, argumentTypes, argumentVariations)
    {
    }

    public DebugHarmonyPatchAttribute(Type[] argumentTypes) : base(argumentTypes)
    {
    }

    public DebugHarmonyPatchAttribute(Type[] argumentTypes, ArgumentType[] argumentVariations) : base(argumentTypes, argumentVariations)
    {
    }

    public DebugHarmonyPatchAttribute(string typeName, string methodName, MethodType methodType = MethodType.Normal) : base(typeName, methodName, methodType)
    {
    }
}