// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;

namespace Mawosoft.PSReflector.TestHelpers;

internal ref struct MemberInfoHelper
{
    public Type Type { get; set; }
    public BindingFlags BindingFlags { get; set; }

    public bool Instance
    {
        readonly get => BindingFlags.HasFlag(BindingFlags.Instance);
        set => BindingFlags = value
            ? BindingFlags | BindingFlags.Instance
            : BindingFlags & ~BindingFlags.Instance;
    }

    public bool Static
    {
        readonly get => BindingFlags.HasFlag(BindingFlags.Static);
        set => BindingFlags = value
            ? BindingFlags | BindingFlags.Static
            : BindingFlags & ~BindingFlags.Static;
    }

    public bool Public
    {
        readonly get => BindingFlags.HasFlag(BindingFlags.Public);
        set => BindingFlags = value
            ? BindingFlags | BindingFlags.Public
            : BindingFlags & ~BindingFlags.Public;
    }

    public bool NonPublic
    {
        readonly get => BindingFlags.HasFlag(BindingFlags.NonPublic);
        set => BindingFlags = value
            ? BindingFlags | BindingFlags.NonPublic
            : BindingFlags & ~BindingFlags.NonPublic;
    }

    public bool FlattenHierarchy
    {
        readonly get => BindingFlags.HasFlag(BindingFlags.FlattenHierarchy);
        set => BindingFlags = value
            ? BindingFlags | BindingFlags.FlattenHierarchy
            : BindingFlags & ~BindingFlags.FlattenHierarchy;
    }

    public bool IgnoreCase
    {
        readonly get => BindingFlags.HasFlag(BindingFlags.IgnoreCase);
        set => BindingFlags = value
            ? BindingFlags | BindingFlags.IgnoreCase
            : BindingFlags & ~BindingFlags.IgnoreCase;
    }

    public MemberInfoHelper(Type type)
    {
        Type = type;
        BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    }
    public readonly ConstructorInfo GetConstructor(Type[] types)
        => Type.GetConstructor(BindingFlags, binder: null, types, modifiers: null)
        ?? throw new MissingMemberException(Type.ToString(), ".ctor");

    public readonly FieldInfo GetField(string name)
        => Type.GetField(name, BindingFlags)
        ?? throw new MissingMemberException(Type.ToString(), name);

    public readonly PropertyInfo GetProperty(string name)
        => Type.GetProperty(name, BindingFlags)
        ?? throw new MissingMemberException(Type.ToString(), name);

    public readonly MethodInfo GetMethod(string name)
        => Type.GetMethod(name, BindingFlags)
        ?? throw new MissingMemberException(Type.ToString(), name);

    public readonly MethodInfo GetMethod(string name, Type[] types)
        => Type.GetMethod(name, BindingFlags, binder: null, types, modifiers: null)
        ?? throw new MissingMemberException(Type.ToString(), name);

    public readonly MemberInfo GetMember(string name)
    {
        var members = Type.GetMember(name, BindingFlags);
        if (members.Length == 1) return members[0];
        if (members.Length == 0) throw new MissingMemberException(Type.ToString(), name);
        throw new AmbiguousMatchException("Ambigous match: {name} of {Type}.");
    }
}
