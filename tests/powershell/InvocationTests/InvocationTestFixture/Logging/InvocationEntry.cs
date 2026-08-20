// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using InvocationTestFixture.Internal;

namespace InvocationTestFixture.Logging;

// SMA already has an InvocationInfo type, so to avoid confusion...
public sealed class InvocationEntry
{
    private readonly InvocationLog _log;
    private readonly ObjectTracker _tracker = new();
    private readonly MethodInfo _method;
    private readonly ParameterInfo[] _parameters;
    private readonly bool _hasParamArray;
    private readonly List<ArgumentInfo> _arguments = [];
    private readonly ArgumentInfo _returnValue = new();
    private InvocationStep _invocationStep = InvocationStep.Before;

    public IReadOnlyList<ArgumentInfo> Arguments => _arguments;
    public ArgumentInfo ReturnValue => _returnValue;
    public string Signature => _method.ToString();
    public int ScriptLine { get; private set; }
    public string? ScriptText { get; private set; }
    public IReadOnlyList<string> ScriptErrors { get; private set; } = [];

    /// <summary>
    /// Returns a report of all errors encountered during this invocation, or <c>null</c> if there weren't any.
    /// </summary>
    public string? ReportErrors()
    {
        StringBuilder? sb = null;
        if (_invocationStep != InvocationStep.After)
        {
            sb = new StringBuilder();
            sb.Append("Incomplete. Last recorded step: ").Append(_invocationStep).AppendLine(".");
        }
        if (ScriptErrors.Count != 0)
        {
            sb ??= new StringBuilder();
            sb.AppendLine("Script errors:");
            foreach (var error in ScriptErrors) sb.Append("  ").Append(error);
        }
        // TODO return value
        // TODO arguments
        return sb?.ToString();
    }

    internal InvocationEntry(InvocationLog log, string methodName, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentException.ThrowIfNullOrEmpty(methodName);
        ArgumentNullException.ThrowIfNull(args);
        _log = log;
        _method = new StrictType(_log.Instance.GetType()).GetMethod(methodName);
        if (_method.ReturnType == typeof(void))
        {
            _returnValue.Exit = new ObjectInfo(ObjectId.AutoNull, AutomationNull.Value);
        }
        _parameters = _method.GetParameters();
        if (_parameters.Length != 0)
        {
            if (_method.IsStatic && _parameters[0].ParameterType == typeof(PSObject))
            {
                // PSCodeMethod with first param PSObject instance
                var tmp = _parameters;
                _parameters = new ParameterInfo[tmp.Length - 1];
                Array.Copy(tmp, 1, _parameters, 0, _parameters.Length);
            }
            _hasParamArray = _parameters.HasParamArray;
        }
        EnsureArgumentInfoCount(_parameters.Length);
        AddArguments(InvocationStep.Before, args, ignoreValueTypeIds: false);
        ExecutionContextProxy.FromDefaultRunspace().DollarErrorVariable?.Clear();
    }

    internal void EndInvocation(object? retval, object?[]? args)
    {
        if (_invocationStep >= InvocationStep.After) ThrowInvalidInvocationSequence();
        _invocationStep = InvocationStep.After;
        _returnValue.After = new ObjectInfo(GetObjectId(retval, ignoreValueTypeIds: true), retval);
        AddArguments(InvocationStep.After, args, ignoreValueTypeIds: false);
        ArrayList? dollarError = ExecutionContextProxy.FromDefaultRunspace().DollarErrorVariable;
        if (dollarError?.Count > 0)
        {
            ScriptErrors = Array.ConvertAll(dollarError.ToArray(), o => o?.ToString() ?? "<null>");
        }
    }

    internal void LogEnter(string methodName, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        if (_invocationStep != InvocationStep.Before || _method.Name != methodName) ThrowInvalidInvocationSequence();
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
        _returnValue.Exit = new ObjectInfo(GetObjectId(retval, ignoreValueTypeIds: true), retval);
        AddArguments(InvocationStep.Exit, args, ignoreValueTypeIds: true);
    }

    private void AddArguments(InvocationStep step, object?[]? args, bool ignoreValueTypeIds)
    {
        if (args is null) return;
        int maxCount = args.Length;
        if (_hasParamArray && args.Length >= _parameters.Length) maxCount--;
        AddArgumentRange(step, args, 0, maxCount, 0, ignoreValueTypeIds);
        if (_hasParamArray)
        {
            int continuationIndex = _parameters.Length - 1;
            if (args.Length == _parameters.Length && PSObject.Unwrap(args[continuationIndex]) is Array paramArray)
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
                _arguments[k][step] = new ObjectInfo(GetObjectId(arg, ignoreValueTypeIds), arg);
            }
        }
        else
        {
            for (int i = sourceIndex, k = destinationIndex; i < sourceEnd; i++, k++)
            {
                var arg = PSObject.Unwrap(args.GetValue(i));
                _arguments[k][step] = new ObjectInfo(GetObjectId(arg, ignoreValueTypeIds), arg);
            }
        }
    }

    private void EnsureArgumentInfoCount(int count)
    {
        while (_arguments.Count < count) _arguments.Add(new());
    }

    private ObjectId GetObjectId(object? obj, bool ignoreValueTypeIds)
    {
        if (obj is null) return ObjectId.NullNull;
        if (ignoreValueTypeIds && obj.GetType().IsValueType) return ObjectId.None;
        return _tracker.GetOrAddId(obj);
    }

    [DoesNotReturn]
    private static void ThrowInvalidInvocationSequence()
    {
        throw new InvalidOperationException("Invalid invocation sequence.");
    }
}
