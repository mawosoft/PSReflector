// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;

namespace Mawosoft.PSReflector.TestTypes;

public class ClassWithUnsupportedMembersBase { internal ClassWithUnsupportedMembersBase() { } }

#pragma warning disable CS0067 // Unreferenced event
#pragma warning disable CA1044 // Properties should not be write only
public class ClassWithUnsupportedMembers : ClassWithUnsupportedMembersBase
{
    static ClassWithUnsupportedMembers() { }

    internal static int StaticWriteOnlyProperty { set => _ = value; }
    internal int WriteOnlyProperty { set => this[0] = value; }

    internal int this[int index] { get => 101; set => _ = value; }

    internal static event EventHandler? StaticEvent;
    internal event EventHandler? Event;

    internal void GenericMethod<T>(T _1) { _ = this[0]; }

    internal static class StaticNestedClass { }
    internal sealed class NestedClass { }

    // Helper for test methods.

    public static MemberInfo[] GetUnsupportedMembers()
    {
        // We have to exclude accessor methods and companion fields for events.
        BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        Type t = typeof(ClassWithUnsupportedMembers);
        return
        [
            .. typeof(ClassWithUnsupportedMembersBase).GetConstructors(bf),
            .. t.GetConstructors(bf),
            .. t.GetProperties(bf),
            .. t.GetEvents(bf),
            t.GetMethod("GenericMethod", bf),
            .. t.GetNestedTypes(bf)
        ];
    }
}
