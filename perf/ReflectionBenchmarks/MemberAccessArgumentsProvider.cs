// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Mawosoft.PSReflector;

namespace ReflectionBenchmarks;

public abstract class MemberAccessArgumentsProvider<T> where T : MemberInfo
{
    public IEnumerable<object> Reflection_Arguments()
    {
        foreach (var member in TestClass.GetMembers<T>())
        {
            var target = new TestClass();
            target.InitMembers();
            yield return TestClassMemberAccess.Create(member, target, (Action?)null);
        }
    }

    public IEnumerable<object> LinqExpression_Get_Arguments()
    {
        var generator = new LinqExpressionAccessorGenerator();
        foreach (var member in TestClass.GetMembers<T>())
        {
            var accessor = generator.CreateGetter(member.MemberInfo);
            var target = new TestClass();
            target.InitMembers();
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }

    public IEnumerable<object> LinqExpression_Set_Arguments()
    {
        var generator = new LinqExpressionAccessorGenerator();
        foreach (var member in TestClass.GetMembers<T>())
        {
            var accessor = generator.CreateSetter(member.MemberInfo);
            var target = new TestClass();
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }

    public IEnumerable<object> DynamicMethod_Get_Arguments()
    {
        var generator = new DynamicMethodAccessorGenerator();
        foreach (var member in TestClass.GetMembers<T>())
        {
            var accessor = generator.CreateGetter(member.MemberInfo);
            var target = new TestClass();
            target.InitMembers();
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }

    public IEnumerable<object> DynamicMethod_Set_Arguments()
    {
        var generator = new DynamicMethodAccessorGenerator();
        foreach (var member in TestClass.GetMembers<T>())
        {
            var accessor = generator.CreateSetter(member.MemberInfo);
            var target = new TestClass();
            target.InitMembers();
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }

    public IEnumerable<object> PSProperty_Arguments()
    {
        foreach (var member in TestClass.GetMembers<T>())
        {
            var baseObject = new TestClass();
            baseObject.InitMembers();
            var target = new PSObject(baseObject);
            var accessor = target.Properties[member.MemberInfo.Name];
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }

    public IEnumerable<object> PSReflector_Arguments()
    {
        foreach (var member in TestClass.GetMembers<T>())
        {
            var reflector = new Reflector();
            reflector.Register([typeof(TestClass)], [new MemberDescriptor { MemberInfo = member.MemberInfo, CanWrite = true }]);
            var baseObject = new TestClass();
            baseObject.InitMembers();
            var target = (PSObject)reflector.Expose(baseObject)!;
            var accessor = target.Properties[member.MemberInfo.Name];
            if (accessor is not (PSBaseField or PSBaseProperty)) throw new InvalidOperationException();
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }
}
