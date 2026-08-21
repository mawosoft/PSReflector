// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation.Internal;
using InvocationTestFixture.Internal;

namespace InvocationTestFixture.Logging;

public sealed class InvocationLog
{
    private readonly List<InvocationEntry> _invocations = [];
    private InvocationEntry? _currentInvocation;
    public object Instance { get; }
    public InvocationMode InvocationMode { get; }
    public IReadOnlyList<InvocationEntry> Invocations => _invocations;

    internal InvocationLog(object instance, InvocationMode invocationMode)
    {
        ArgumentNullException.ThrowIfNull(instance);
        if (invocationMode == default || !Enum.IsDefined(typeof(InvocationMode), invocationMode))
        {
            throw new ArgumentOutOfRangeException(nameof(invocationMode));
        }
        Instance = instance;
        InvocationMode = invocationMode;
    }
    internal void BeginInvocation(string methodName, object?[] args)
    {
        if (_currentInvocation is not null) EndInvocation(AutomationNull.Value, null);
        _currentInvocation = new InvocationEntry(this, methodName, args);
        _invocations.Add(_currentInvocation);
    }

    internal void EndInvocation(object? retval, object?[]? args)
    {
        if (_currentInvocation is null) ThrowNoCurrentInvocation();
        _currentInvocation.EndInvocation(retval, args);
        _currentInvocation = null;
    }

    internal void LogEnter(string methodName, object?[] args)
    {
        if (_currentInvocation is null) ThrowNoCurrentInvocation();
        _currentInvocation.LogEnter(methodName, args);
    }

    internal void LogExit(object? retval, object?[]? args)
    {
        if (_currentInvocation is null) ThrowNoCurrentInvocation();
        _currentInvocation.LogExit(retval, args);
    }

    [DoesNotReturn]
    private static void ThrowNoCurrentInvocation()
    {
        throw new InvalidOperationException("No current invocation.");
    }
}
