// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections;
using System.Management.Automation;
using System.Reflection;
using InvocationTestFixture.Internal;

namespace InvocationTestFixture;

public static class PSCodeMethodHelper
{
    private static readonly MethodInfo s_miGetMethodInformationArray =
        new StrictType(typeof(PSObject).Assembly, SMA.Types.DotNetAdapter)
        .GetMethod(SMA.Members.GetMethodInformationArray);
    private static readonly FieldInfo s_fi_codeReferenceMethodInformation =
        new StrictType(typeof(PSCodeMethod))
        .GetField(SMA.Members._codeReferenceMethodInformation);
    private static readonly FieldInfo s_fi_useReflection =
        new StrictType(typeof(PSObject).Assembly, SMA.Types.MethodInformation)
        .GetField(SMA.Members._useReflection);

    public static bool? GetUseReflection(PSCodeMethod method)
    {
        var coderef = s_fi_codeReferenceMethodInformation.GetValue(method);
        if (coderef is null) return null;
        return (bool)s_fi_useReflection.GetValue(((object[])coderef)[0]);
    }

    public static void SetUseReflection(PSCodeMethod method, bool value)
    {
        ArgumentNullException.ThrowIfNull(method);
        var coderef = s_fi_codeReferenceMethodInformation.GetValue(method);
        if (coderef is null)
        {
            coderef = s_miGetMethodInformationArray.Invoke(null, [new MethodInfo[] { method.CodeReference }]);
            s_fi_codeReferenceMethodInformation.SetValue(method, coderef);
        }
        s_fi_useReflection.SetValue(((object[])coderef)[0], value);
    }

    public static Hashtable GetUseReflection(PSObject pso)
    {
        ArgumentNullException.ThrowIfNull(pso);
        int trueCount = 0;
        int falseCount = 0;
        int nullCount = 0;
        foreach (var method in pso.Methods)
        {
            if (method is PSCodeMethod codeMethod)
            {
                switch (GetUseReflection(codeMethod))
                {
                    case false: falseCount++; break;
                    case true: trueCount++; break;
                    default: nullCount++; break;
                }
            }
        }
        return new Hashtable(StringComparer.OrdinalIgnoreCase)
        {
            ["true"] = trueCount,
            ["false"] = falseCount,
            ["undefined"] = nullCount
        };
    }

    public static void SetUseReflection(PSObject pso, bool value)
    {
        ArgumentNullException.ThrowIfNull(pso);
        foreach (var method in pso.Methods)
        {
            if (method is PSCodeMethod codeMethod) SetUseReflection(codeMethod, value);
        }
    }

    public static PSObject? AddCodeMethods(object? obj)
    {
        var pso = obj as PSObject;
        var o = pso is not null ? pso.BaseObject : obj;
        if (o is null) return null;
        if (o is Type type)
        {
            o = Activator.CreateInstance(type);
            pso = PSObject.AsPSObject(o);
        }
        else
        {
            type = o.GetType();
            pso ??= PSObject.AsPSObject(o);
        }
        var members = pso.Members;
        foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            var param = mi.GetParameters();
            if (param.Length != 0 && param[0].ParameterType == typeof(PSObject))
            {
                members.Add(new PSCodeMethod(mi.Name, mi));
            }
        }
        return pso;
    }
}
