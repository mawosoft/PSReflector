// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Reflection;

namespace Mawosoft.PSReflector;

/// <summary>
/// Provides functionality to register types and their non-public members for exposure and a way to
/// access them like public members.
/// </summary>
public class Reflector
{
    internal static readonly Func<object, bool, PSObject> PSObjectAsPSObject2Delegate = (Func<object, bool, PSObject>?)
        typeof(PSObject).GetMethod(nameof(PSObject.AsPSObject), BindingFlags.NonPublic | BindingFlags.Static,
                                   binder: null, [typeof(object), typeof(bool)], modifiers: null)?
        .CreateDelegate(typeof(Func<object, bool, PSObject>))
        ?? throw new MissingMethodException(nameof(PSObject), nameof(PSObject.AsPSObject));

    private readonly ConcurrentDictionary<Type, TypeDescriptor> _types = [];
    private readonly ConcurrentDictionary<string, Type> _typeNames = new(StringComparer.OrdinalIgnoreCase); // TODO

    /// <summary>Returns a collection of all registered types.</summary>
    public ICollection<Type> Types => _types.Keys;

    /// <summary>Returns <c>true</c> if the specified type has been registered.</summary>
    public bool ContainsType(Type type) => _types.ContainsKey(type);

    /// <summary>Returns <c>true</c> if the specified type has been registered.</summary>
    public bool ContainsType(string typeName) => _typeNames.ContainsKey(typeName);

    /// <summary>Gets a registered type by its name.</summary>
    /// <returns>The registered type, or <c>null</c> if it doesn't exist.</returns>
    public Type? GetType(string typeName) => _typeNames.TryGetValue(typeName, out var type) ? type : null;

    public void RegisterType(IDictionary typesAndMembers)
    {
        throw new NotImplementedException(); // TODO
    }

    public void RegisterType(string assemblyName, IDictionary typesAndMembers)
    {
        throw new NotImplementedException(); // TODO
    }

    public void RegisterType(string assemblyName, string @namespace, IDictionary typesAndMembers)
    {
        throw new NotImplementedException(); // TODO
    }

    public void RegisterType(Assembly assembly, IDictionary typesAndMembers)
    {
        throw new NotImplementedException(); // TODO
    }

    public void RegisterType(Assembly assembly, string @namespace, IDictionary typesAndMembers)
    {
        throw new NotImplementedException(); // TODO
    }

    /// <summary>
    /// Adds a type and its non-public members that should be exposed.
    /// </summary>
    /// <param name="type">
    /// The type whose non-public members are described via <paramref name="members"/>.
    /// </param>
    /// <param name="members">
    /// An array of <see cref="MemberDescriptor"/> instances that describe the non-public
    /// members to expose.
    /// </param>
    /// <remarks>
    /// A subsequent call of <c>AddType</c> with the same <paramref name="type"/> overwrites any existing
    /// entries for that type. <paramref name="members"/> of a type are <b>not</b> added cumulatively.
    /// </remarks>
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods",
        Justification = "False positive for 'members'.")]
    public void AddType(Type type, MemberDescriptor[] members)
    {
        // TODO make internal or private in favor of RegisterType() methods.
        ArgumentNullException.ThrowIfNull(type);
        if (members is null || members.Length == 0) ThrowMembersArgumentException();
        if (!IsAllowedType(type)) throw new ArgumentException(null, nameof(type));
        var typeInfo = new TypeDescriptor();
        foreach (var member in members)
        {
            if (member is null) ThrowMembersArgumentException();
            var mi = member.MemberInfo;
            if (mi is null)
            {
                mi = FindMemberInfo(type, member);
                if (mi is null) ThrowMembersArgumentException();
            }
            else
            {
                if (!IsAllowedMemberInfo(type, mi)) ThrowMembersArgumentException();
            }
            var psName = member.PSName;
            if (psName is null || psName.Length == 0) psName = mi is ConstructorInfo ? "new" : mi.Name;
            typeInfo.Pending.Add((psName, mi, member.CanWrite));
        }
        _types[type] = typeInfo;
    }

    /// <summary>
    /// Adds a type and its non-public members that should be exposed.
    /// </summary>
    /// <param name="type">
    /// The type whose non-public members are described via <paramref name="members"/>.
    /// </param>
    /// <param name="members">
    /// An array of non-public member names to expose.
    /// </param>
    /// <remarks>
    /// - Member names must be unambiguous. If they aren't, use one of the other overloads.<br />
    /// - If the member names refer to properties or fields, they will be exposed as read-only.
    /// If you need them writable,  use one of the other overloads.<br />
    /// - A subsequent call of <c>AddType</c> with the same <paramref name="type"/> overwrites any existing
    /// entries for that type. <paramref name="members"/> of a type are <b>not</b> added cumulatively.
    /// </remarks>
    public void AddType(Type type, string[] members)
    {
        throw new NotImplementedException(); // TODO obsolete this in favor of IDictionary conversion.
        //ArgumentNullException.ThrowIfNull(type);
        //if (members is null || members.Length == 0) ThrowMembersArgumentException();
        //var descriptors = new MemberDescriptor[members.Length];
        //for (int i = 0; i < descriptors.Length; i++)
        //{
        //    if (string.IsNullOrEmpty(members[i])) ThrowMembersArgumentException();
        //    descriptors[i] = new MemberDescriptor { Name = members[i] };
        //}
        // TODO AddType(type, Type.EmptyTypes, descriptors);
    }

    /// <summary>
    /// Returns <paramref name="obj"/> wrapped in a new <see cref="PSObject"/> if <paramref name="obj"/>
    /// is an instance of a type or a type that has been previously added via <c>AddType</c>.
    /// Otherwise it returns the same <paramref name="obj"/> unchanged.
    /// </summary>
    /// <param name="obj">The object to wrap.</param>
    /// <returns>
    /// - If <paramref name="obj"/> is an instance of a registered type, it is wrapped in a new
    /// <see cref="PSObject"/>. That wrapper has all declared non-public instance <b>and static</b>
    /// members added as public instance properties and methods. Non-public constructors are not
    /// added in this case.<br />
    /// - If <paramref name="obj"/> is a registered type, it is wrapped in a new <see cref="PSObject"/>
    /// as well. That wrapper exposes only the declared non-public static members and non-public
    /// constructors, but still as instance properties and methods.<br />
    /// - If <paramref name="obj"/> is something else (including <c>null</c>), it is returned unchanged.
    /// </returns>
    public object? Wrap(object? obj)
    {
        var o = obj is PSObject pso ? pso.BaseObject : obj;
        if (o is null) return obj;
        if (o is not Type type) type = o.GetType();
        if (!_types.TryGetValue(type, out var typeInfo)) return obj;
        return WrapObject(o, typeInfo);
    }

    /// <summary>
    /// Returns <paramref name="obj"/> wrapped in a new <see cref="PSObject"/> if <paramref name="asType"/>
    /// is a type that has been previously added via <c>AddType</c> and <paramref name="obj"/> is an instance
    /// of a type or a type that is assignable to <paramref name="asType"/>.
    /// Returns null if <paramref name="obj"/> is null.
    /// Throws if <paramref name="obj"/> is anything else or <paramref name="asType"/> has not been added.
    /// </summary>
    /// <param name="obj">The object to wrap.</param>
    /// <param name="asType">The base type to use.</param>
    /// <returns>
    /// - If <paramref name="obj"/> is an instance of a type assignable to <paramref name="asType"/>, it is
    /// wrapped in a new <see cref="PSObject"/>. That wrapper has all declared non-public instance
    /// <b>and static</b> members added as public instance properties and methods. Non-public constructors
    /// are not added in this case.<br />
    /// - If <paramref name="obj"/> is a type assignable to <paramref name="asType"/>, it is wrapped in a
    /// new <see cref="PSObject"/> as well. That wrapper exposes only the declared non-public static members
    /// and non-public constructors, but still as instance properties and methods.<br />
    /// - If <paramref name="obj"/> is <c>null</c>, <c>null</c> is returned.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// - <paramref name="obj"/> is not null <c>null</c> and not an instance of a type or a type assignable to
    /// <paramref name="asType"/>.<br />
    /// - <paramref name="asType"/> is not a type registered via <c>AddType</c>.
    /// </exception>
    public object? WrapAs(object? obj, Type asType)
    {
        var o = obj is PSObject pso ? pso.BaseObject : obj;
        if (o is null) return obj;
        if (o is not Type type) type = o.GetType();
        if (!(asType?.IsAssignableFrom(type) ?? false)) throw new ArgumentException(null, nameof(obj));
        if (!_types.TryGetValue(asType, out var typeInfo)) throw new ArgumentException(null, nameof(asType));
        return WrapObject(o, typeInfo);
    }

    public object Wrap(string typeName)
    {
        throw new NotImplementedException(); // TODO
    }

    private PSObject WrapObject(object obj, TypeDescriptor typeInfo)
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

    private void UpdateTypeDescriptor(TypeDescriptor typeInfo)
    {
        if (typeInfo.IsInitialized) return;
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
            mt = member.ParamCount.HasValue
                ? MemberTypes.Method | MemberTypes.Constructor
                : MemberDescriptor.AllowedMemberTypes;
        }
        var candidates = type.GetMember(name, mt,
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
            | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
        MemberInfo? found = null;
        MemberInfo? foundExact = null;
        bool foundMultiple = false;
        foreach (var candidate in candidates)
        {
            if (!IsAllowedMemberInfo(type, candidate)) continue;
            if (!member.ParamCount.HasValue
                || candidate is not MethodBase mb
                || mb.GetParameters().Length == member.ParamCount.Value)
            {
                if (name == candidate.Name)
                {
                    if (foundExact is not null) return null;
                    foundExact = candidate;
                }
                else
                {
                    if (found is not null) foundMultiple = true;
                    found = candidate;
                }
            }
        }
        if (foundExact is not null) return foundExact;
        if (foundMultiple) return null;
        return found;
    }

    [DoesNotReturn]
    private static void ThrowMembersArgumentException() => throw new ArgumentException(null, "members");

    private sealed class TypeDescriptor
    {
        public List<(string PSName, MemberInfo MemberInfo, bool CanWrite)> Pending { get; set; } = [];
        public List<PSMemberInfo> InstanceMembers { get; set; } = null!;
        public List<PSMemberInfo> TypeMembers { get; set; } = null!;
        public List<PSBaseConstructor> Constructors { get; set; } = null!;
        public bool IsInitialized { get; set; }
    }
}
