// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class ReflectorExposeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(typeof(ConcreteBaseClass))]
    public void InvalidFallbackType_Throws(Type? fallbackType)
    {
        Reflector reflector = new();
        var instance = new SimpleClass();
        Assert.Throws<ArgumentException>(nameof(fallbackType), () => reflector.Expose(instance, fallbackType!));
    }

    public static readonly TheoryData<object?> Unregistered_TheoryData =
    [
        null!,
        new SimpleClass(),
        new PSObject(new SimpleClass()),
        typeof(SimpleClass),
        new PSObject(typeof(SimpleClass)),
    ];

    [Theory]
    [MemberData(nameof(Unregistered_TheoryData))]
    public void Unregistered_PassesThru(object? instance)
    {
        var fallbackType = typeof(ConcreteBaseClass);
        Reflector reflector = new();
        reflector.Register([fallbackType], [new("Method1")]);
        Assert.Multiple(
            () => Assert.Same(instance, reflector.Expose(instance)),
            () => Assert.Same(instance, reflector.Expose(instance, fallbackType)));
    }

    [Fact]
    public void Exposed_ReturnsNewExposed()
    {
        var fallbackType = typeof(ConcreteBaseClass);
        Reflector reflector = new();
        reflector.Register([fallbackType], [new("Method1")]);
        reflector.Register([typeof(SimpleClass)], [new("Method1")]);

        object instance1 = new SimpleClass();
        var pso1 = Assert.IsType<PSObject>(reflector.Expose(instance1));
        var psbm1 = Assert.Single(pso1.Members.OfType<IPSBaseMemberInfo>());
        Assert.Same(instance1, pso1.BaseObject);

        object instance2 = new DerivedClass();
        var pso2 = Assert.IsType<PSObject>(reflector.Expose(instance2, fallbackType));
        var psbm2 = Assert.Single(pso2.Members.OfType<IPSBaseMemberInfo>());
        Assert.Same(instance2, pso2.BaseObject);

        Assert.Multiple(
            () =>
            {
                var pso = Assert.IsType<PSObject>(reflector.Expose(pso1));
                Assert.NotSame(pso1, pso);
                Assert.Same(instance1, pso.BaseObject);
                var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
                Assert.Equal(psbm1, psbm, IPSBaseMemberInfoComparer.Instance);
            },
            () =>
            {
                var pso = Assert.IsType<PSObject>(reflector.Expose(pso1, fallbackType));
                Assert.NotSame(pso1, pso);
                Assert.Same(instance1, pso.BaseObject);
                var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
                Assert.Equal(psbm1, psbm, IPSBaseMemberInfoComparer.Instance);
            },
            () =>
            {
                var pso = Assert.IsType<PSObject>(reflector.Expose(pso2, fallbackType));
                Assert.NotSame(pso2, pso);
                Assert.Same(instance2, pso.BaseObject);
                var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
                Assert.Equal(psbm2, psbm, IPSBaseMemberInfoComparer.Instance);
            });
    }

    private static readonly string[] s_SimpleClassStaticMemberNames = ["ConstField", "StaticField", "StaticProperty", "StaticMethod1"];
    private static readonly string[] s_SimpleClassInstanceMemberNames = ["Field", "Property", "Method1"];

    [Fact]
    public void Expose_Instance_Succeeds()
    {
        var fallbackType = typeof(ConcreteBaseClass);
        var type = typeof(SimpleClass);
        MemberDescriptor[] members =
        [
            new() { Name = ".ctor", ParamCount = 2},
            .. Array.ConvertAll(s_SimpleClassStaticMemberNames, n => new MemberDescriptor(n)),
            .. Array.ConvertAll(s_SimpleClassInstanceMemberNames, n => new MemberDescriptor(n)),
        ];
        HashSet<string> expectedNames =
        [
            .. s_SimpleClassStaticMemberNames,
            .. s_SimpleClassInstanceMemberNames
        ];
        Reflector reflector = new();
        reflector.Register([fallbackType], [new("Method1")]);
        reflector.Register([type], members);
        var instance = Activator.CreateInstance(type);
        Assert.Multiple(
            () =>
            {
                var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
                Assert.Equal(expectedNames, [.. pso.Members.OfType<IPSBaseMemberInfo>().Select(m => m.Name)]);
            },
            () =>
            {
                var pso = Assert.IsType<PSObject>(reflector.Expose(instance, fallbackType));
                Assert.Equal(expectedNames, [.. pso.Members.OfType<IPSBaseMemberInfo>().Select(m => m.Name)]);
            });
    }

    [Fact]
    public void Expose_Type_Succeeds()
    {
        var fallbackType = typeof(ConcreteBaseClass);
        var type = typeof(SimpleClass);
        MemberDescriptor[] members =
        [
            new() { Name = ".ctor", ParamCount = 2},
            .. Array.ConvertAll(s_SimpleClassStaticMemberNames, n => new MemberDescriptor(n)),
            .. Array.ConvertAll(s_SimpleClassInstanceMemberNames, n => new MemberDescriptor(n)),
        ];
        HashSet<string> expectedNames =
        [
            "new",
            .. s_SimpleClassStaticMemberNames,
        ];
        Reflector reflector = new();
        reflector.Register([fallbackType], [new("Method1")]);
        reflector.Register([type], members);
        Assert.Multiple(
            () =>
            {
                var pso = Assert.IsType<PSObject>(reflector.Expose(type));
                Assert.Equal(expectedNames, [.. pso.Members.OfType<IPSBaseMemberInfo>().Select(m => m.Name)]);
            },
            () =>
            {
                var pso = Assert.IsType<PSObject>(reflector.Expose(type, fallbackType));
                Assert.Equal(expectedNames, [.. pso.Members.OfType<IPSBaseMemberInfo>().Select(m => m.Name)]);
            });
    }

    [Fact]
    public void Expose_WithFallbackType_Succeeds()
    {
        var fallbackType = typeof(ConcreteBaseClass);
        var member = new MemberDescriptor("Method1");
        Reflector reflector = new();
        reflector.Register([fallbackType], [member]);
        var instance = new DerivedClass();
        Assert.IsNotType<PSObject>(reflector.Expose(instance));
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance, fallbackType));
        var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Equal(member.Name, psbm.Name);
    }
}
