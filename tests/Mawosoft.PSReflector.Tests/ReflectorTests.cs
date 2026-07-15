// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class ReflectorTests
{
    // TODO Review and move to ReflectorExposeTests

    private static readonly string[] s_SimpleClassStaticMemberNames = ["ConstField", "StaticField", "StaticProperty", "StaticMethod1"];
    private static readonly string[] s_SimpleClassInstanceMemberNames = ["Field", "Property", "Method1"];

    [Fact]
    public void Expose_Instance_Succeeds()
    {
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
        reflector.Register([type], members);
        var instance = Activator.CreateInstance(type);
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
        Assert.Equal(expectedNames, [.. pso.Members.OfType<IPSBaseMemberInfo>().Select(m => m.Name)]);
    }

    [Fact]
    public void Expose_Type_Succeeds()
    {
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
        reflector.Register([type], members);
        var pso = Assert.IsType<PSObject>(reflector.Expose(type));
        Assert.Equal(expectedNames, [.. pso.Members.OfType<IPSBaseMemberInfo>().Select(m => m.Name)]);
    }

    [Fact]
    public void Expose_WithFallbackType_Succeeds()
    {
        var type = typeof(ConcreteBaseClass);
        var member = new MemberDescriptor("Method1");
        Reflector reflector = new();
        reflector.Register([type], [member]);
        var instance = new DerivedClass();
        Assert.IsNotType<PSObject>(reflector.Expose(instance));
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance, type));
        var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Equal(member.Name, psbm.Name);
    }

    [Fact]
    public void AutoExpose_Succeeds()
    {
        // TODO
    }
}
