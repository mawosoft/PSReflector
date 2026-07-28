// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace ReflectionBenchmarks;

public abstract class FieldOrPropertyAccessBenchmarks<T> : MemberAccessArgumentsProvider<T> where T : MemberInfo
{
    [Benchmark]
    [ArgumentsSource(nameof(LinqExpression_Get_Arguments))]
    public object? LinqExpression_Get(TestClassMemberAccess<T, TestClass, Func<object?, object?>> args)
    {
        return args.Accessor(args.Target);
    }

    [Benchmark]
    [ArgumentsSource(nameof(LinqExpression_Set_Arguments))]
    public void LinqExpression_Set(TestClassMemberAccess<T, TestClass, Action<object?, object?>> args)
    {
        args.Accessor(args.Target, args.Arguments);
    }

    [Benchmark]
    [ArgumentsSource(nameof(DynamicMethod_Get_Arguments))]
    public object? DynamicMethod_Get(TestClassMemberAccess<T, TestClass, Func<object?, object?>> args)
    {
        return args.Accessor(args.Target);
    }

    [Benchmark]
    [ArgumentsSource(nameof(DynamicMethod_Set_Arguments))]
    public void DynamicMethod_Set(TestClassMemberAccess<T, TestClass, Action<object?, object?>> args)
    {
        args.Accessor(args.Target, args.Arguments);
    }

    [Benchmark]
    [ArgumentsSource(nameof(PSProperty_Arguments))]
    public object? PSProperty_Get(TestClassMemberAccess<T, PSObject, PSPropertyInfo> args)
    {
        return args.Accessor.Value;
    }

    [Benchmark]
    [ArgumentsSource(nameof(PSProperty_Arguments))]
    public void PSProperty_Set(TestClassMemberAccess<T, PSObject, PSPropertyInfo> args)
    {
        args.Accessor.Value = args.Arguments;
    }

    [Benchmark]
    [ArgumentsSource(nameof(PSReflector_Arguments))]
    public object? PSReflector_Get(TestClassMemberAccess<T, PSObject, PSPropertyInfo> args)
    {
        return args.Accessor.Value;
    }

    [Benchmark]
    [ArgumentsSource(nameof(PSReflector_Arguments))]
    public void PSReflector_Set(TestClassMemberAccess<T, PSObject, PSPropertyInfo> args)
    {
        args.Accessor.Value = args.Arguments;
    }
}
