// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mawosoft.PSReflector;

public sealed partial class Reflector
{
#pragma warning disable CA1716 // Identifiers should not match keywords
    private static class Throw
    {
        public static void IfNullOrEmpty([NotNull] object?[]? array, [CallerArgumentExpression(nameof(array))] string? paramName = null)
        {
            if (array is null) throw new ArgumentNullException(paramName);
            if (array.Length == 0) throw new ArgumentException("Array cannot be empty.", paramName);
            if (Array.IndexOf(array, null) >= 0) throw new ArgumentNullException(paramName, "Array element cannot be null.");
        }

        [DoesNotReturn]
        public static void UnsupportedType(Type type, string paramName)
        {
            throw new ArgumentException(
                $"Type {type} is not supported."
                + " Interfaces, abstract classes, open generics, arrays, pointers,"
                + " reference types, and 'System.Type' itself are not allowed.",
                paramName);
        }

        [DoesNotReturn]
        public static void InvalidMemberDescriptor(Type type, MemberDescriptor descriptor, string paramName)
        {
            throw new ArgumentException($"The {descriptor} for {type} is invalid.", paramName);
        }

        [DoesNotReturn]
        public static void NonmatchingMemberDescriptor(Type type, MemberDescriptor descriptor, string paramName)
        {
            throw new ArgumentException(
                $"The reflected type {descriptor.MemberInfo!.ReflectedType}"
                + " in the MemberInfo of {descriptor} is not assignable to {type}.",
                paramName);
        }


        [DoesNotReturn]
        public static void UnsupportedMember(Type type, MemberDescriptor descriptor, string paramName)
        {
            throw new ArgumentException(
                $"The {descriptor} for {type} contains an unsupported MemberInfo.",
                paramName);
        }

        [DoesNotReturn]
        public static void MemberNotFound(Type type, MemberDescriptor descriptor, List<MemberInfo> members)
        {
            if (members.Count == 0)
            {
                throw new MissingMemberException($"The {descriptor} does not match any member in {type}.");
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append("The ").Append(descriptor.ToString())
                    .Append(" for ").Append(type.ToString()).Append(" is ambiguous:");
                foreach (var member in members)
                {
                    sb.AppendLine().Append(member.ToString());
                }
                throw new AmbiguousMatchException(sb.ToString());
            }
        }
    }
}
