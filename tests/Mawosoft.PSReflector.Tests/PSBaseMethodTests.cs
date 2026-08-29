// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using System.Management.Automation.Internal;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class PSBaseMethodTests
{
    private const string TestMember = nameof(TestMember);
    private const string DefaultMember = "Method1";

    private static readonly string[] s_member =
    [
        "StaticMethod1", "StaticMethod2", "StaticMethod3", "StaticVoidMethod4",
        "Method1", "Method2", "Method3", "VoidMethod4"
    ];

    public static readonly TheoryData<Type> Type_TheoryData = [typeof(SimpleClass), typeof(SimpleStruct)];
    public static readonly MatrixTheoryData<Type, string> Type_NullEmpty_MatrixData = new([typeof(SimpleClass), typeof(SimpleStruct)], [null!, ""]);
    public static readonly MatrixTheoryData<Type, string> Type_Member_MatrixData = new([typeof(SimpleClass), typeof(SimpleStruct)], s_member);

    private static bool IsStaticMemberName(string memberName)
        => memberName.StartsWith("Static", StringComparison.Ordinal);

    private static bool IsVoidMemberName(string memberName)
        => memberName.Contains("Void", StringComparison.Ordinal);

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
        var mi = mih.GetMethod(DefaultMember);
        var psm = new PSBaseMethod(mi, name);
        Assert.Equal(mi.Name, psm.Name);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Ctor_SetsName(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(DefaultMember);
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
        var mi = mih.GetMethod(DefaultMember);
        var psm = new PSBaseMethod(mi, null);
        Assert.Equal(PSMemberTypes.CodeMethod, psm.MemberType);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_TypeNameOfValue_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(DefaultMember);
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
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Invoke_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(memberName);
        var psm = new PSBaseMethod(mi, TestMember);
        object? instance = Activator.CreateInstance(type);
        Assert.IsType(type, instance);
        var pso = Reflector.PSObjectAsPSObject2Delegate(instance, true);
        pso.Members.Add(psm);
        var psm2 = pso.Methods[TestMember];
        Assert.Equal(psm, (IPSBaseMemberInfo)psm2, IPSBaseMemberInfoComparer.Instance);
        Assert.Same(instance, pso.BaseObject);

        var (param, retval) = memberName[memberName.Length - 1] switch
        {
            '1' => Invoker1(pso),
            '2' => Invoker2(pso),
            '3' => Invoker3(pso),
            '4' => Invoker4(pso),
            _ => throw new InvalidOperationException(),
        };

        var info = Assert.IsType<ILastCallInfoProvider>(pso.BaseObject, exactMatch: false);
        var call = IsStaticMemberName(memberName) ? info.LastCallInfoStatic : info.LastCallInfo;
        object? expectedRetval = IsVoidMemberName(memberName) ? AutomationNull.Value : memberName;
        Assert.NotNull(call);
        Assert.Equal(memberName, call.MemberName);
        Assert.Equal(param, call.Arguments);
        Assert.Equal(expectedRetval, retval);

        static (object[] param, object? retval) Invoker1(dynamic pso)
        {
            string retval = pso.TestMember(111, "foo");
            return ([111, "foo"], retval);
        }

        static (object[] param, object? retval) Invoker2(dynamic pso)
        {
            int p1 = 123;
            var p1ref = new PSReference(p1);
            string retval = pso.TestMember(p1ref);
            Assert.Equal(p1 * 2, p1ref.Value);
            return ([p1], retval);
        }
        static (object[] param, object? retval) Invoker3(dynamic pso)
        {
            string[] p1 = ["foo", "bar", "buzz"];
            string retval = pso.TestMember(p1[0], p1[1], p1[2]);
            return ([p1], retval);
        }
        static (object[] param, object? retval) Invoker4(dynamic pso)
        {
            int p1 = 42;
            PSObject retval = pso.TestMember(p1);
            return ([p1], retval);
        }
    }

    // IPSBaseMemberInfo

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_MemberInfo_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(DefaultMember);
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
        var mi = mih.GetMethod(DefaultMember);
        IPSBaseMemberInfo psbm = new PSBaseMethod(mi, null);
        Assert.False(psbm.CanWrite);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Reflector_Roundtrip_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var mi = mih.GetMethod(DefaultMember);
        IPSBaseMemberInfo psbm = new PSBaseMethod(mi, null);
        Assert.Null(psbm.Reflector);
        var rfl = new Reflector();
        psbm.Reflector = rfl;
        Assert.Equal(rfl, psbm.Reflector);
        psbm.Reflector = null;
        Assert.Null(psbm.Reflector);
    }
}
