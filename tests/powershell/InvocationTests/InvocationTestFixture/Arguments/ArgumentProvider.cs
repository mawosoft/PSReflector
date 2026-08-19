// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InvocationTestFixture.Arguments;

// TODO Create arguments based on MethodInfo, maybe with variants.
// - Variants can be controlled via args, e.g. caller passes a sentinel like Type.Missing for default, otherwise arg is passed through as is?
// - For ParamArray, create a sensible number (5?). Caller can skip via trailing $null on LHS of assignment.
// - Consider returning PSObject with note properties for replacement/mutation tests.
public static class ArgumentProvider
{
    /// <summary>
    /// Creates arguments of specified types and counts.
    /// </summary>
    /// <param name="typesAndCounts">
    /// Sequence of types and counts that specifies the arguments to create.<br/>
    /// - Optional first <c>int</c>: Argument position (1..n).<br/>
    /// - <c>null</c>: Creates at least one null argument.<br/>
    /// - <c>Type</c>: Creates at least one argument of the specified type.<br/>
    /// - <c>int</c>: Number of arguments to create with the preceding type.
    ///   Can be omitted if only one argument is needed.
    /// </param>
    public static object?[] Create(params object?[] typesAndCounts)
    {
        typesAndCounts ??= [];
        List<object?> result = [];
        Type? type = null;
        bool hasType = false;
        int position = 1;
        for (int i = 0; i < typesAndCounts.Length; i++)
        {
            var arg = typesAndCounts[i];
            if (arg is null or Type)
            {
                type = (Type?)arg;
                // This allows count of 1 to be omitted.
                result.Add(CreateOne(type, position++));
                hasType = true;
            }
            else
            {
                int count = (int)arg;
                if (count < 1) ThrowForTypesAndCounts();
                if (result.Count == 0)
                {
                    position = count;
                }
                else
                {
                    if (!hasType) ThrowForTypesAndCounts();
                    for (int k = 1; k < count; k++)
                    {
                        result.Add(CreateOne(type, position++));
                    }
                    hasType = false;
                }
            }
        }
        if (result.Count == 0) ThrowForTypesAndCounts();
        return result.ToArray();


        [DoesNotReturn]
        static void ThrowForTypesAndCounts()
        {
            throw new ArgumentException(null, nameof(typesAndCounts));
        }
    }

    private static object? CreateOne(Type? type, int position)
    {
        if (type is null) return null;
        if (type == typeof(EnumArg))
        {
            var arg = (EnumArg)position;
            if (arg is < EnumArg.One or > EnumArg.Ten) arg = EnumArg.Default;
            return arg;
        }
        else if (typeof(IArgument).IsAssignableFrom(type))
        {
            var arg = (IArgument)Activator.CreateInstance(type);
            arg.Name = $"a{position}";
            arg.Value = "new";
            return arg;
        }
        else
        {
            return Activator.CreateInstance(type);
        }
    }
}
