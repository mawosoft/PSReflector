// Copyright (c) Matthias Wolf, Mawosoft.

using System.Reflection;

namespace Mawosoft.PSReflector;

/// <summary>
/// Describes a non-public instance or static member to expose.
/// </summary>
/// <remarks>
/// <para>
/// The description must resolve unambiguously. If that is not possible, provide the MemberInfo itself.
/// Allowed Member types are Property, Field, Method, and Constructor. Only instance constructors are
/// supported, all other members can be instance or static.
/// Properties are expected to have no parameters. Indexer properties, or properties where only the setter
/// is non-public, must be described via their accessor methods.
/// For constructors or methods, the ParamCount can be used if it is enough to disambiguate.
/// </para>
/// <para>
/// When wrapping an instance of a registered type, both static and instance members are added, excluding
/// constructors. When wrapping the registered type itself, the static members and the constructors are
/// added.
/// </para>
/// </remarks>
public sealed class MemberDescriptor
{
    public const MemberTypes AllowedMemberTypes = MemberTypes.Field
                                                  | MemberTypes.Property
                                                  | MemberTypes.Method
                                                  | MemberTypes.Constructor;

    /// <summary>The original member name.</summary>
    /// <remarks>
    /// Constructors are named <c>.ctor</c>.
    /// Explicit interface implementations must be prefixed with the fully qualified interface name.
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// The name to use for the PSMemberInfo. Default is the same as Name or <c>new</c> for constructors.
    /// </summary>
    public string? PSName { get; set; }

    /// <summary>
    /// The type(s) of the members to consider. This can be a flags combination.
    /// Default is all allowed member types.
    /// </summary>
    public MemberTypes MemberType { get; set; }

    /// <summary>
    /// The number of parameters. Can disambiguate constructors and methods. Default is unspecified.
    /// </summary>
    public int? ParamCount { get; set; }

    /// <summary>
    /// A unique member info. If specified, it takes precedence over Name, MemberType, and ParamCount.
    /// </summary>
    public MemberInfo? MemberInfo { get; set; }

    /// <summary>Expose properties and fields as writable. Default is read-only.</summary>
    public bool CanWrite { get; set; }

    public MemberDescriptor() { }

    public MemberDescriptor(string name)
    {
        Name = name;
    }

    public MemberDescriptor(string name, MemberTypes memberType)
    {
        Name = name;
        MemberType = memberType;
    }

    public MemberDescriptor(MemberInfo memberInfo)
    {
        MemberInfo = memberInfo;
    }
}
