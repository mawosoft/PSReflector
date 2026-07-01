// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Reflection;
using System.Threading;

namespace Mawosoft.PSReflector;

/// <summary>
/// Provides functionality to register types and their non-public members for exposure and a way to
/// access them like public members.
/// </summary>
public sealed class Reflector
{
    internal static readonly Func<object, bool, PSObject> PSObjectAsPSObject2Delegate =
        (Func<object, bool, PSObject>?)
        typeof(PSObject).GetMethod(nameof(PSObject.AsPSObject), BindingFlags.NonPublic | BindingFlags.Static,
                                   binder: null, [typeof(object), typeof(bool)], modifiers: null)?
        .CreateDelegate(typeof(Func<object, bool, PSObject>))
        ?? throw new MissingMethodException(nameof(PSObject), nameof(PSObject.AsPSObject));

    private readonly ConcurrentDictionary<Type, TypeDescriptor> _registeredTypes = [];
    private readonly ConcurrentDictionary<Type, bool> _additionalTypes = [];
    private volatile int _typesVersion;

    /// <summary>Returns a collection of types that have been registered with this instance.</summary>
    public ICollection<Type> RegisteredTypes => _registeredTypes.Keys;

    /// <summary>Returns <c>true</c> if the specified type has been registered with this instance.</summary>
    public bool IsTypeRegistered(Type type) => _registeredTypes.ContainsKey(type);

    /// <summary>
    /// Returns a collection of additional types that trigger automatic wrapping of return values from
    /// registered members.
    /// </summary>
    public ICollection<Type> AdditionalTypes => _additionalTypes.Keys;

    /// <summary>
    /// Registers non-public type members to be exposed like public members via <see cref="Wrap(object?)"/>.
    /// </summary>
    /// <param name="types">
    /// An array of types whose non-public members are described by <paramref name="members"/>. 
    /// Types must be concrete (not abstract classes or interfaces), though static classes are permitted.
    /// </param>
    /// <param name="members">
    /// An array of <see cref="MemberDescriptor"/> instances that describe the non-public members to expose.
    /// </param>
    /// <param name="caseSensitive">
    /// <c>true</c> to perform a case-sensitive search for member names. The default is <c>false</c>, which
    /// performs a case-insensitive search to match PowerShell's behavior. If a case-insensitive search is
    /// ambiguous, a single case-sensitive match will resolve the ambiguity.
    /// </param>
    /// <remarks>
    /// - You can invoke this method multiple times to cumulatively register different members for the same
    /// type.<br/>
    /// - Passing an array of types allows you to conveniently register shared interface members across several
    /// implementing types in a single call.
    /// </remarks>
    public void Register(Type[] types, MemberDescriptor[] members, bool caseSensitive = false)
    {
        Throw.IfNullOrEmpty(types);
        Throw.IfNullOrEmpty(members);
        List<MemberInfo> memberInfos = [];
        List<TypeDescriptor> typeDescriptors = [];
        foreach (var type in types)
        {
            if (type.HasElementType
                || type.IsInterface
                || (type.IsAbstract && !type.IsSealed)
                || type.ContainsGenericParameters
                || typeof(Type).IsAssignableFrom(type))
            {
                Throw.UnsupportedType(type, nameof(types));
            }
            var typeDescriptor = new TypeDescriptor(type);
            typeDescriptors.Add(typeDescriptor);
            foreach (var member in members)
            {
                if ((string.IsNullOrEmpty(member.Name) && member.MemberInfo is null)
                    || (member.ParamCount.HasValue && member.ParamCount.Value < 0))
                {
                    Throw.InvalidMemberDescriptor(type, member, nameof(members));
                }
                var mi = member.MemberInfo;
                if (mi is null)
                {
                    if (FindMemberInfo(type, member.Name!, member.ParamCount, caseSensitive, memberInfos))
                    {
                        mi = memberInfos[0];
                    }
                    else
                    {
                        Throw.MemberNotFound(type, member, memberInfos);
                    }
                }
                else
                {
                    if (mi.ReflectedType is null || !mi.ReflectedType.IsAssignableFrom(type))
                        Throw.NonmatchingMemberDescriptor(type, member, nameof(members));
                    if (!IsAllowedMemberInfo(type, mi))
                        Throw.UnsupportedMember(type, member, nameof(members));
                }
                var psName = member.PSName;
                if (psName is null || psName.Length == 0) psName = mi is ConstructorInfo ? "new" : mi.Name;
                typeDescriptor.MemberDescriptors.Add((psName, mi, member.CanWrite));
            }
        }
        Interlocked.Increment(ref _typesVersion);
        foreach (var typeDescriptor in typeDescriptors)
        {
            _registeredTypes.AddOrUpdate(
                typeDescriptor.Type,
                (type) =>
                {
                    // Track additional member return types that may yield an instance of type.
                    // TODO We are currently limiting this to interfaces and abstract base classes.
                    //      Not sure if that limit makes sense or even the entire idea, since
                    //      it only applies to members we add, not the regular public ones.
                    foreach (var t in type.GetInterfaces())
                    {
                        _additionalTypes.TryAdd(t, false);
                    }
                    if (!type.IsValueType)
                    {
                        for (var t = type.BaseType; t is not null; t = t.BaseType)
                        {
                            if (t.IsAbstract) _additionalTypes.TryAdd(t, false);
                        }
                    }
                    return typeDescriptor;
                },
                (type, existing) =>
                {
                    // Replacing the descriptor ensures:
                    // - Access to MemberDescriptors is thread-safe.
                    // - TypeDescriptor reverts to unitialized.
                    // - Even a higher Version on the existing TypeDescriptor doesn't cause problems.
                    var td = new TypeDescriptor(type);
                    td.MemberDescriptors.AddRange(existing.MemberDescriptors);
                    td.MemberDescriptors.AddRange(typeDescriptor.MemberDescriptors);
                    return td;
                });
        }
    }

    /// <summary>
    /// Returns <paramref name="obj"/> wrapped in a new <see cref="PSObject"/> if <paramref name="obj"/>
    /// is an instance of a registered type, or the registered type itself. The type must have been 
    /// previously registered via <see cref="Register(Type[], MemberDescriptor[], bool)"/>.
    /// Otherwise, it returns the same <paramref name="obj"/> unchanged.
    /// </summary>
    /// <param name="obj">The object to wrap.</param>
    /// <returns>
    /// - If <paramref name="obj"/> is an instance of a registered type, it is wrapped in a new
    /// <see cref="PSObject"/>. The wrapper exposes all registered non-public instance <b>and static</b>
    /// members as public instance properties and methods. Non-public constructors are not
    /// added in this case.<br />
    /// - If <paramref name="obj"/> is a registered type itself, it is also wrapped in a new
    /// <see cref="PSObject"/>. This wrapper exposes only the registered non-public static members and
    /// non-public constructors, but still as instance properties and methods.<br />
    /// - If <paramref name="obj"/> is something else (including <c>null</c>), it is returned unchanged.
    /// </returns>
    public object? Wrap(object? obj)
    {
        var o = obj is PSObject pso ? pso.BaseObject : obj;
        if (o is null) return obj;
        if (o is not Type type) type = o.GetType();
        if (!_registeredTypes.TryGetValue(type, out var typeDescriptor)) return obj;
        return WrapObject(o, typeDescriptor);
    }

    /// <summary>
    /// Returns <paramref name="obj"/> wrapped in a new <see cref="PSObject"/> if it is 
    /// an instance of a registered type, the type itself, or if it can be cast to the 
    /// <paramref name="fallbackType"/>. Types must have been previously registered via 
    /// <see cref="Register(Type[], MemberDescriptor[], bool)"/>.
    /// Otherwise, it returns the same <paramref name="obj"/> unchanged.
    /// </summary>
    /// <param name="obj">The object to wrap.</param>
    /// <param name="fallbackType">
    /// A previously registered type to use as a fallback if the type of <paramref name="obj"/>
    /// is not registered.
    /// </param>
    /// <returns>
    /// - If <paramref name="obj"/> is an instance of a registered type, or if it can be cast to 
    /// <paramref name="fallbackType"/>, it is wrapped in a new <see cref="PSObject"/>. 
    /// This wrapper exposes all registered non-public instance <b>and static</b> members 
    /// (from either its own type or the fallback type) as public instance properties and methods. 
    /// Non-public constructors are not added in this case.<br />
    /// - If <paramref name="obj"/> is a registered type itself, it is also wrapped in a new
    /// <see cref="PSObject"/>. This wrapper exposes only the registered non-public static members
    /// and non-public constructors, but still as instance properties and methods.<br />
    /// - If <paramref name="obj"/> is something else (including <c>null</c>), it is returned unchanged.
    /// </returns>
    public object? Wrap(object? obj, Type fallbackType)
    {
        var o = obj is PSObject pso ? pso.BaseObject : obj;
        if (o is null) return obj;
        TypeDescriptor? typeDescriptor;
        if (o is Type type)
        {
            if (!_registeredTypes.TryGetValue(type, out typeDescriptor)) return obj;
        }
        else
        {
            type = o.GetType();
            if (!_registeredTypes.TryGetValue(type, out typeDescriptor))
            {
                if (fallbackType is null || !_registeredTypes.TryGetValue(fallbackType, out typeDescriptor))
                {
                    throw new ArgumentException(null, nameof(fallbackType));
                }
                if (!fallbackType.IsAssignableFrom(type)) return obj;
            }
        }
        return WrapObject(o, typeDescriptor);
    }

    private PSObject WrapObject(object obj, TypeDescriptor typeDescriptor)
    {
        if (typeDescriptor.Version < _typesVersion) UpdateTypeDescriptor(typeDescriptor);
        var pso = PSObjectAsPSObject2Delegate(obj, true);
        var psoMembers = pso.Members;
        var source = obj is Type ? typeDescriptor.TypeMembers : typeDescriptor.InstanceMembers;
        foreach (var m in source) psoMembers.Add(m);
        return pso;
    }

    private void UpdateTypeDescriptor(TypeDescriptor typeDescriptor)
    {
        int version = _typesVersion;
        if (typeDescriptor.Version >= version) return;
        var memberDescriptors = typeDescriptor.MemberDescriptors;
        var instanceMembers = new List<PSMemberInfo>(memberDescriptors.Count);
        var typeMembers = new List<PSMemberInfo>();
        foreach ((string PSName, MemberInfo MemberInfo, bool CanWrite) in memberDescriptors)
        {
            if (MemberInfo is ConstructorInfo ci)
            {
                typeMembers.Add(new PSBaseConstructor(ci, PSName) { Reflector = this });
            }
            else
            {
                PSMemberInfo psMember = MemberInfo switch
                {
                    FieldInfo mi => new PSBaseField(mi, PSName, CanWrite),
                    PropertyInfo mi => new PSBaseProperty(mi, PSName, CanWrite),
                    MethodInfo mi => new PSBaseMethod(mi, PSName),
                    _ => throw new UnreachableException(),
                };
                IPSBaseMemberInfo psBaseMember = (IPSBaseMemberInfo)psMember;
                var returnType = psBaseMember.ReturnType;
                if (_registeredTypes.ContainsKey(returnType) || _additionalTypes.ContainsKey(returnType))
                {
                    psBaseMember.Reflector = this;
                }
                instanceMembers.Add(psMember);
                if (psBaseMember.IsStatic) typeMembers.Add(psMember);
            }
        }
        lock (typeDescriptor)
        {
            if (typeDescriptor.Version < version)
            {
                typeDescriptor.InstanceMembers = instanceMembers;
                typeDescriptor.TypeMembers = typeMembers;
                typeDescriptor.Version = version;
            }
        }
    }

    private static bool IsAllowedMemberInfo(Type type, MemberInfo memberInfo)
    {
        if ((memberInfo.MemberType & MemberDescriptor.AllowedMemberTypes) == 0) return false;
        if (!(memberInfo.ReflectedType?.IsAssignableFrom(type) ?? false)) return false;
        switch (memberInfo)
        {
            case FieldInfo mi:
                if (mi.FieldType.ContainsGenericParameters) return false;
                break;
            case PropertyInfo mi:
                if (!mi.CanRead) return false;
                if (mi.GetIndexParameters().Length != 0) return false;
                if (mi.PropertyType.ContainsGenericParameters) return false;
                break;
            case MethodInfo mi:
                if (mi.ContainsGenericParameters) return false;
                break;
            case ConstructorInfo mi:
                if (mi.DeclaringType != type) return false;
                if (mi.IsStatic) return false;
                if (mi.ContainsGenericParameters) return false;
                break;
            default:
                return false;
        }
        return true;
    }

    private static bool FindMemberInfo(
        Type type,
        string name,
        int? paramCount,
        bool caseSensitive,
        List<MemberInfo> results)
    {
        results.Clear();
        var mt = name == ".ctor"
            ? MemberTypes.Constructor
            : paramCount.HasValue
                ? MemberTypes.Method | MemberTypes.Constructor
                : MemberDescriptor.AllowedMemberTypes;
        var bf = caseSensitive
            ? BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
              | BindingFlags.FlattenHierarchy
            : BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
              | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase;
        var candidates = type.GetMember(name, mt, bf);
        int exactMatches = 0;
        foreach (var candidate in candidates)
        {
            if (!IsAllowedMemberInfo(type, candidate)) continue;
            if (!paramCount.HasValue
                || candidate is not MethodBase mb
                || mb.GetParameters().Length == paramCount.Value)
            {
                if (caseSensitive || name == candidate.Name)
                {
                    results.Insert(exactMatches++, candidate);
                }
                else
                {
                    results.Add(candidate);
                }
            }
        }
        if (exactMatches != 0)
        {
            results.RemoveRange(exactMatches, results.Count - exactMatches);
        }
        return results.Count == 1;
    }
}
