// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Reflection;

namespace Mawosoft.PSReflector;

#pragma warning disable CA1815 // Override equals and operator equals on value types

internal readonly struct ArgumentProcessor
{
    private readonly ParamInfo[] _parameters;

    public ArgumentProcessor(MethodBase method)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 0)
        {
            _parameters = [];
        }
        else
        {
            _parameters = new ParamInfo[parameters.Length];
            bool isLast = true;
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                _parameters[i] = new ParamInfo(parameters[i], isLast);
                isLast = false;
            }
        }
    }

    public object?[] PreProcess(object?[]? arguments, out bool postProcess)
    {
        postProcess = false;
        var parameters = _parameters;
        if (arguments is not null && arguments.Length == parameters.Length)
        {
            bool shouldProcess = false;
            for (int i = 0; !shouldProcess && i < arguments.Length; i++)
            {
                var a = arguments[i];
                shouldProcess = parameters[i].IsByRef || (a is not null && a.GetType() != parameters[i].Type);
            }
            if (!shouldProcess) return arguments;
        }
        if (parameters.Length == 0)
        {
            return [];
        }
        arguments ??= [];
        var processed = new object?[parameters.Length];
        Type? paramArrayElementType = parameters[parameters.Length - 1].ParamArrayElementType;
        int fixedArgsCount = paramArrayElementType is null ? parameters.Length : parameters.Length - 1;
        for (int i = 0; i < fixedArgsCount; i++)
        {
            ref var pi = ref parameters[i];
            if (i < arguments.Length)
            {
                var a = arguments[i];
                if (pi.IsByRef)
                {
                    postProcess = true;
                    PSReference? psr = a as PSReference ?? (a as PSObject)?.BaseObject as PSReference;
                    if (psr is not null) a = psr.Value;
                }
                processed[i] = LanguagePrimitives.ConvertTo(a, pi.Type);
            }
            else
            {
                processed[i] = pi.DefaultValue;
            }
        }
        if (paramArrayElementType is not null)
        {
            int varArgsCount = arguments.Length - fixedArgsCount;
            if (varArgsCount <= 0)
            {
                processed[fixedArgsCount] = Array.CreateInstance(paramArrayElementType, 0);
            }
            else if (varArgsCount == 1)
            {
                processed[fixedArgsCount] = LanguagePrimitives.ConvertTo(arguments[fixedArgsCount], parameters[fixedArgsCount].Type);
            }
            else
            {
                var paramArray = Array.CreateInstance(paramArrayElementType, varArgsCount);
                processed[fixedArgsCount] = paramArray;
                for (int i = 0; i < varArgsCount; i++)
                {
                    var a = LanguagePrimitives.ConvertTo(arguments[i + fixedArgsCount], paramArrayElementType);
                    paramArray.SetValue(a, i);
                }
            }
        }
        return processed;
    }

    public void PostProcess(object?[] processed, object?[]? original)
    {
        if (original is null) return;
        var parameters = _parameters;
        int n = Math.Min(parameters.Length, Math.Min(processed.Length, original.Length));
        for (int i = 0; i < n; i++)
        {
            if (!parameters[i].IsByRef) continue;
            var o = original[i];
            PSReference? psr = o as PSReference ?? (o as PSObject)?.BaseObject as PSReference;
            if (psr is not null)
            {
                psr.Value = processed[i];
            }
            else
            {
                original[i] = processed[i];
            }
        }
    }

    private readonly struct ParamInfo
    {
        public Type Type { get; }
        public Type? ParamArrayElementType { get; }
        public object? DefaultValue { get; }
        public bool IsByRef { get; }
        public bool IsOptional { get; }
        public ParamInfo(ParameterInfo info, bool isLast)
        {
            Type = info.ParameterType;
            DefaultValue = Type.Missing;
            if (Type.IsByRef)
            {
                Type = Type.GetElementType();
                IsByRef = true;
            }
            if (info.IsOptional)
            {
                DefaultValue = info.DefaultValue;
                IsOptional = true;
            }
            if (isLast && info.IsDefined(typeof(ParamArrayAttribute)))
            {
                Debug.Assert(!IsByRef && !IsOptional);
                ParamArrayElementType = Type.GetElementType();
            }
        }
    }
}
