// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ReflectionBenchmarks;

#if !NET
#pragma warning disable IDE0370 // Remove unnecessary suppression
#endif

internal sealed class LinqExpressionAccessorGenerator : AccessorGenerator
{
    public override Func<object?, object?> CreateFieldGetter(FieldInfo memberInfo)
    {
        var instanceParam = Expression.Parameter(typeof(object));
        Type instanceType = memberInfo.DeclaringType!;
        Expression? instance = memberInfo.IsStatic
            ? null
            : instanceType.IsValueType
                ? Expression.Unbox(instanceParam, instanceType)
                : Expression.Convert(instanceParam, instanceType);
        var field = Expression.Field(instance, memberInfo);
        return Expression.Lambda<Func<object?, object?>>(Expression.Convert(field, typeof(object)), instanceParam).Compile();
    }

    public override Action<object?, object?> CreateFieldSetter(FieldInfo memberInfo)
    {
        var instanceParam = Expression.Parameter(typeof(object));
        var valueParam = Expression.Parameter(typeof(object));
        Type instanceType = memberInfo.DeclaringType!;
        Expression? instance = memberInfo.IsStatic
            ? null
            : instanceType.IsValueType
                ? Expression.Unbox(instanceParam, instanceType)
                : Expression.Convert(instanceParam, instanceType);
        var field = Expression.Field(instance, memberInfo);
        return Expression.Lambda<Action<object?, object?>>(
            Expression.Assign(field, Expression.Convert(valueParam, memberInfo.FieldType)),
            instanceParam,
            valueParam).Compile();
    }

    public override Func<object?, object?> CreatePropertyGetter(PropertyInfo memberInfo)
    {
        return null!; // TODO
    }

    public override Action<object?, object?> CreatePropertySetter(PropertyInfo memberInfo)
    {
        return null!; // TODO
    }

    public override Func<object?, object?[], object?> CreateInvoker(MethodInfo methodInfo)
    {
        return null!; // TODO
    }
}
