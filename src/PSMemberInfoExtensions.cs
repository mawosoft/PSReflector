// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mawosoft.PSReflector;

internal static class PSMemberInfoExtensions
{
    private static readonly FieldInfo s_psMemberInfoInstanceField =
        typeof(PSMemberInfo).GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new MissingFieldException(nameof(PSMemberInfo), "instance");

    public static object? GetInstance(this PSMemberInfo @this) => s_psMemberInfoInstanceField.GetValue(@this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? GetBaseObject(this PSMemberInfo @this)
    {
        var baseObject = GetInstance(@this);
        if (baseObject is not PSObject pso) return baseObject;
        return pso.BaseObject;
    }
}
