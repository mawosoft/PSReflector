// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mawosoft.PSReflector;

internal static class PSMemberInfoExtensions
{
    // TODO Linq delegate. See benchmarks
    private static readonly FieldInfo s_psMemberInfoInstanceField =
        typeof(PSMemberInfo).GetField("instance", BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new MissingFieldException(nameof(PSMemberInfo), "instance");

    public static object? GetInstance(this PSMemberInfo @this) => s_psMemberInfoInstanceField.GetValue(@this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? GetBaseObject(this PSMemberInfo @this)
    {
        var baseObject = GetInstance(@this);
        if (baseObject is not PSObject pso) return baseObject;
        return pso.BaseObject;
    }
}

internal static class MethodBaseInvoker
{
    private static readonly MethodInfo s_dotNetAdapterGetMethodInformationArray;
    private static readonly MethodInfo s_dotNetAdapterAuxiliaryConstructorInvoke;
    private static readonly MethodInfo s_dotNetAdapterAuxiliaryMethodInvoke;
    private static readonly MethodInfo s_adapterGetBestMethodAndArguments;

    static MethodBaseInvoker()
    {
        const BindingFlags bfs = BindingFlags.NonPublic | BindingFlags.Static;
        Assembly smaAssembly = typeof(PSObject).Assembly;
        Type tiMethodInformation = smaAssembly.GetType("System.Management.Automation.MethodInformation");
        Type tiMethodInformationArray = tiMethodInformation.MakeArrayType();
        Type tiDotNetAdapter = smaAssembly.GetType("System.Management.Automation.DotNetAdapter");
        s_dotNetAdapterGetMethodInformationArray = tiDotNetAdapter.GetMethod("GetMethodInformationArray", bfs);
        s_dotNetAdapterAuxiliaryConstructorInvoke = tiDotNetAdapter.GetMethod("AuxiliaryConstructorInvoke", bfs);
        s_dotNetAdapterAuxiliaryMethodInvoke = tiDotNetAdapter.GetMethod("AuxiliaryMethodInvoke", bfs);
        Type tiAdapter = smaAssembly.GetType("System.Management.Automation.Adapter");
        //var x1 = tiAdapter.GetMember("GetBestMethodAndArguments", bfs);
        s_adapterGetBestMethodAndArguments = tiAdapter.GetMethod("GetBestMethodAndArguments", bfs, binder: null, [typeof(string), tiMethodInformationArray, typeof(object[]), typeof(object[]).MakeByRefType()], modifiers: null);
    }

    public static object GetMethodInformationArray(MethodBase method)
    {
        MethodBase[] methods = [method];
        return s_dotNetAdapterGetMethodInformationArray.Invoke(null, [methods]);
    }

    public static object InvokeMethod(object? target, MethodInfo method, object methodInformationArray, object[] arguments)
    {
        // TODO This fails for SimpleStruct instance methods, most likely due to struct copy.
        //      (The original struct has no caller info afterwards)
        //      The dynamic IL code uses Unbox_Any (copies value) instead of Unbox (copies value ref)
        // TODO There might be better suited methods on DotNetAdapter, like
        //      MethodInvokeDotNet(string methodName, object target, MethodInformation[] methodInformation, PSMethodInvocationConstraints invocationConstraints, object[] arguments)
        object[] newArguments;
        object[] adapterArguments = [method.Name, methodInformationArray, arguments, null!];
        s_adapterGetBestMethodAndArguments.Invoke(null, adapterArguments);
        newArguments = (object[])adapterArguments[3];
        if (!method.IsStatic && method.DeclaringType.IsValueType)
        {
            // TODO temp test. This doesn't deal with params Something[]
            var argsParam = Expression.Parameter(typeof(object[]));
            var instanceParam = Expression.Parameter(typeof(object));
            var paramInfos = method.GetParameters();
            var callArgs = new Expression[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; i++)
            {
                var indexExpr = Expression.Constant(i);
                var argAccess = Expression.ArrayIndex(argsParam, indexExpr);
                var castArg = Expression.Convert(argAccess, paramInfos[i].ParameterType);
                callArgs[i] = castArg;
            }
            var instance = Expression.Unbox(instanceParam, method.DeclaringType);
            var callExpr = Expression.Call(instance, method, callArgs);
            Expression body;
            if (method.ReturnType == typeof(void))
            {
                var nullConst = Expression.Constant(null);
                body = Expression.Block(callExpr, nullConst);
            }
            else
            {
                body = Expression.Convert(callExpr, typeof(object));
            }
            var lamda = Expression.Lambda<Func<object, object[], object>>(body, instanceParam, argsParam);
            var compiled = lamda.Compile();
            var result = compiled(target!, newArguments);
            if (method.ReturnType == typeof(void)) return AutomationNull.Value;
            return result;
        }
        object methodInformation = ((object[])methodInformationArray)[0];
        return s_dotNetAdapterAuxiliaryMethodInvoke.Invoke(null, [target, newArguments, methodInformation, arguments]);
    }
}
