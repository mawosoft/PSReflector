// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class PSBaseConstructorTests
{
    public static readonly TheoryData<Type> Type_TheoryData = new(
        [typeof(SimpleClass), typeof(SimpleStruct)]);

    public static readonly MatrixTheoryData<Type, string> Type_NullEmpty_MatrixData = new(
        [typeof(SimpleClass), typeof(SimpleStruct)],
        [null!, ""]);

    [Fact]
    public void Ctor_NullInfo_Throws()
    {
        Assert.Throws<ArgumentNullException>("info", () => _ = new PSBaseConstructor(null!, null));
    }

    [Theory]
    [MemberData(nameof(Type_NullEmpty_MatrixData))]
    public void Ctor_NullOrEmptyName_Defaults(Type type, string name)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        var psm = new PSBaseConstructor(ci, name);
        Assert.Equal("new", psm.Name);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Ctor_SetsName(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        var name = "Foo";
        var psm = new PSBaseConstructor(ci, name);
        Assert.Equal(name, psm.Name);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_OverloadDefinitions_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        var psm = new PSBaseConstructor(ci, null);
        Assert.Equal(ci.ToString(), Assert.Single(psm.OverloadDefinitions));
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_MemberType_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        var psm = new PSBaseConstructor(ci, null);
        Assert.Equal(PSMemberTypes.CodeMethod, psm.MemberType);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_TypeNameOfValue_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        var psm = new PSBaseConstructor(ci, null);
        Assert.Equal(typeof(PSBaseConstructor).FullName, psm.TypeNameOfValue);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void ToString_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        var psm = new PSBaseConstructor(ci, null);
        Assert.Equal(ci.ToString(), psm.ToString());
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Copy_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        var psm = new PSBaseConstructor(ci, null);
        var psm2 = psm.Copy();
        Assert.NotSame(psm, psm2);
        Assert.Equal(psm, (IPSBaseMemberInfo)psm2, IPSBaseMemberInfoComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Invoke_Succeeds(Type type)
    {
        Assert.Multiple(
        [
            () => {
                dynamic pso = Setup(type, [typeof(int), typeof(string)]);
                object? instance = pso.TestMember(111, "foo");
                Verify(type, instance, [111, "foo"]);
            },
            () => {
                dynamic pso = Setup(type, [typeof(int[])]);
                int[] p1 = [1,2,3];
                object? instance = pso.TestMember(p1);
                Verify(type, instance, [p1]);
            },
            () => {
                dynamic pso = Setup(type, [typeof(string[])]);
                string[] p1 = ["foo","bar","buzz"];
                object? instance = pso.TestMember(p1);
                Verify(type, instance, [p1]);
            },
        ]);

        static PSObject Setup(Type type, Type[] paramTypes)
        {
            var mih = new MemberInfoHelper(type);
            var ci = mih.GetConstructor(paramTypes);
            var psm = new PSBaseConstructor(ci, "TestMember");
            var pso = Reflector.PSObjectAsPSObject2Delegate(type, true);
            pso.Members.Add(psm);
            var psm2 = pso.Methods["TestMember"];
            Assert.Equal(psm, (IPSBaseMemberInfo)psm2, IPSBaseMemberInfoComparer.Instance);
            return pso;
        }

        static void Verify(Type type, object? instance, object?[] param)
        {
            Assert.IsType(type, instance);
            var info = Assert.IsType<ILastCallInfoProvider>(instance, exactMatch: false);
            Assert.Equal(".ctor", info.LastCallInfo?.MemberName);
            Assert.Equal(param, info.LastCallInfo?.Arguments);
        }
    }

    // IPSBaseMemberInfo

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_MemberInfo_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        IPSBaseMemberInfo psbm = new PSBaseConstructor(ci, null);
        Assert.Equal(ci, psbm.MemberInfo);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_ReturnType_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        IPSBaseMemberInfo psbm = new PSBaseConstructor(ci, null);
        Assert.Equal(type, psbm.ReturnType);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_IsStatic_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        IPSBaseMemberInfo psbm = new PSBaseConstructor(ci, null);
        Assert.False(psbm.IsStatic);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_CanWrite_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        IPSBaseMemberInfo psbm = new PSBaseConstructor(ci, null);
        Assert.False(psbm.CanWrite);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Reflector_Roundtrip_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var ci = mih.GetConstructor([typeof(int), typeof(string)]);
        IPSBaseMemberInfo psbm = new PSBaseConstructor(ci, null);
        Assert.Null(psbm.Reflector);
        var rfl = new Reflector();
        psbm.Reflector = rfl;
        Assert.Equal(rfl, psbm.Reflector);
        psbm.Reflector = null;
        Assert.Null(psbm.Reflector);
    }
}
