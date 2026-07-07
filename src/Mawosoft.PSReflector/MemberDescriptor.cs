// Copyright (c) Matthias Wolf, Mawosoft.

using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

/// <summary>
/// Describes a non-public instance or static member to expose.
/// </summary>
/// <remarks>
/// <para>
/// The description must resolve unambiguously. If that is not possible, provide the MemberInfo itself.
/// Allowed instance or static members are parameterless properties, fields, and methods. Also allowed
/// are instance constructors. To use indexer or parametrized properties, properties where only the setter
/// is non-public, or events, describe their accessor methods instead.
/// For constructors or methods, the ParamCount can be used if it is enough to disambiguate.
/// </para>
/// <para>
/// In PowerShell, you don't need to instantiate <see cref="MemberDescriptor"/> directly.
/// Use hashtable-notation to specify multiple properties, e.g. <c>@{ Name='Foo'; PSName='Bar' }</c>.
/// Use a <see langword="string" /> argument if you want to only specify the <see cref="Name"/>,
/// or a <see cref="System.Reflection.MemberInfo"/> object to pass only the <see cref="MemberInfo"/>.
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
    /// The name to use for the <see cref="PSMemberInfo"/> . Default is the same as <see cref="Name"/>
    /// or <c>new</c> for constructors.
    /// </summary>
    public string? PSName { get; set; }

    /// <summary>
    /// The number of parameters. Can disambiguate constructor and method overloads.
    /// Default is unspecified.
    /// </summary>
    public int? ParamCount { get; set; }

    /// <summary>
    /// A unique member info. If specified, it takes precedence over <see cref="Name"/> and
    /// <see cref="ParamCount"/>.
    /// </summary>
    public MemberInfo? MemberInfo { get; set; }

    /// <summary>Expose properties and fields as writable. Default is read-only.</summary>
    public bool CanWrite { get; set; }

    /// <summary>Default constructor.</summary>
    public MemberDescriptor() { }

    /// <summary>Constructor to support PowerShell conversion from <see langword="string"/>.</summary>
    public MemberDescriptor(string name) => Name = name;

    /// <summary>
    /// Constructor to support PowerShell conversion from <see cref="System.Reflection.MemberInfo"/>.
    /// </summary>
    public MemberDescriptor(MemberInfo memberInfo) => MemberInfo = memberInfo;

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(nameof(MemberDescriptor)).Append(" { ");
        bool comma = false;
        if (Name is null && MemberInfo is null)
        {
            sb.Append(nameof(Name)).Append(": null");
            comma = true;
        }
        else if (Name is not null)
        {
            sb.Append(nameof(Name)).Append(": ").Append(Name);
            comma = true;
        }
        if (PSName is not null)
        {
            if (comma) sb.Append(", ");
            sb.Append(nameof(PSName)).Append(": ").Append(PSName);
            comma = true;
        }
        if (ParamCount.HasValue)
        {
            if (comma) sb.Append(", ");
            sb.Append(nameof(ParamCount)).Append(": ").Append(ParamCount.Value);
            comma = true;
        }
        if (MemberInfo is not null)
        {
            if (comma) sb.Append(", ");
            sb.Append(nameof(MemberInfo)).Append(": ").Append(MemberInfo.ToString());
            comma = true;
        }
        if (CanWrite)
        {
            if (comma) sb.Append(", ");
            sb.Append(nameof(CanWrite)).Append(": true");
            comma = true;
        }
        sb.Append(" }");
        return sb.ToString();
    }
}
