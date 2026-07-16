// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class PSBaseMethodTests
{
    public static readonly TheoryData<Type> Type_TheoryData = [typeof(SimpleClass), typeof(SimpleStruct)];

    public static readonly MatrixTheoryData<Type, string> Type_NullEmpty_MatrixData = new(
        [typeof(SimpleClass), typeof(SimpleStruct)],
        [null!, ""]);

    public static readonly MatrixTheoryData<Type, string> Type_Member_MatrixData = new(
        [typeof(SimpleClass), typeof(SimpleStruct)],
        ["StaticMethod1", "StaticMethod2", "StaticMethod3", "Method1", "Method2", "Method3"]);

    private static bool IsStaticMemberName(string memberName)
        => memberName.StartsWith("Static", StringComparison.Ordinal);

    [Fact]
    public void Ctor_NullInfo_Throws()
    {
        Assert.Throws<ArgumentNullException>("info", () => _ = new PSBaseMethod(null!, null));
    }

    [Theory]
    [MemberData(nameof(Type_NullEmpty_MatrixData))]
    public void Ctor_NullOrEmptyName_Defaults(Type type, string name)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Method1");
        var psm = new PSBaseMethod(mi, name);
        Assert.Equal(mi.Name, psm.Name);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Ctor_SetsName(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Method1");
        var name = "Foo";
        var psm = new PSBaseMethod(mi, name);
        Assert.Equal(name, psm.Name);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_OverloadDefinitions_Succeeds(Type type, string memberName)
    {
        string prefix = IsStaticMemberName(memberName) ? "static " : "";
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(memberName);
        string expected = prefix + mi.ToString();
        var psm = new PSBaseMethod(mi, null);
        Assert.Equal(expected, Assert.Single(psm.OverloadDefinitions));
        // Custom membername does not change result
        psm = new PSBaseMethod(mi, "Foo");
        Assert.Equal(expected, Assert.Single(psm.OverloadDefinitions));
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_MemberType_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Method1");
        var psm = new PSBaseMethod(mi, null);
        Assert.Equal(PSMemberTypes.CodeMethod, psm.MemberType);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_TypeNameOfValue_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Method1");
        var psm = new PSBaseMethod(mi, null);
        Assert.Equal(typeof(PSBaseMethod).FullName, psm.TypeNameOfValue);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void ToString_Succeeds(Type type, string memberName)
    {
        string prefix = IsStaticMemberName(memberName) ? "static " : "";
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(memberName);
        string expected = prefix + mi.ToString();
        var psm = new PSBaseMethod(mi, null);
        Assert.Equal(expected, psm.ToString());
        // Custom membername does not change result
        psm = new PSBaseMethod(mi, "Foo");
        Assert.Equal(expected, psm.ToString());
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Copy_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(memberName);
        var psm = new PSBaseMethod(mi, "Foo");
        var psm2 = psm.Copy();
        Assert.NotSame(psm, psm2);
        Assert.Equal(psm, (IPSBaseMemberInfo)psm2, IPSBaseMemberInfoComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Invoke_Succeeds(Type type)
    {
        Assert.Multiple(
            () =>
            {
                string memberName = "Method1";
                dynamic pso = Setup(type, memberName);
                string retval = pso.TestMember(111, "foo");
                Verify(pso, memberName, retval, [111, "foo"]);
            },
            () =>
            {
                string memberName = "StaticMethod1";
                dynamic pso = Setup(type, memberName);
                string retval = pso.TestMember(111, "foo");
                Verify(pso, memberName, retval, [111, "foo"]);
            },
            () =>
            {
                string memberName = "Method2";
                dynamic pso = Setup(type, memberName);
                int[] p1 = [1, 2, 3];
                string retval = pso.TestMember(p1);
                Verify(pso, memberName, retval, [p1]);
            },
            () =>
            {
                string memberName = "StaticMethod2";
                dynamic pso = Setup(type, memberName);
                int[] p1 = [1, 2, 3];
                string retval = pso.TestMember(p1);
                Verify(pso, memberName, retval, [p1]);
            },
            () =>
            {
                string memberName = "Method3";
                dynamic pso = Setup(type, memberName);
                string[] p1 = ["foo", "bar", "buzz"];
                string retval = pso.TestMember(p1);
                Verify(pso, memberName, retval, [p1]);
            },
            () =>
            {
                string memberName = "StaticMethod3";
                dynamic pso = Setup(type, memberName);
                string[] p1 = ["foo", "bar", "buzz"];
                string retval = pso.TestMember(p1);
                Verify(pso, memberName, retval, [p1]);
            });

        static PSObject Setup(Type type, string memberName)
        {
            var mih = new MemberInfoHelper(type);
            var mi = mih.GetMethod(memberName);
            var psm = new PSBaseMethod(mi, "TestMember");
            object? instance = Activator.CreateInstance(type);
            Assert.IsType(type, instance);
            var pso = Reflector.PSObjectAsPSObject2Delegate(instance, true);
            pso.Members.Add(psm);
            var psm2 = pso.Methods["TestMember"];
            Assert.Equal(psm, (IPSBaseMemberInfo)psm2, IPSBaseMemberInfoComparer.Instance);
            Assert.Same(instance, pso.BaseObject);
            return pso;
        }

        static void Verify(object pso, string memberName, string retval, object?[] param)
        {
            var info = Assert.IsType<ILastCallInfoProvider>(((PSObject)pso).BaseObject, exactMatch: false);
            var call = IsStaticMemberName(memberName) ? info.LastCallInfoStatic : info.LastCallInfo;
            Assert.NotNull(call);
            Assert.Equal(memberName, call.MemberName);
            Assert.Equal(param, call.Arguments);
            Assert.Equal(memberName, retval);
        }
    }

    // IPSBaseMemberInfo

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_MemberInfo_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Method1");
        IPSBaseMemberInfo psbm = new PSBaseMethod(mi, null);
        Assert.Equal(mi, psbm.MemberInfo);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_ReturnType_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(memberName);
        IPSBaseMemberInfo psbm = new PSBaseMethod(mi, null);
        Assert.Equal(mi.ReturnType, psbm.ReturnType);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_IsStatic_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(memberName);
        IPSBaseMemberInfo psbm = new PSBaseMethod(mi, null);
        Assert.Equal(IsStaticMemberName(memberName), psbm.IsStatic);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_CanWrite_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Method1");
        IPSBaseMemberInfo psbm = new PSBaseMethod(mi, null);
        Assert.False(psbm.CanWrite);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Reflector_Roundtrip_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod("Method1");
        IPSBaseMemberInfo psbm = new PSBaseMethod(mi, null);
        Assert.Null(psbm.Reflector);
        var rfl = new Reflector();
        psbm.Reflector = rfl;
        Assert.Equal(rfl, psbm.Reflector);
        psbm.Reflector = null;
        Assert.Null(psbm.Reflector);
    }
}
