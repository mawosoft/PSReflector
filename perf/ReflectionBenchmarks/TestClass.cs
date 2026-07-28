// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ReflectionBenchmarks;

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS0649 // Field is never assigned to
public class TestClass
{
    internal static readonly TestClassMember<FieldInfo>[] Fields =
    [
        new(typeof(TestClass).GetField(nameof(Field1)), 1, 111),
        new(typeof(TestClass).GetField(nameof(Field2)), 2, new Version(3, 2, 1)),
    ];

    internal static readonly TestClassMember<PropertyInfo>[] Properties =
    [
        new(typeof(TestClass).GetProperty(nameof(Property1)), 1, 111),
        new(typeof(TestClass).GetProperty(nameof(Property2)), 2, new Version(3, 2, 1)),
    ];

    internal static TestClassMember<T>[] GetMembers<T>() where T : MemberInfo
    {
        if (typeof(T) == typeof(FieldInfo)) return (TestClassMember<T>[])(object)Fields;
        if (typeof(T) == typeof(PropertyInfo)) return (TestClassMember<T>[])(object)Properties;
        throw new NotImplementedException();
    }

    internal void InitMembers()
    {
        Property1 = Field1 = 101;
        Property2 = Field2 = new Version(1, 2, 3);
    }

    public int Field1;
    public Version? Field2;
    public int Property1 { get; set; }
    public Version? Property2 { get; set; }

    // TODO variants few/many arguments valuetype/class
    [MethodImpl(MethodImplOptions.NoInlining)] public void Method1() { }
    [MethodImpl(MethodImplOptions.NoInlining)] public int Method2(int p1) => p1;
    [MethodImpl(MethodImplOptions.NoInlining)] public Version? Method3(Version? p1) => p1;
}
