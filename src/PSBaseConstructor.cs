// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;

namespace Mawosoft.PSReflector;

public sealed class PSBaseConstructor : PSMethodInfo, IPSBaseMemberInfo
{
    private readonly ConstructorInfo _ctorInfo;
    private Reflector? _reflector;

    public override Collection<string> OverloadDefinitions => [ToString()];
    public override PSMemberTypes MemberType => PSMemberTypes.CodeMethod;
    public override string TypeNameOfValue => typeof(PSBaseConstructor).FullName!;
    public override PSMemberInfo Copy() => new PSBaseConstructor(_ctorInfo, Name, _reflector);

    public override object? Invoke(params object[] arguments)
    {
        var retval = _ctorInfo.Invoke(arguments);
        return _reflector is null ? retval : _reflector.Wrap(retval);
    }

    public override string ToString() => PSBaseMemberFormatter.Format(_ctorInfo);

    internal PSBaseConstructor(ConstructorInfo info, string? name)
    {
        ArgumentNullException.ThrowIfNull(info);
        SetMemberName(string.IsNullOrEmpty(name) ? "new" : name);
        _ctorInfo = info;
    }

    private PSBaseConstructor(ConstructorInfo info, string name, Reflector? reflector)
    {
        SetMemberName(name);
        _ctorInfo = info;
        _reflector = reflector;
    }

    // IPSBaseMemberInfo

    MemberInfo IPSBaseMemberInfo.MemberInfo => _ctorInfo;
    Type IPSBaseMemberInfo.ReturnType => _ctorInfo.ReflectedType;
    bool IPSBaseMemberInfo.IsStatic => _ctorInfo.IsStatic;
    bool IPSBaseMemberInfo.CanWrite => false;
    Reflector? IPSBaseMemberInfo.Reflector { get => _reflector; set => _reflector = value; }
}
