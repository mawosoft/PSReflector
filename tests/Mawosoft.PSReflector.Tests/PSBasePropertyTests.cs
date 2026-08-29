// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class PSBasePropertyTests
{
    private const string TestMember = nameof(TestMember);
    private const string DefaultMember = "Property";

    private static readonly string[] s_readOnlyMember = ["StaticReadOnlyProperty", "ReadOnlyProperty"];
    private static readonly string[] s_readWriteMember = ["StaticProperty", "Property"];

    public static readonly TheoryData<Type> Type_TheoryData = [typeof(SimpleClass), typeof(SimpleStruct)];
    public static readonly MatrixTheoryData<Type, string> Type_NullEmpty_MatrixData = new([typeof(SimpleClass), typeof(SimpleStruct)], [null!, ""]);
    public static readonly MatrixTheoryData<Type, bool, string> Type_Bool_ReadWriteMember_MatrixData = new([typeof(SimpleClass), typeof(SimpleStruct)], [true, false], s_readWriteMember);
    public static readonly MatrixTheoryData<Type, string> Type_Member_MatrixData = new([typeof(SimpleClass), typeof(SimpleStruct)], [.. s_readOnlyMember, .. s_readWriteMember]);
    public static readonly MatrixTheoryData<Type, string> Type_ReadOnlyMember_MatrixData = new([typeof(SimpleClass), typeof(SimpleStruct)], s_readOnlyMember);
    public static readonly MatrixTheoryData<Type, string> Type_ReadWriteMember_MatrixData = new([typeof(SimpleClass), typeof(SimpleStruct)], s_readWriteMember);

    private static bool IsStaticMemberName(string memberName)
        => memberName.StartsWith("Static", StringComparison.Ordinal);

    private static bool IsReadOnlyMemberName(string memberName)
        => Array.IndexOf(s_readOnlyMember, memberName) >= 0;

    private static PSObject SetupTestMember(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        var psm = new PSBaseProperty(pi, TestMember, true);
        object? instance = Activator.CreateInstance(type);
        Assert.IsType(type, instance);
        var pso = Reflector.PSObjectAsPSObject2Delegate(instance, true);
        pso.Members.Add(psm);
        var psm2 = pso.Properties[TestMember];
        Assert.Equal(psm, (IPSBaseMemberInfo)psm2, IPSBaseMemberInfoComparer.Instance);
        Assert.Same(instance, pso.BaseObject);
        return pso;
    }

    [Fact]
    public void Ctor_NullInfo_Throws()
    {
        Assert.Throws<ArgumentNullException>("info", () => _ = new PSBaseProperty(null!, null, false));
    }

    [Theory]
    [MemberData(nameof(Type_NullEmpty_MatrixData))]
    public void Ctor_NullOrEmptyName_Defaults(Type type, string name)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(DefaultMember);
        var psm = new PSBaseProperty(pi, name, false);
        Assert.Equal(pi.Name, psm.Name);
    }

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Ctor_SetsName(Type type)
    {
        var name = "Foo";
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(DefaultMember);
        var psm = new PSBaseProperty(pi, name, false);
        Assert.Equal(name, psm.Name);
    }

    [Theory]
    [MemberData(nameof(Type_Bool_ReadWriteMember_MatrixData))]
    public void Ctor_SetsCanWrite(Type type, bool canWrite, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        var psm = new PSBaseProperty(pi, null, canWrite);
        Assert.Equal(canWrite, psm.IsSettable);
        Assert.Equal(canWrite, ((IPSBaseMemberInfo)psm).CanWrite);
    }

    [Theory]
    [MemberData(nameof(Type_ReadOnlyMember_MatrixData))]
    public void Ctor_IgnoresCanWrite_IfReadOnly(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        var psm = new PSBaseProperty(pi, null, true);
        Assert.False(psm.IsSettable);
        Assert.False(((IPSBaseMemberInfo)psm).CanWrite);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_IsGettable_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        var psm = new PSBaseProperty(pi, null, false);
        Assert.True(psm.IsGettable);
    }

    // public void Get_IsSettable_Succeeds()
    // see Ctor_SetsCanWrite

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Get_MemberType_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(DefaultMember);
        var psm = new PSBaseProperty(pi, null, false);
        Assert.Equal(PSMemberTypes.CodeProperty, psm.MemberType);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_TypeNameOfValue_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        var psm = new PSBaseProperty(pi, null, false);
        Assert.Equal(pi.PropertyType.ToString(), psm.TypeNameOfValue);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void ToString_Succeeds(Type type, string memberName)
    {
        string prefix = IsStaticMemberName(memberName) ? "static " : "";
        string suffix = IsReadOnlyMemberName(memberName) ? "{get;}" : "{get;set;}";
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        var psm = new PSBaseProperty(pi, null, true);
        var result = psm.ToString();
        Assert.StartsWith(prefix, result, StringComparison.Ordinal);
        Assert.EndsWith(suffix, result, StringComparison.Ordinal);
        Assert.Contains(" " + memberName + " ", result, StringComparison.Ordinal);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Copy_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        var psm = new PSBaseProperty(pi, "Foo", true);
        var psm2 = psm.Copy();
        Assert.NotSame(psm, psm2);
        Assert.Equal(psm, (IPSBaseMemberInfo)psm2, IPSBaseMemberInfoComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_Value_Succeeds(Type type, string memberName)
    {
        PSObject pso = SetupTestMember(type, memberName);
        dynamic dynpso = pso;
        var accessor = Assert.IsType<IMemberAccessor>(pso.BaseObject, exactMatch: false);

        int expected = accessor.SetMemberValue(memberName, 0);
        int actual = dynpso.TestMember;
        Assert.Equal(expected, actual);

        expected = accessor.SetMemberValue(memberName, 42);
        actual = dynpso.TestMember;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Type_ReadWriteMember_MatrixData))]
    public void Set_Value_Succeeds(Type type, string memberName)
    {
        PSObject pso = SetupTestMember(type, memberName);
        dynamic dynpso = pso;
        var accessor = Assert.IsType<IMemberAccessor>(pso.BaseObject, exactMatch: false);
        Assert.Equal(0, accessor.SetMemberValue(memberName, 0));
        Assert.Multiple(
            () =>
            {
                int expected = 42;
                dynpso.TestMember = expected;
                int actual = accessor.GetMemberValue<int>(memberName);
                Assert.Equal(expected, actual);
            },
            () =>
            {
                int expected = 24;
                dynpso.TestMember = $"{expected}";
                int actual = accessor.GetMemberValue<int>(memberName);
                Assert.Equal(expected, actual);
            });
    }

    [Theory]
    [MemberData(nameof(Type_ReadOnlyMember_MatrixData))]
    public void Set_Value_ReadOnlyThrows(Type type, string memberName)
    {
        PSObject pso = SetupTestMember(type, memberName);
        dynamic dynpso = pso;
        Assert.Throws<MemberAccessException>(() => dynpso.TestMember = 42);
    }

    // IPSBaseMemberInfo

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_MemberInfo_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        IPSBaseMemberInfo psbm = new PSBaseProperty(pi, null, false);
        Assert.Equal(pi, psbm.MemberInfo);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_ReturnType_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        IPSBaseMemberInfo psbm = new PSBaseProperty(pi, null, false);
        Assert.Equal(pi.PropertyType, psbm.ReturnType);
    }

    [Theory]
    [MemberData(nameof(Type_Member_MatrixData))]
    public void Get_IsStatic_Succeeds(Type type, string memberName)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(memberName);
        IPSBaseMemberInfo psbm = new PSBaseProperty(pi, null, false);
        Assert.Equal(IsStaticMemberName(memberName), psbm.IsStatic);
    }

    // public void Get_CanWrite_Succeeds()
    // see Ctor_SetsCanWrite

    [Theory]
    [MemberData(nameof(Type_TheoryData))]
    public void Reflector_Roundtrip_Succeeds(Type type)
    {
        var mih = new MemberInfoHelper(type);
        var pi = mih.GetProperty(DefaultMember);
        IPSBaseMemberInfo psbm = new PSBaseProperty(pi, null, false);
        Assert.Null(psbm.Reflector);
        var rfl = new Reflector();
        psbm.Reflector = rfl;
        Assert.Equal(rfl, psbm.Reflector);
        psbm.Reflector = null;
        Assert.Null(psbm.Reflector);
    }
}
