// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace ReflectionBenchmarks;

// TODO enable benchmarks when generator is implemented
// TODO Add CreateInvoker benchmarks
public class AccessorGenerationBenchmarks
{
    private readonly LinqExpressionAccessorGenerator _linqExpressionGenerator = new();
    private readonly DynamicMethodAccessorGenerator _dynamicMethodGenerator = new();

    public static IEnumerable<object> Fields => TestClass.Fields;
    public static IEnumerable<object> Properties => TestClass.Properties;

    [Benchmark]
    [ArgumentsSource(nameof(Fields))]
    public Delegate LinqExpression_Getter_Field(TestClassMember<FieldInfo> args)
    {
        return _linqExpressionGenerator.CreateFieldGetter(args.MemberInfo);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Fields))]
    public Delegate LinqExpression_Setter_Field(TestClassMember<FieldInfo> args)
    {
        return _linqExpressionGenerator.CreateFieldSetter(args.MemberInfo);
    }

    //[Benchmark]
    //[ArgumentsSource(nameof(Properties))]
    //public Delegate LinqExpression_Getter_Property(TestClassMember<PropertyInfo> args)
    //{
    //    return _linqExpressionGenerator.CreatePropertyGetter(args.MemberInfo);
    //}

    //[Benchmark]
    //[ArgumentsSource(nameof(Properties))]
    //public Delegate LinqExpression_Setter_Property(TestClassMember<PropertyInfo> args)
    //{
    //    return _linqExpressionGenerator.CreatePropertySetter(args.MemberInfo);
    //}

    [Benchmark]
    [ArgumentsSource(nameof(Fields))]
    public Delegate DynamicMethod_Getter_Field(TestClassMember<FieldInfo> args)
    {
        return _dynamicMethodGenerator.CreateFieldGetter(args.MemberInfo);
    }

    //[Benchmark]
    //[ArgumentsSource(nameof(Fields))]
    //public Delegate DynamicMethod_Setter_Field(TestClassMember<FieldInfo> args)
    //{
    //    return _dynamicMethodGenerator.CreateFieldSetter(args.MemberInfo);
    //}

    //[Benchmark]
    //[ArgumentsSource(nameof(Properties))]
    //public Delegate DynamicMethod_Getter_Property(TestClassMember<PropertyInfo> args)
    //{
    //    return _dynamicMethodGenerator.CreatePropertyGetter(args.MemberInfo);
    //}

    //[Benchmark]
    //[ArgumentsSource(nameof(Properties))]
    //public Delegate DynamicMethod_Setter_Property(TestClassMember<PropertyInfo> args)
    //{
    //    return _dynamicMethodGenerator.CreatePropertySetter(args.MemberInfo);
    //}
}
