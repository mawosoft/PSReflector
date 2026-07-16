// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using Mawosoft.PSReflector.TestTypes;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace Mawosoft.PSReflector;

public class ReflectorAutoExposeTests
{
    private static object? InvokeTestClassMemberDynamic(PSObject pso, string member, object? value)
    {
        CSharpArgumentInfo[] argInfo =
        [
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
        ];
        if (member.EndsWith("Method", StringComparison.Ordinal))
        {
            var binder = Binder.InvokeMember(CSharpBinderFlags.None, member, null, null, argInfo);
            var site = CallSite<Func<CallSite, PSObject, object?, object?>>.Create(binder);
            return site.Target(site, pso, value);
        }
        else
        {
            var binder = Binder.SetMember(CSharpBinderFlags.None, member, null, argInfo);
            var setterSite = CallSite<Action<CallSite, PSObject, object?>>.Create(binder);
            setterSite.Target(setterSite, pso, value);
            binder = Binder.GetMember(CSharpBinderFlags.None, member, null, [argInfo[0]]);
            var site = CallSite<Func<CallSite, PSObject, object?>>.Create(binder);
            var result = site.Target(site, pso);
            setterSite.Target(setterSite, pso, null);
            return result;
        }
    }

    private static readonly (Type, string)[] s_types_MemberPrefixes =
    [
            (typeof(DerivedClass), "AbstractBaseClass"),
            (typeof(DerivedClass), "DerivedClass"),
            (typeof(ClassWithNonGenericInterface), "INonGenericInterface"),
            (typeof(ClassWithGenericInterface), "IGenericInterface"),
            (typeof(ClassWithExplicitInterface), "IExplicitInterface"),
            (typeof(ClassWithMultipleInterfaces), "INonGenericInterface"),
            (typeof(ClassWithMultipleInterfaces), "IGenericInterface"),
            (typeof(ClassWithMultipleInterfaces), "IExplicitInterface"),
    ];
    private static readonly string[] s_memberTypeSuffixes = ["Field", "Property", "Method"];
    private static readonly MemberDescriptor[] s_testClassMembers = s_types_MemberPrefixes
        .Select(vt => vt.Item2)
        .Distinct()
        .SelectMany(s1 => s_memberTypeSuffixes, (s1, s2) => new MemberDescriptor { Name = s1 + s2, CanWrite = true })
        .ToArray();

    private sealed class AutoExpose_TheoryData : TheoryData<Type, string, string>
    {
        public AutoExpose_TheoryData()
        {
            foreach ((Type type, string prefix) in s_types_MemberPrefixes)
            {
                foreach (var suffix in s_memberTypeSuffixes)
                {
                    Add(type, prefix, suffix);
                }
            }
        }
    }

    [Theory]
    [ClassData(typeof(AutoExpose_TheoryData))]
    public void AutoExpose_Succeeds(Type type, string memberPrefix, string memberSuffix)
    {
        Reflector reflector = new();
        reflector.Register([typeof(AutoExposeTestClass)], s_testClassMembers);
        reflector.Register([type], [new("Method1")]);
        var instance = new AutoExposeTestClass();
        var value = Activator.CreateInstance(type);
        var memberName = memberPrefix + memberSuffix;
        var pso = Assert.IsType<PSObject>(reflector.Expose(instance));
        var result = InvokeTestClassMemberDynamic(pso, memberName, value);
        var pso2 = Assert.IsType<PSObject>(result);
        Assert.Same(value, pso2.BaseObject);
        Assert.Single(pso2.Members.OfType<IPSBaseMemberInfo>());
    }
}
