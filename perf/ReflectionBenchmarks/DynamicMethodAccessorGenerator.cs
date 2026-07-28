// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionBenchmarks;

#if !NET
#pragma warning disable IDE0370 // Remove unnecessary suppression
#endif

internal sealed class DynamicMethodAccessorGenerator : AccessorGenerator
{
    public override Func<object?, object?> CreateFieldGetter(FieldInfo memberInfo)
    {
        var dm = new DynamicMethod("", typeof(object), [typeof(object)], restrictedSkipVisibility: true);
        var il = dm.GetILGenerator();
        if (memberInfo.IsStatic)
        {
            il.Emit(OpCodes.Ldsfld, memberInfo);
        }
        else
        {
            il.Emit(OpCodes.Ldarg_0);
            Type instanceType = memberInfo.DeclaringType!;
            il.Emit(instanceType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, instanceType);
            il.Emit(OpCodes.Ldfld, memberInfo);
        }
        Type fieldType = memberInfo.FieldType;
        if (fieldType.IsValueType)
        {
            il.Emit(OpCodes.Box, fieldType);
        }
        il.Emit(OpCodes.Ret);
        return (Func<object?, object?>)dm.CreateDelegate(typeof(Func<object?, object?>));
    }

    public override Action<object?, object?> CreateFieldSetter(FieldInfo memberInfo)
    {
        var dm = new DynamicMethod("", null, [typeof(object), typeof(object)], restrictedSkipVisibility: true);
        var il = dm.GetILGenerator();
        bool isStatic = memberInfo.IsStatic;
        if (!isStatic)
        {
            il.Emit(OpCodes.Ldarg_0);
            Type instanceType = memberInfo.DeclaringType!;
            il.Emit(instanceType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, instanceType);
        }
        il.Emit(OpCodes.Ldarg_1);
        Type fieldType = memberInfo.FieldType;
        il.Emit(fieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, fieldType);
        il.Emit(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, memberInfo);
        il.Emit(OpCodes.Ret);
        return (Action<object?, object?>)dm.CreateDelegate(typeof(Action<object?, object?>));
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
