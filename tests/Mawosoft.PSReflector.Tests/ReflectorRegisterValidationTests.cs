// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class ReflectorRegisterValidationTests
{
    public static readonly TheoryData<Type[]> NullTypes_TheoryData = new(
        [null!, [typeof(SimpleClass), null!, typeof(SimpleStruct)]]);

    [Theory]
    [MemberData(nameof(NullTypes_TheoryData))]
    public void NullTypes_Throws(Type[] types)
    {
        Reflector reflector = new();
        MemberDescriptor[] members = [new("Method1")];
        var ex = Assert.Throws<ArgumentNullException>(nameof(types), () => reflector.Register(types, members));
        var pattern = "Array element";
        if (types is null)
        {
            Assert.DoesNotContain(pattern, ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Assert.Contains(pattern, ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        Assert.Empty(reflector.RegisteredTypes);
    }

    public static readonly TheoryData<MemberDescriptor[]> NullMembers_TheoryData = new(
        [null!, [new("Method1"), null!, new("Method2")]]);

    [Theory]
    [MemberData(nameof(NullMembers_TheoryData))]
    public void NullMembers_Throws(MemberDescriptor[] members)
    {
        Reflector reflector = new();
        Type[] types = [typeof(SimpleClass), typeof(SimpleStruct)];
        var ex = Assert.Throws<ArgumentNullException>(nameof(members), () => reflector.Register(types, members));
        var pattern = "Array element";
        if (members is null)
        {
            Assert.DoesNotContain(pattern, ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Assert.Contains(pattern, ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        Assert.Empty(reflector.RegisteredTypes);
    }

    [Fact]
    public void EmptyTypes_Throws()
    {
        Reflector reflector = new();
        MemberDescriptor[] members = [new("Method1")];
        Assert.Throws<ArgumentException>("types", () => reflector.Register([], members));
        Assert.Empty(reflector.RegisteredTypes);
    }

    [Fact]
    public void EmptyMembers_Throws()
    {
        Reflector reflector = new();
        Type[] types = [typeof(SimpleClass), typeof(SimpleStruct)];
        Assert.Throws<ArgumentException>("members", () => reflector.Register(types, []));
        Assert.Empty(reflector.RegisteredTypes);
    }

    public static readonly TheoryData<string, Type> UnsupportedType_TheoryData = new(
    [
        ("interface", typeof(IFormattable)),
        ("abstract", typeof(Stream)),
        ("open generics", typeof(Dictionary<,>)),
        ("System.Type", typeof(Type)),
        ("PSObject", typeof(PSObject)),
        ("array", typeof(SimpleClass[])),
        ("pointer", typeof(SimpleClass).MakePointerType()),
        ("by ref", typeof(SimpleClass).MakeByRefType()),
    ]);

    [Theory]
    [MemberData(nameof(UnsupportedType_TheoryData))]
    public void UnsupportedType_Throws(string hint, Type type)
    {
        _ = hint;
        Reflector reflector = new();
        Type[] types = [typeof(SimpleClass), type, typeof(SimpleStruct)];
        MemberDescriptor[] members = [new("Method1")];
        var ex = Assert.Throws<ArgumentException>("types", () => reflector.Register(types, members));
        Assert.Contains(type.ToString() + " is not supported", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(reflector.RegisteredTypes);
    }

    public static readonly TheoryData<MemberDescriptor> InvalidMember_TheoryData =
        [new MemberDescriptor(), new MemberDescriptor() { Name = "Method1", ParamCount = -1 }];

    [Theory]
    [MemberData(nameof(InvalidMember_TheoryData))]
    public void InvalidMember_Throws(MemberDescriptor member)
    {
        Reflector reflector = new();
        Type[] types = [typeof(SimpleClass)];
        MemberDescriptor[] members = [new("Method1"), member];
        var ex = Assert.Throws<ArgumentException>("members", () => reflector.Register(types, members));
        Assert.Contains(member.ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(SimpleClass), ex.Message, StringComparison.Ordinal);
        Assert.Contains(" is invalid.", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(reflector.RegisteredTypes);
    }

    [Fact]
    public void NonmatchingMember_Throws()
    {
        Reflector reflector = new();
        Type[] types = [typeof(SimpleClass)];
        var mih = new MemberInfoHelper(typeof(SimpleStruct));
        MemberDescriptor member = new(mih.GetMethod("Method2"));
        MemberDescriptor[] members = [new("Method1"), member];
        var ex = Assert.Throws<ArgumentException>("members", () => reflector.Register(types, members));
        Assert.Contains(member.ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(SimpleClass), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(SimpleStruct), ex.Message, StringComparison.Ordinal);
        Assert.Contains(" not assignable to ", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(reflector.RegisteredTypes);
    }

    public static readonly TheoryData<MemberDescriptor> UnsupportedMember_TheoryData = new(
        Array.ConvertAll(ClassWithUnsupportedMembers.GetUnsupportedMembers(), mi => new MemberDescriptor(mi)));

    [Theory]
    [MemberData(nameof(UnsupportedMember_TheoryData))]
    public void UnsupportedMember_Throws(MemberDescriptor member)
    {
        Reflector reflector = new();
        Type[] types = [typeof(ClassWithUnsupportedMembers)];
        MemberDescriptor[] members = [member];
        var ex = Assert.Throws<ArgumentException>("members", () => reflector.Register(types, members));
        Assert.Contains(member.ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(ClassWithUnsupportedMembers), ex.Message, StringComparison.Ordinal);
        Assert.Contains(" unsupported MemberInfo.", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(reflector.RegisteredTypes);
    }

    [Fact]
    public void MissingMember_Throws()
    {
        Reflector reflector = new();
        Type[] types = [typeof(SimpleClass)];
        MemberDescriptor member = new("Method4");
        MemberDescriptor[] members = [new("Method1"), member];
        var ex = Assert.Throws<MissingMemberException>(() => reflector.Register(types, members));
        Assert.Contains(member.ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(SimpleClass), ex.Message, StringComparison.Ordinal);
        Assert.Empty(reflector.RegisteredTypes);
    }

    [Fact]
    public void AmbiguousMember_Throws()
    {
        Reflector reflector = new();
        Type[] types = [typeof(ClassWithOverloads)];
        MemberDescriptor member = new("Overload1");
        MemberDescriptor[] members = [new("Method1"), member];
        var ex = Assert.Throws<AmbiguousMatchException>(() => reflector.Register(types, members));
        Assert.Contains(member.ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(ClassWithOverloads), ex.Message, StringComparison.Ordinal);
        Assert.Empty(reflector.RegisteredTypes);
    }

    [Fact]
    public void PropertyWithPublicGetter_ByName_Throws()
    {
        Reflector reflector = new();
        Type[] types = [typeof(SimpleClass)];
        MemberDescriptor member = new("MixedProperty");
        MemberDescriptor[] members = [new("Method1"), member];
        var ex = Assert.Throws<MissingMemberException>(() => reflector.Register(types, members));
        Assert.Contains(member.ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(SimpleClass), ex.Message, StringComparison.Ordinal);
        Assert.Empty(reflector.RegisteredTypes);
    }
}
