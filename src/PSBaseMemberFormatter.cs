// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

// TODO member formatting
// The .NET core functionality is too terse for generic types (e.g. Dictionary`2).
// The SMA functionality is based on:
// - Microsoft.PowerShell.ToStringCodeMethods.Type
// - System.Management.Automation.DotNetAdapter.GetMethodInfoOverloadDefinition
// This is will use the AssemblyQualifiedName for non-public types and thus is too verbose.
// We will use the .NET core functionality for now, but encapsulated in PSBaseMemberFormatter.
internal static class PSBaseMemberFormatter
{
    public static string Format(Type type)
    {
        return type.ToString();
    }
    public static void Format(StringBuilder sb, Type type)
    {
        sb.Append(type.ToString());
    }
    public static string Format(MethodBase method)
    {
        return method.IsStatic ? "static " + method.ToString() : method.ToString();
    }
    public static void Format(StringBuilder sb, MethodBase method)
    {
        if (method.IsStatic) sb.Append("static ");
        sb.Append(method.ToString());
    }
}
