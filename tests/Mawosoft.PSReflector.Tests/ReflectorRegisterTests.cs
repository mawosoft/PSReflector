// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class ReflectorRegisterTests
{
    [Fact]
    public void ByMemberInfo_Succeeds()
    {
        var type = typeof(ClassWithOverloads);
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Overload1", [typeof(int), typeof(int)]);
        Reflector reflector = new();
        reflector.Register([type], [new(mi)]);
        object? instance = Activator.CreateInstance(type);
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
        var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Equal(mi, psbm.MemberInfo);
    }

    private sealed class ByName_TheoryData : TheoryData<Type, string, bool, MemberInfo>
    {
        public ByName_TheoryData()
        {
            var type = typeof(SimpleClass);
            var mih = new MemberInfoHelper(type);
            MemberInfo mi;
            string[] names = ["StaticField", "StaticProperty", "StaticMethod1", "Field", "Property", "Method1"];
            foreach (var name in names)
            {
                mi = mih.GetMember("StaticField");
                Add(type, mi.Name.ToUpperInvariant(), false, mi);
            }
            type = typeof(ClassWithExplicitInterface);
            mih = new MemberInfoHelper(type);
            mi = mih.GetMethod("Mawosoft.PSReflector.TestTypes.IExplicitInterface.ExplicitMethod");
            Add(type, mi.Name.ToUpperInvariant(), false, mi);
            type = typeof(ClassWithOverloads);
            mih = new MemberInfoHelper(type);
            mi = mih.GetMethod("IgnoreCase1");
            Add(type, mi.Name, true, mi);
        }
    }

    [Theory]
    [ClassData(typeof(ByName_TheoryData))]
    public void ByName_Succeeds(Type type, string name, bool caseSensitive, MemberInfo expected)
    {
        var member = new MemberDescriptor(name);
        Reflector reflector = new();
        reflector.Register([type], [member], caseSensitive);
        object? instance = Activator.CreateInstance(type);
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
        var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Equal(expected, psbm.MemberInfo);
    }

    private sealed class ByNameAndParamCount_TheoryData : TheoryData<Type, object?, MemberDescriptor, MemberInfo>
    {
        public ByNameAndParamCount_TheoryData()
        {
            var type = typeof(ClassWithOverloads);
            var mih = new MemberInfoHelper(type);
            MemberInfo mi = mih.GetConstructor([typeof(int[])]);
            var member = new MemberDescriptor { Name = ".ctor", ParamCount = 1 };
            Add(type, type, member, mi);
            mi = mih.GetMethod("Overload1", [typeof(int), typeof(int)]);
            member = new MemberDescriptor { Name = "Overload1", ParamCount = 2 };
            Add(type, Activator.CreateInstance(type), member, mi);
        }
    }

    [Theory]
    [ClassData(typeof(ByNameAndParamCount_TheoryData))]
    public void ByNameAndParamCount_Succeeds(
        Type type,
        object? instance,
        MemberDescriptor member,
        MemberInfo expected)
    {
        Reflector reflector = new();
        reflector.Register([type], [member]);
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
        var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Equal(expected, psbm.MemberInfo);
    }

    [Fact]
    public void CumulativeMembers_Succeeds()
    {
        var type = typeof(SimpleClass);
        Reflector reflector = new();
        reflector.Register([type], [new("Method1")]);
        reflector.Register([type], [new("Method2")]);
        object? instance = Activator.CreateInstance(type);
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
        Assert.Equal(2, pso.Members.OfType<IPSBaseMemberInfo>().Count());
        var mih = new MemberInfoHelper(type);
        var psbm = Assert.IsType<IPSBaseMemberInfo>(pso.Members["Method1"], exactMatch: false);
        Assert.Equal(mih.GetMethod("Method1"), psbm.MemberInfo);
        psbm = Assert.IsType<IPSBaseMemberInfo>(pso.Members["Method2"], exactMatch: false);
        Assert.Equal(mih.GetMethod("Method2"), psbm.MemberInfo);
    }

    [Fact]
    public void SharedMember_Succeeds()
    {
        Reflector reflector = new();
        Type[] types = [typeof(ClassWithExplicitInterface), typeof(ClassWithMultipleInterfaces)];
        reflector.Register(
            types,
            [new("Mawosoft.PSReflector.TestTypes.IExplicitInterface.ExplicitMethod")
            {
                PSName = "TestMember"
            }]);
        Assert.Multiple(
            () =>
            {
                var type = types[0];
                object? instance = Activator.CreateInstance(type);
                dynamic pso = Assert.IsType<PSObject>(reflector.Expose(instance));
                Assert.Equal(type.Name, pso.TestMember());
            },
            () =>
            {
                var type = types[1];
                object? instance = Activator.CreateInstance(type);
                dynamic pso = Assert.IsType<PSObject>(reflector.Expose(instance));
                Assert.Equal(type.Name, pso.TestMember());
            });
    }

    private sealed class SetsMemberName_TheoryData : TheoryData<Type, object?, MemberInfo, string?, string>
    {
        public SetsMemberName_TheoryData()
        {
            const string psName = "TestMember";
            var type = typeof(SimpleClass);
            var mih = new MemberInfoHelper(type);
            MemberInfo mi = mih.GetConstructor([typeof(int), typeof(string)]);
            Add(type, type, mi, null, "new");
            Add(type, type, mi, "", "new");
            Add(type, type, mi, psName, psName);
            string[] names = ["Field", "Property", "Method1"];
            foreach (var name in names)
            {
                mi = mih.GetMember(name);
                Add(type, Activator.CreateInstance(type), mi, null, name);
                Add(type, Activator.CreateInstance(type), mi, "", name);
                Add(type, Activator.CreateInstance(type), mi, psName, psName);
            }
        }
    }

    [Theory]
    [ClassData(typeof(SetsMemberName_TheoryData))]
    public void Sets_MemberName(
        Type type,
        object? instance,
        MemberInfo memberInfo,
        string? psName,
        string expectedName)
    {
        Reflector reflector = new();
        reflector.Register([type], [new MemberDescriptor { MemberInfo = memberInfo, PSName = psName }]);
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
        var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Equal(memberInfo, psbm.MemberInfo);
        Assert.Equal(expectedName, psbm.Name);
    }

    [Fact]
    public void Updates_ExistingTypes()
    {
        Reflector reflector = new();
        reflector.Register([typeof(AutoExposeTestClass)], [new("DerivedClassProperty")]);
        var testClass = new AutoExposeTestClass();
        var pso = Assert.IsType<PSObject>(reflector.Expose(testClass));
        var psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Null(psbm.Reflector);
        reflector.Register([typeof(DerivedClass)], [new("Method1")]);
        pso = Assert.IsType<PSObject>(reflector.Expose(testClass));
        psbm = Assert.Single(pso.Members.OfType<IPSBaseMemberInfo>());
        Assert.Same(reflector, psbm.Reflector);
    }
}
