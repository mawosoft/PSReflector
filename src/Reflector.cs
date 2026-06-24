// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Reflection;

namespace Mawosoft.PSReflector;

public class Reflector
{
    internal static readonly Func<object, bool, PSObject> PSObjectAsPSObject2Delegate = (Func<object, bool, PSObject>?)
        typeof(PSObject).GetMethod(nameof(PSObject.AsPSObject), BindingFlags.NonPublic | BindingFlags.Static, binder: null, [typeof(object), typeof(bool)], modifiers: null)?
        .CreateDelegate(typeof(Func<object, bool, PSObject>))
        ?? throw new MissingMethodException(nameof(PSObject), nameof(PSObject.AsPSObject));

    private readonly ConcurrentDictionary<Type, TypeDesriptor> _types = [];

    public ICollection<Type> Types => _types.Keys;

    public bool ContainsType(Type type) => _types.ContainsKey(type);

    public void AddType(Type type, MemberDescriptor[] members) => AddType(type, Type.EmptyTypes, members);

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "False positive for 'members'.")]
    public void AddType(Type type, string[] members)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (members is null || members.Length == 0) ThrowMembersArgumentException();
        var descriptors = new MemberDescriptor[members.Length];
        for (int i = 0; i < descriptors.Length; i++)
        {
            if (string.IsNullOrEmpty(members[i])) ThrowMembersArgumentException();
            descriptors[i] = new MemberDescriptor { Name = members[i] };
        }
        AddType(type, Type.EmptyTypes, descriptors);
    }

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "False positive for 'members'.")]
    public void AddType(Type baseType, Type[] derivedTypes, MemberDescriptor[] members)
    {
        ArgumentNullException.ThrowIfNull(baseType);
        ArgumentNullException.ThrowIfNull(derivedTypes);
        if (members is null || members.Length == 0) ThrowMembersArgumentException();
        if (!IsAllowedType(baseType)) throw new ArgumentException(null, nameof(baseType));
        foreach (var type in derivedTypes)
        {
            if (!IsAllowedType(type) || !baseType.IsAssignableFrom(type)) throw new ArgumentException(null, nameof(derivedTypes));
        }

        var typeInfo = new TypeDesriptor();
        foreach (var member in members)
        {
            if (member is null) ThrowMembersArgumentException();
            var mi = member.MemberInfo;
            if (mi is null)
            {
                mi = FindMemberInfo(baseType, member);
                if (mi is null) ThrowMembersArgumentException();
            }
            else
            {
                if (!IsAllowedMemberInfo(baseType, mi)) ThrowMembersArgumentException();
            }
            var psName = member.PSName;
            if (psName is null || psName.Length == 0) psName = mi is ConstructorInfo ? "new" : mi.Name;
            typeInfo.Pending.Add((psName, mi, member.CanWrite));
        }
        _types[baseType] = typeInfo;
        foreach (var type in derivedTypes) _types[type] = typeInfo;
    }

    public object? Wrap(object? obj)
    {
        var o = obj is PSObject pso ? pso.BaseObject : obj;
        if (o is null) return obj;
        if (o is not Type type) type = o.GetType();
        if (!_types.TryGetValue(type, out var typeInfo)) return obj;
        return WrapObject(o, typeInfo);
    }

    public object? WrapAs(object? obj, Type asType)
    {
        var o = obj is PSObject pso ? pso.BaseObject : obj;
        if (o is null) return obj;
        if (o is not Type type) type = o.GetType();
        if (!(asType?.IsAssignableFrom(type) ?? false)) throw new ArgumentException(null, nameof(asType));
        if (!_types.TryGetValue(asType, out var typeInfo)) return obj;
        return WrapObject(o, typeInfo);
    }

    private PSObject WrapObject(object obj, TypeDesriptor typeInfo)
    {
        if (!typeInfo.IsInitialized) UpdateTypeDescriptor(typeInfo);
        var pso = PSObjectAsPSObject2Delegate(obj, true);
        var psoMembers = pso.Members;
        if (obj is Type type)
        {
            foreach (var m in typeInfo.Constructors)
            {
                if (m.DeclaringType == type) psoMembers.Add(m);
            }
            foreach (var m in typeInfo.TypeMembers) psoMembers.Add(m);
        }
        else
        {
            foreach (var m in typeInfo.InstanceMembers) psoMembers.Add(m);
        }
        return pso;
    }

    private void UpdateTypeDescriptor(TypeDesriptor typeInfo)
    {
        if (!typeInfo.IsInitialized)
        {
            var pending = typeInfo.Pending;
            var instanceMembers = new List<PSMemberInfo>(pending.Count);
            var typeMembers = new List<PSMemberInfo>();
            var constructors = new List<PSBaseConstructor>();
            foreach ((string PSName, MemberInfo MemberInfo, bool CanWrite) in pending)
            {
                if (MemberInfo is ConstructorInfo ci)
                {
                    constructors.Add(new PSBaseConstructor(ci, PSName, this));
                }
                else
                {
                    bool isStatic;
                    PSMemberInfo psMember;
                    switch (MemberInfo)
                    {
                        case FieldInfo mi:
                            isStatic = mi.IsStatic;
                            psMember = new PSBaseField(mi, PSName, CanWrite,
                                _types.ContainsKey(mi.FieldType) ? this : null);
                            break;
                        case PropertyInfo mi:
                            isStatic = mi.GetMethod?.IsStatic == true;
                            psMember = new PSBaseProperty(mi, PSName, CanWrite,
                                _types.ContainsKey(mi.PropertyType) ? this : null);
                            break;
                        case MethodInfo mi:
                            isStatic = mi.IsStatic;
                            psMember = new PSBaseMethod(mi, PSName,
                                _types.ContainsKey(mi.ReturnType) ? this : null);
                            break;
                        default:
                            throw new UnreachableException();
                    }
                    instanceMembers.Add(psMember);
                    if (isStatic) typeMembers.Add(psMember);
                }
            }
            lock (typeInfo)
            {
                if (!typeInfo.IsInitialized)
                {
                    typeInfo.Constructors = constructors;
                    typeInfo.InstanceMembers = instanceMembers;
                    typeInfo.TypeMembers = typeMembers;
                    // We keep Pending around to allow some kind of Refresh to be implemented.
                    // That would be useful if types are added after Wrap() has been called and
                    // auto-wrapping of return values needs to be re-determined.
                    typeInfo.IsInitialized = true;
                }
            }

        }
    }

    private static bool IsAllowedType(Type? type)
    {
        if (type is null) return false;
        if (type.IsInterface || type.IsArray || type.ContainsGenericParameters) return false;
        return true;
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
                if (!mi.CanRead || mi.GetIndexParameters().Length != 0 || mi.PropertyType.ContainsGenericParameters) return false;
                break;
            case MethodInfo mi:
                if (mi.ContainsGenericParameters) return false;
                break;
            case ConstructorInfo mi:
                if (mi.DeclaringType != type || mi.IsStatic || mi.ContainsGenericParameters) return false;
                break;
            default:
                return false;
        }
        return true;
    }

    private static MemberInfo? FindMemberInfo(Type type, MemberDescriptor member)
    {
        var mt = member.MemberType;
        if ((mt & ~MemberDescriptor.AllowedMemberTypes) != 0) return null;
        var name = member.Name;
        if (string.IsNullOrEmpty(name))
        {
            if (mt != MemberTypes.Constructor) return null;
            name = ".ctor";
        }
        else if (member.Name == ".ctor")
        {
            mt = MemberTypes.Constructor;
        }
        else if (mt == default)
        {
            mt = member.ParamCount.HasValue ? MemberTypes.Method | MemberTypes.Constructor : MemberDescriptor.AllowedMemberTypes;
        }
        var candidates = type.GetMember(name, mt, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        MemberInfo? found = null;
        foreach (var candidate in candidates)
        {
            if (!IsAllowedMemberInfo(type, candidate)) continue;
            if (member.ParamCount.HasValue && candidate is MethodBase mb && mb.GetParameters().Length != member.ParamCount.Value) continue;
            if (found is not null) return null;
            found = candidate;
        }
        return found;
    }

    [DoesNotReturn]
    private static void ThrowMembersArgumentException() => throw new ArgumentException(null, "members");

    private sealed class TypeDesriptor
    {
        public List<(string PSName, MemberInfo MemberInfo, bool CanWrite)> Pending { get; set; } = [];
        public List<PSMemberInfo> InstanceMembers { get; set; } = null!;
        public List<PSMemberInfo> TypeMembers { get; set; } = null!;
        public List<PSBaseConstructor> Constructors { get; set; } = null!;
        public bool IsInitialized { get; set; }
    }
}
