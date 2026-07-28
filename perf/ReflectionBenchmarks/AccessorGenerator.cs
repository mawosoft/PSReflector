// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;

namespace ReflectionBenchmarks;

#if !NET
#pragma warning disable IDE0370 // Remove unnecessary suppression
#endif

internal abstract class AccessorGenerator
{
    public Func<object?, object?> CreateGetter<T>(T memberInfo) where T : MemberInfo
    {
        if (typeof(T) == typeof(FieldInfo))
        {
            return CreateFieldGetter((FieldInfo)(object)memberInfo);
        }
        if (typeof(T) == typeof(PropertyInfo))
        {
            return CreatePropertyGetter((PropertyInfo)(object)memberInfo);
        }
        throw new NotImplementedException();
    }

    public Action<object?, object?> CreateSetter<T>(T memberInfo) where T : MemberInfo
    {
        if (typeof(T) == typeof(FieldInfo))
        {
            return CreateFieldSetter((FieldInfo)(object)memberInfo);
        }
        if (typeof(T) == typeof(PropertyInfo))
        {
            return CreatePropertySetter((PropertyInfo)(object)memberInfo);
        }
        throw new NotImplementedException();
    }

    public static Delegate CreateTypedDelegate(MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        int typeCount = parameters.Length;
        bool isStatic = methodInfo.IsStatic;
        if (!isStatic) typeCount++;
        Type returnType = methodInfo.ReturnType;
        if (returnType != typeof(void)) typeCount++;
        Type delegateType = typeof(Action);
        if (typeCount != 0)
        {
            Type[] types = new Type[typeCount];
            int i = 0;
            if (!isStatic) types[i++] = methodInfo.DeclaringType!;
            for (int k = 0; k < parameters.Length; k++)
            {
                types[i++] = parameters[k].ParameterType;
            }
            if (returnType != typeof(void))
            {
                types[i] = returnType;
                delegateType = typeof(Func<,>).MakeGenericType(types);
            }
            else
            {
                delegateType = typeof(Action<,>).MakeGenericType(types);
            }
        }
        return methodInfo.CreateDelegate(delegateType);
    }

    protected AccessorGenerator() { }

    public abstract Func<object?, object?> CreateFieldGetter(FieldInfo memberInfo);
    public abstract Action<object?, object?> CreateFieldSetter(FieldInfo memberInfo);
    public abstract Func<object?, object?> CreatePropertyGetter(PropertyInfo memberInfo);
    public abstract Action<object?, object?> CreatePropertySetter(PropertyInfo memberInfo);
    public abstract Func<object?, object?[], object?> CreateInvoker(MethodInfo methodInfo);
}
