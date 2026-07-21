// Copyright (c) Matthias Wolf, Mawosoft.

using System;

namespace Mawosoft.PSReflector.TestTypes;

// Members must match SimpleClass
public struct SimpleStruct : ILastCallInfoProvider, IMemberAccessor
{
    internal const int ConstField = 101;
    internal static readonly int StaticReadOnlyField = 102;
    [ThreadStatic] internal static int StaticField;

    [ThreadStatic] private static int s_staticReadonlyProperty;
    [ThreadStatic] private static int s_staticProperty;

    internal static int StaticReadOnlyProperty
    {
        get { LastCallInfoStatic2 = new CallInfo([]); return s_staticReadonlyProperty; }
    }

    internal static int StaticProperty
    {
        get { LastCallInfoStatic2 = new CallInfo([]); return s_staticProperty; }
        set { LastCallInfoStatic2 = new CallInfo([value]); s_staticProperty = value; }
    }

    internal readonly int ReadOnlyField = 201;
    internal int Field;

    private int _readonlyProperty;
    private int _property;

    internal int ReadOnlyProperty
    {
        get { LastCallInfo = new CallInfo([]); return _readonlyProperty; }
    }

    internal int Property
    {
        get { LastCallInfo = new CallInfo([]); return _property; }
        set { LastCallInfo = new CallInfo([value]); _property = value; }
    }

    public SimpleStruct() { }
    internal SimpleStruct(int p1, string p2) => LastCallInfo = new CallInfo([p1, p2]);
    internal SimpleStruct(int[] p1) => LastCallInfo = new CallInfo([p1]);
    internal SimpleStruct(params string[] p1) => LastCallInfo = new CallInfo([p1]);

    internal static string StaticMethod1(int p1, string p2)
    {
        LastCallInfoStatic2 = new CallInfo([p1, p2]);
        return LastCallInfoStatic2.MemberName;
    }

    internal static string StaticMethod2(int[] p1)
    {
        LastCallInfoStatic2 = new CallInfo([p1]);
        return LastCallInfoStatic2.MemberName;
    }

    internal static string StaticMethod3(params string[] p1)
    {
        LastCallInfoStatic2 = new CallInfo([p1]);
        return LastCallInfoStatic2.MemberName;
    }

    internal static void StaticVoidMethod4(int p1)
    {
        LastCallInfoStatic2 = new CallInfo([p1]);
    }

    internal string Method1(int p1, string p2)
    {
        LastCallInfo = new CallInfo([p1, p2]);
        return LastCallInfo.MemberName;
    }

    internal string Method2(int[] p1)
    {
        LastCallInfo = new CallInfo([p1]);
        return LastCallInfo.MemberName;
    }

    internal string Method3(params string[] p1)
    {
        LastCallInfo = new CallInfo([p1]);
        return LastCallInfo.MemberName;
    }

    internal void VoidMethod4(int p1)
    {
        LastCallInfo = new CallInfo([p1]);
    }

    // ILastCallInfoProvider

    [ThreadStatic]
    private static CallInfo? s_lastCallInfoStatic;

    public static CallInfo? LastCallInfoStatic2
    {
        get => s_lastCallInfoStatic;
        set => s_lastCallInfoStatic = value;
    }

    public CallInfo? LastCallInfo { get; set; }

    public readonly CallInfo? LastCallInfoStatic
    {
        get => s_lastCallInfoStatic;
        set => s_lastCallInfoStatic = value;
    }

    // IMemberAccessor

    // Helpers for test methods.
    // The goal here is to provide a reflection-like interface without using reflection.
    // Do not access the properties itself here, only their explicit backing fields.

    /// <inheritdoc/>
    public T GetMemberValue<T>(string memberName)
    {
        if (typeof(T) == typeof(int))
        {
            return memberName switch
            {
                nameof(ConstField) => (T)(object)ConstField,
                nameof(StaticReadOnlyField) => (T)(object)StaticReadOnlyField,
                nameof(StaticField) => (T)(object)StaticField,
                nameof(StaticReadOnlyProperty) => (T)(object)s_staticReadonlyProperty,
                nameof(StaticProperty) => (T)(object)s_staticProperty,
                nameof(ReadOnlyField) => (T)(object)ReadOnlyField,
                nameof(Field) => (T)(object)Field,
                nameof(ReadOnlyProperty) => (T)(object)_readonlyProperty,
                nameof(Property) => (T)(object)_property,
            };
        }
        throw new MemberAccessException();
    }

    /// <inheritdoc/>
    public T SetMemberValue<T>(string memberName, T value)
    {
        if (typeof(T) == typeof(int))
        {
            switch (memberName)
            {
                case nameof(StaticField):
                    StaticField = (int)(object)value!;
                    return value;
                case nameof(StaticReadOnlyProperty):
                    s_staticReadonlyProperty = (int)(object)value!;
                    return value;
                case nameof(StaticProperty):
                    s_staticProperty = (int)(object)value!;
                    return value;
                case nameof(Field):
                    Field = (int)(object)value!;
                    return value;
                case nameof(ReadOnlyProperty):
                    _readonlyProperty = (int)(object)value!;
                    return value;
                case nameof(Property):
                    _property = (int)(object)value!;
                    return value;
                case nameof(ConstField):
                case nameof(StaticReadOnlyField):
                case nameof(ReadOnlyField):
                    return GetMemberValue<T>(memberName);
            }
        }
        throw new MemberAccessException();
    }
}
