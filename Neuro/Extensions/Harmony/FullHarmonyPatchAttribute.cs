using System;
using System.Diagnostics;
using HarmonyLib;

namespace Neuro.Extensions.Harmony;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = true)]
[Conditional("FULL")]
public sealed class FullHarmonyPatchAttribute : HarmonyPatch
{
    public FullHarmonyPatchAttribute()
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType) : base(declaringType)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, Type[] argumentTypes) : base(declaringType, argumentTypes)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, string methodName) : base(declaringType, methodName)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, string methodName, params Type[] argumentTypes) : base(declaringType, methodName, argumentTypes)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, methodName, argumentTypes, argumentVariations)
    {
    }

    public FullHarmonyPatchAttribute(string typeName, string methodName) : base(typeName, methodName)
    {
    }

    public FullHarmonyPatchAttribute(string typeName, string methodName, MethodType methodType, Type[] argumentTypes = null, ArgumentType[] argumentVariations = null) : base(typeName, methodName, methodType, argumentTypes, argumentVariations)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, MethodType methodType) : base(declaringType, methodType)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, MethodType methodType, params Type[] argumentTypes) : base(declaringType, methodType, argumentTypes)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, methodType, argumentTypes, argumentVariations)
    {
    }

    public FullHarmonyPatchAttribute(Type declaringType, string methodName, MethodType methodType) : base(declaringType, methodName, methodType)
    {
    }

    public FullHarmonyPatchAttribute(string methodName) : base(methodName)
    {
    }

    public FullHarmonyPatchAttribute(string methodName, params Type[] argumentTypes) : base(methodName, argumentTypes)
    {
    }

    public FullHarmonyPatchAttribute(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(methodName, argumentTypes, argumentVariations)
    {
    }

    public FullHarmonyPatchAttribute(string methodName, MethodType methodType) : base(methodName, methodType)
    {
    }

    public FullHarmonyPatchAttribute(MethodType methodType) : base(methodType)
    {
    }

    public FullHarmonyPatchAttribute(MethodType methodType, params Type[] argumentTypes) : base(methodType, argumentTypes)
    {
    }

    public FullHarmonyPatchAttribute(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(methodType, argumentTypes, argumentVariations)
    {
    }

    public FullHarmonyPatchAttribute(Type[] argumentTypes) : base(argumentTypes)
    {
    }

    public FullHarmonyPatchAttribute(Type[] argumentTypes, ArgumentType[] argumentVariations) : base(argumentTypes, argumentVariations)
    {
    }

    public FullHarmonyPatchAttribute(string typeName, string methodName, MethodType methodType = MethodType.Normal) : base(typeName, methodName, methodType)
    {
    }
}