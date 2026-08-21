// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using InvocationTestFixture.Internal;

namespace InvocationTestFixture.Logging;

// SMA already has an InvocationInfo type, so to avoid confusion...
public sealed class InvocationEntry
{
    private readonly InvocationLog _log;
    private readonly ObjectTracker _tracker = new();
    private readonly ParameterInfo[] _parameterInfos;
    private readonly bool _hasParamArray;
    private readonly List<ArgumentInfo> _argumentInfos = [];
    private InvocationStep _invocationStep = InvocationStep.Before;

    public MethodInfo MethodInfo { get; }
    public IReadOnlyList<ArgumentInfo> ArgumentInfos => _argumentInfos;
    public ArgumentInfo ReturnValue { get; } = new();
    public int ScriptLine { get; private set; }
    public string? ScriptText { get; private set; }
    public IReadOnlyList<object> ScriptErrors { get; private set; } = [];

    internal InvocationEntry(InvocationLog log, string methodName, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(methodName);
        ArgumentNullException.ThrowIfNull(args);
        _log = log;
        MethodInfo = new StrictType(_log.Instance.GetType()).GetMethod(methodName);
        _parameterInfos = MethodInfo.GetParameters();
        if (_parameterInfos.Length != 0)
        {
            if (MethodInfo.IsStatic && _parameterInfos[0].ParameterType == typeof(PSObject))
            {
                // PSCodeMethod with first param PSObject instance
                Array.Copy(_parameterInfos, 1, _parameterInfos, 0, _parameterInfos.Length - 1);
                Array.Resize(ref _parameterInfos, _parameterInfos.Length - 1);
            }
            if (_parameterInfos.Length != 0 && _parameterInfos[_parameterInfos.Length - 1].IsDefined(typeof(ParamArrayAttribute), inherit: false))
            {
                _hasParamArray = true;
            }
        }
        EnsureArgumentInfoCount(_parameterInfos.Length);
        AddArguments(InvocationStep.Before, args, ignoreValueTypeIds: false);
        ExecutionContextProxy.FromDefaultRunspace().DollarErrorVariable?.Clear();
    }

    internal void EndInvocation(object? retval, object?[]? args)
    {
        if (_invocationStep >= InvocationStep.After) ThrowInvalidInvocationSequence();
        _invocationStep = InvocationStep.After;
        ReturnValue.After = new ObjectInfo(GetObjectId(retval, ignoreValueTypeIds: true), retval);
        AddArguments(InvocationStep.After, args, ignoreValueTypeIds: false);
        ArrayList? dollarError = ExecutionContextProxy.FromDefaultRunspace().DollarErrorVariable;
        if (dollarError?.Count > 0) ScriptErrors = dollarError.ToArray();
    }

    internal void LogEnter(string methodName, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(methodName);
        ArgumentNullException.ThrowIfNull(args);
        if (_invocationStep != InvocationStep.Before || MethodInfo.Name != methodName) ThrowInvalidInvocationSequence();
        _invocationStep = InvocationStep.Enter;
        AddArguments(InvocationStep.Enter, args, ignoreValueTypeIds: true);
        CallStackFrame? frame = Runspace.DefaultRunspace?.Debugger?.GetCallStack().FirstOrDefault();
        if (frame is not null)
        {
            ScriptLine = frame.ScriptLineNumber;
            ScriptText = frame.Position?.Text;
        }
    }

    internal void LogExit(object? retval, object?[]? args)
    {
        if (_invocationStep != InvocationStep.Enter) ThrowInvalidInvocationSequence();
        _invocationStep = InvocationStep.Exit;
        ReturnValue.Exit = new ObjectInfo(GetObjectId(retval, ignoreValueTypeIds: true), retval);
        AddArguments(InvocationStep.Exit, args, ignoreValueTypeIds: true);
    }

    private void AddArguments(InvocationStep step, object?[]? args, bool ignoreValueTypeIds)
    {
        if (args is null) return;
        int maxCount = args.Length;
        if (_hasParamArray && args.Length >= _parameterInfos.Length) maxCount--;
        AddArgumentRange(step, args, 0, maxCount, 0, ignoreValueTypeIds);
        if (_hasParamArray)
        {
            int continuationIndex = _parameterInfos.Length - 1;
            if (args.Length == _parameterInfos.Length && PSObject.Unwrap(args[continuationIndex]) is Array paramArray)
            {
                AddArgumentRange(step, paramArray, 0, -1, continuationIndex, ignoreValueTypeIds);
            }
            else
            {
                AddArgumentRange(step, args, continuationIndex, -1, continuationIndex, ignoreValueTypeIds);
            }
        }
    }

    private void AddArgumentRange(
        InvocationStep step,
        Array? args,
        int sourceIndex,
        int maxCount,
        int destinationIndex,
        bool ignoreValueTypeIds)
    {
        if (args is null || args.Length == 0 || maxCount == 0) return;
        int sourceEnd = args.Length;
        if (maxCount > 0)
        {
            sourceEnd = Math.Min(sourceEnd, sourceIndex + maxCount);
        }
        if (sourceIndex >= sourceEnd) return;
        EnsureArgumentInfoCount(destinationIndex + sourceEnd - sourceIndex);
        bool slowArray = false;
        if (!ignoreValueTypeIds)
        {
            if (args.GetType().GetElementType().IsValueType)
            {
                ignoreValueTypeIds = true;
                slowArray = true;
            }
        }
        if (!slowArray && args is object?[] objects)
        {
            for (int i = sourceIndex, k = destinationIndex; i < sourceEnd; i++, k++)
            {
                var arg = PSObject.Unwrap(objects[i]);
                _argumentInfos[k][step] = new ObjectInfo(GetObjectId(arg, ignoreValueTypeIds), arg);
            }
        }
        else
        {
            for (int i = sourceIndex, k = destinationIndex; i < sourceEnd; i++, k++)
            {
                var arg = PSObject.Unwrap(args.GetValue(i));
                _argumentInfos[k][step] = new ObjectInfo(GetObjectId(arg, ignoreValueTypeIds), arg);
            }
        }
    }

    private void EnsureArgumentInfoCount(int count)
    {
        while (_argumentInfos.Count < count) _argumentInfos.Add(new());
    }

    private int GetObjectId(object? obj, bool ignoreValueTypeIds)
    {
        if (obj is null) return (int)SpecialObjectId.NullNull;
        if (ignoreValueTypeIds && obj.GetType().IsValueType) return (int)SpecialObjectId.None;
        return _tracker.GetOrAddId(obj);
    }

    [DoesNotReturn]
    private static void ThrowInvalidInvocationSequence()
    {
        throw new InvalidOperationException("Invalid invocation sequence.");
    }
}
/*
1) Simple call
                               arg nesting level
call(a1, a2, a3)                  0 
invoke(target, @(a1, a2, a3))     1
-or-
$arguments = @(a1, a2, a3)
invoke(target, $arguments)        1

2) ByRef call
                               arg nesting level
call(a1, [ref]a2, a3)             0 
invoke(target, @(a1, a2, a3))     1
-or-
$arguments = @(a1, a2, a3)
invoke(target, $arguments)        1
# copy back?

Should we always treat args as an invoke-like array?
- pro:
  - Internal LogCall receives args as [a1, a2, a3] anyway
  - ByRef may be easier to track for invoke?

3) Params array sample sig: a1, params[] a2
call                nest     invoke
(a1)                0  1     (t, [a1, (cast)[]]) // (t, [a1]) causes paramcount mismatch
(a1, null)                   (t, [a1, null])
(a1, [])            1  2     (t, [a1, (cast)[]])
(a1, pa1)                    (t, [a1, (cast)[pa1]])
(a1, [pa1])                    ditto
(a1, pa1, pa2)               (t, [a1, (cast)[pa1, pa2]])
(a1, [pa1, pa2])               ditto
*/
