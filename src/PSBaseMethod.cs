// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;

using static Mawosoft.PSReflector.PSBaseMemberFormatter;

namespace Mawosoft.PSReflector;

public sealed class PSBaseMethod : PSMethodInfo
{
    private readonly MethodInfo _methodInfo;
    private readonly bool _isStatic;
    private readonly Reflector? _reflector;
    private string? _definition;

    public override Collection<string> OverloadDefinitions => [ToString()];
    public override PSMemberTypes MemberType => PSMemberTypes.CodeMethod;
    public override string TypeNameOfValue => typeof(PSBaseMethod).FullName!;
    public override PSMemberInfo Copy()
        => new PSBaseMethod(_methodInfo, Name, _isStatic, _definition, _reflector);

    public override object? Invoke(params object[] arguments)
    {
        var baseObject = _isStatic ? null : this.GetBaseObject();
        var retval = _methodInfo.Invoke(baseObject, arguments);
        return _reflector is null ? retval : _reflector.Wrap(retval);
    }

    public override string ToString() => _definition ??= Format(_methodInfo);

    internal PSBaseMethod(MethodInfo info, string? name, Reflector? reflector)
    {
        ArgumentNullException.ThrowIfNull(info);
        SetMemberName(string.IsNullOrEmpty(name) ? info.Name : name);
        _methodInfo = info;
        _isStatic = info.IsStatic;
        _reflector = reflector;
    }

    private PSBaseMethod(MethodInfo info, string name, bool isStatic, string? definition, Reflector? reflector)
    {
        SetMemberName(name);
        _methodInfo = info;
        _isStatic = isStatic;
        _definition = definition;
        _reflector = reflector;
    }
}
