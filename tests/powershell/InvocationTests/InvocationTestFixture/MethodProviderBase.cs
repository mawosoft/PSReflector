// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;
using System.Runtime.CompilerServices;
using InvocationTestFixture.Logging;

namespace InvocationTestFixture;

public abstract class MethodProviderBase
{
    public InvocationLog Log { get; }

    protected MethodProviderBase(InvocationMode invocationMode)
    {
        Log = new InvocationLog(this, invocationMode);
    }

    public static void DebugBreak()
    {
        if (!System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Launch();
        System.Diagnostics.Debugger.Break();
    }

    public static string[] GetTestMethodNames(Type type)
    {
        if (type is null || !typeof(MethodProviderBase).IsAssignableFrom(type))
        {
            throw new ArgumentException(null, nameof(type));
        }
        return Array.ConvertAll(
            type.GetMethods(BindingFlags.DeclaredOnly
                            | BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.Static),
            mi => mi.Name);
    }

    public string[] GetTestMethodNames() => GetTestMethodNames(GetType());

    public void BeginInvocation(string methodName, object?[] args) => Log.BeginInvocation(methodName, args);
    public void EndInvocation(object?[] args) => Log.EndInvocation(AutomationNull.Value, args);
    public void EndInvocation(object? retval, object?[] args) => Log.EndInvocation(retval, args);

    protected static void LogEnter(object instance, object?[] args, [CallerMemberName] string caller = "")
        => GetThis(instance).Log.LogEnter(caller, args);

    protected static void LogExit(object instance, object?[]? args)
        => GetThis(instance).Log.LogExit(AutomationNull.Value, args);

    protected static void LogExit(object instance, object? retval, object?[]? args)
        => GetThis(instance).Log.LogExit(retval, args);

    private static MethodProviderBase GetThis(object instance)
    {
        if (instance is PSObject pso) instance = pso.BaseObject;
        var @this = instance as MethodProviderBase;
        return @this ?? throw new ArgumentException(null, nameof(instance));
    }
}
