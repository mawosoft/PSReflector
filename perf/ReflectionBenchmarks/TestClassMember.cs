// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;

namespace ReflectionBenchmarks;

public class TestClassMember<T> where T : MemberInfo
{
    public readonly T MemberInfo;
    public readonly int Id;
    public readonly object? Arguments;

    public TestClassMember(T? memberInfo, int id, object? arguments)
    {
        MemberInfo = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
        Id = id;
        Arguments = arguments;
    }

    public TestClassMember(TestClassMember<T> other)
    {
        MemberInfo = other.MemberInfo;
        Id = other.Id;
        Arguments = other.Arguments;
    }

    public override string ToString()
    {
        Type? returnType = MemberInfo switch
        {
            FieldInfo mi => mi.FieldType,
            PropertyInfo mi => mi.PropertyType,
            MethodInfo mi => mi.ReturnType,
            ConstructorInfo mi => mi.DeclaringType,
            _ => null,
        };
        string text = returnType is null ? "null" : returnType == typeof(void) ? "void" : returnType.IsValueType ? "struct" : "class";
        if (MemberInfo is MethodBase mb)
        {
            var parameters = mb.GetParameters();
            if (parameters.Length == 0)
            {
                text += " ()";
            }
            else
            {
                bool hasValueTypes = false;
                bool hasClassTypes = false;
                foreach (var p in parameters)
                {
                    if (p.ParameterType.IsValueType)
                        hasValueTypes = true;
                    else
                        hasClassTypes = true;
                }
                string args = hasValueTypes && hasClassTypes ? "mixed" : hasValueTypes ? "struct" : "class";
                text += $" ({parameters.Length}x {args})";
            }
        }
        return text;
    }
}
