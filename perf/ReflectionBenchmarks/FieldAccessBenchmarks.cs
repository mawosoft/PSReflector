// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace ReflectionBenchmarks;

public class FieldAccessBenchmarks : FieldOrPropertyAccessBenchmarks<FieldInfo>
{
    private readonly Version _typedField2Argument = new(3, 2, 1);

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public object? Reflection_Get(TestClassMemberAccess<FieldInfo, TestClass, Action?> args)
    {
        return args.MemberInfo.GetValue(args.Target);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public void Reflection_Set(TestClassMemberAccess<FieldInfo, TestClass, Action?> args)
    {
        args.MemberInfo.SetValue(args.Target, args.Arguments);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public bool Direct_Get(TestClassMemberAccess<FieldInfo, TestClass, Action?> args)
    {
        return args.Id switch
        {
            1 => args.Target.Field1 == default,
            2 => args.Target.Field2 == default,
            _ => throw new ArgumentOutOfRangeException("args.Id"),
        };
    }

    [Benchmark]
    [ArgumentsSource(nameof(Reflection_Arguments))]
    public void Direct_Set(TestClassMemberAccess<FieldInfo, TestClass, Action?> args)
    {
        switch (args.Id)
        {
            case 1:
                args.Target.Field1 = 222;
                break;
            case 2:
                args.Target.Field2 = _typedField2Argument;
                break;
            default:
                throw new ArgumentOutOfRangeException("args.Id");
        }
    }
}
