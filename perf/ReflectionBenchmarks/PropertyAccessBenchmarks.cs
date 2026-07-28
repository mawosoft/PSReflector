// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace ReflectionBenchmarks;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

public class PropertyAccessBenchmarks : FieldOrPropertyAccessBenchmarks<PropertyInfo>
{
    private readonly Version _typedProperty2Argument = new(3, 2, 1);

    public static IEnumerable<object> TypedDelegate_Get_Arguments()
    {
        foreach (var member in TestClass.Properties)
        {
            var getter = member.MemberInfo.GetMethod ?? throw new InvalidOperationException();
            var accessor = AccessorGenerator.CreateTypedDelegate(getter);
            var target = new TestClass();
            target.InitMembers();
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }

    public static IEnumerable<object> TypedDelegate_Set_Arguments()
    {
        foreach (var member in TestClass.Properties)
        {
            var setter = member.MemberInfo.SetMethod ?? throw new InvalidOperationException();
            var accessor = AccessorGenerator.CreateTypedDelegate(setter);
            var target = new TestClass();
            target.InitMembers();
            yield return TestClassMemberAccess.Create(member, target, accessor);
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public object? Reflection_Get(TestClassMemberAccess<PropertyInfo, TestClass, Action?> args)
    {
        return args.MemberInfo.GetValue(args.Target);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public void Reflection_Set(TestClassMemberAccess<PropertyInfo, TestClass, Action?> args)
    {
        args.MemberInfo.SetValue(args.Target, args.Arguments);
    }

    [Benchmark]
    [ArgumentsSource(nameof(TypedDelegate_Get_Arguments))]
    public bool TypedDelegate_Get(TestClassMemberAccess<PropertyInfo, TestClass, Delegate> args)
    {
        return args.Id switch
        {
            1 => ((Func<TestClass, int>)args.Accessor)(args.Target) == default,
            2 => ((Func<TestClass, Version>)args.Accessor)(args.Target) == default,
            _ => throw new ArgumentOutOfRangeException("args.Id"),
        };
    }

    [Benchmark]
    [ArgumentsSource(nameof(TypedDelegate_Get_Arguments))]
    public unsafe bool TypedDelegate_Get_Unsafe(TestClassMemberAccess<PropertyInfo, TestClass, Delegate> args)
    {
        var d = args.Accessor;
        return args.Id switch
        {
            1 => (*(Func<TestClass, int>*)&d)(args.Target) == default,
            2 => (*(Func<TestClass, Version>*)&d)(args.Target) == default,
            _ => throw new ArgumentOutOfRangeException("args.Id"),
        };
    }

    [Benchmark]
    [ArgumentsSource(nameof(TypedDelegate_Set_Arguments))]
    public void TypedDelegate_Set(TestClassMemberAccess<PropertyInfo, TestClass, Delegate> args)
    {
        switch (args.Id)
        {
            case 1:
                ((Action<TestClass, int>)args.Accessor)(args.Target, 222);
                break;
            case 2:
                ((Action<TestClass, Version>)args.Accessor)(args.Target, _typedProperty2Argument);
                break;
            default:
                throw new ArgumentOutOfRangeException("args.Id");
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(TypedDelegate_Set_Arguments))]
    public unsafe void TypedDelegate_Set_Unsafe(TestClassMemberAccess<PropertyInfo, TestClass, Delegate> args)
    {
        var d = args.Accessor;
        switch (args.Id)
        {
            case 1:
                (*(Action<TestClass, int>*)&d)(args.Target, 222);
                break;
            case 2:
                (*(Action<TestClass, Version>*)&d)(args.Target, _typedProperty2Argument);
                break;
            default:
                throw new ArgumentOutOfRangeException("args.Id");
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public bool Direct_Get(TestClassMemberAccess<PropertyInfo, TestClass, Action?> args)
    {
        return args.Id switch
        {
            1 => args.Target.Property1 == default,
            2 => args.Target.Property2 == default,
            _ => throw new ArgumentOutOfRangeException("args.Id"),
        };
    }

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public void Direct_Set(TestClassMemberAccess<PropertyInfo, TestClass, Action?> args)
    {
        switch (args.Id)
        {
            case 1:
                args.Target.Property1 = 222;
                break;
            case 2:
                args.Target.Property2 = _typedProperty2Argument;
                break;
            default:
                throw new ArgumentOutOfRangeException("args.Id");
        }
    }
}
