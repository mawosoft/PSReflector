// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;

using static Mawosoft.PSReflector.PSBaseMemberFormatter;

namespace Mawosoft.PSReflector;

public sealed class PSBaseConstructor : PSMethodInfo
{
    private readonly ConstructorInfo _ctorInfo;
    private readonly Reflector? _reflector;
    private string? _definition;

    public override Collection<string> OverloadDefinitions => [ToString()];
    public override PSMemberTypes MemberType => PSMemberTypes.CodeMethod;
    public override string TypeNameOfValue => typeof(PSBaseConstructor).FullName!;
    public override PSMemberInfo Copy() => new PSBaseConstructor(_ctorInfo, Name, _definition, _reflector);

    public override object? Invoke(params object[] arguments)
    {
        var retval = _ctorInfo.Invoke(arguments);
        return _reflector is null ? retval : _reflector.Wrap(retval);
    }

    public override string ToString() => _definition ??= Format(_ctorInfo);

    internal Type? DeclaringType => _ctorInfo.DeclaringType;

    internal PSBaseConstructor(ConstructorInfo info, string? name, Reflector? reflector)
    {
        ArgumentNullException.ThrowIfNull(info);
        SetMemberName(string.IsNullOrEmpty(name) ? "new" : name);
        _ctorInfo = info;
        _reflector = reflector;
    }

    private PSBaseConstructor(ConstructorInfo info, string name, string? definition, Reflector? reflector)
    {
        SetMemberName(name);
        _ctorInfo = info;
        _definition = definition;
        _reflector = reflector;
    }
}
