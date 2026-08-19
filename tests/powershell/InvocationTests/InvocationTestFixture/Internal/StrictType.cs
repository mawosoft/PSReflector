// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;

namespace InvocationTestFixture.Internal;

internal ref struct StrictType
{
    private const BindingFlags DefaultBindingFlags = BindingFlags.Public
                                                     | BindingFlags.NonPublic
                                                     | BindingFlags.Instance
                                                     | BindingFlags.Static
                                                     | BindingFlags.DeclaredOnly;

    public Type Type { get; set; }
    public BindingFlags BindingFlags { get; set; } = DefaultBindingFlags;

    public StrictType(Assembly assembly, string typeName)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentException.ThrowIfNullOrEmpty(typeName);
        Type = assembly.GetType(typeName) ?? throw new TypeLoadException($"Type not found: {typeName}");
    }

    public StrictType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        Type = type;
    }

    public readonly FieldInfo GetField(string name)
        => Type.GetField(name, BindingFlags)
        ?? throw new MissingMemberException(Type.ToString(), name);

    public readonly PropertyInfo GetProperty(string name)
        => Type.GetProperty(name, BindingFlags)
        ?? throw new MissingMemberException(Type.ToString(), name);

    public readonly MethodInfo GetMethod(string name)
        => Type.GetMethod(name, BindingFlags)
        ?? throw new MissingMemberException(Type.ToString(), name);
}
