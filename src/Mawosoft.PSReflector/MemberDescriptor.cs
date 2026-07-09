// Copyright (c) Matthias Wolf, Mawosoft.

using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

/// <summary>
/// Describes a non-public type member to register for exposure. Types and their members are registered
/// via <see cref="Reflector.Register(System.Type[], Mawosoft.PSReflector.MemberDescriptor[], bool)"/>.
/// </summary>
public sealed class MemberDescriptor
{
    public const MemberTypes AllowedMemberTypes = MemberTypes.Field
                                                  | MemberTypes.Property
                                                  | MemberTypes.Method
                                                  | MemberTypes.Constructor;

    /// <summary>The original member name.</summary>
    public string? Name { get; set; }

    /// <summary>
    /// The name to use for the member in PowerShell. Default is the same as <see cref="Name"/>
    /// or <c>new</c> for constructors.
    /// </summary>
    public string? PSName { get; set; }

    /// <summary>
    /// The number of parameters. Can disambiguate constructor and method overloads.
    /// Default is unspecified.
    /// </summary>
    public int? ParamCount { get; set; }

    /// <summary>
    /// A <see cref="System.Reflection.MemberInfo"/> retrieved via reflection.
    /// If specified, it takes precedence over <see cref="Name"/> and <see cref="ParamCount"/>.
    /// </summary>
    public MemberInfo? MemberInfo { get; set; }

    /// <summary>
    /// Controls write access if the described member is a property or field. Default is read-only.
    /// </summary>
    public bool CanWrite { get; set; }

    public MemberDescriptor() { }

    public MemberDescriptor(string name) => Name = name;

    public MemberDescriptor(MemberInfo memberInfo) => MemberInfo = memberInfo;

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
