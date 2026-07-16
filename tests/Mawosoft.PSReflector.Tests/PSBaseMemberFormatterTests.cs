// Copyright (c) Matthias Wolf, Mawosoft.

using System.Reflection;
using System.Text;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class PSBaseMemberFormatterTests
{
    [Fact]
    public void Format_Type_Succeeds()
    {
        var type = typeof(SimpleClass);
        var typeText = type.ToString();
        Assert.Multiple(
            () =>
            {
                Assert.Equal(typeText, PSBaseMemberFormatter.Format(type));
            },
            () =>
            {
                var sb = new StringBuilder();
                const string prefix = "prefix";
                sb.Append(prefix);
                PSBaseMemberFormatter.Format(sb, type);
                Assert.Equal(prefix + typeText, sb.ToString());
            });
    }

    public static readonly TheoryData<MethodBase> MethodBase_TheoryData =
    [
        typeof(SimpleClass).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, binder: null, [typeof(int), typeof(string)], modifiers: null)!,
        typeof(SimpleClass).GetMethod("Method1", BindingFlags.NonPublic | BindingFlags.Instance)!,
        typeof(SimpleClass).GetMethod("StaticMethod3", BindingFlags.NonPublic | BindingFlags.Static)!,
    ];

    [Theory]
    [MemberData(nameof(MethodBase_TheoryData))]
    public void Format_MethodBase_Succeeds(MethodBase method)
    {
        var methodText = method.ToString();
        if (method.IsStatic) methodText = "static " + methodText;
        Assert.Multiple(
            () =>
            {
                Assert.Equal(methodText, PSBaseMemberFormatter.Format(method));
            },
            () =>
            {
                var sb = new StringBuilder();
                const string prefix = "prefix";
                sb.Append(prefix);
                PSBaseMemberFormatter.Format(sb, method);
                Assert.Equal(prefix + methodText, sb.ToString());
            });
    }
}
