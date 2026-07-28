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
    public override PSMemberInfo Copy() => new PSBaseConstructor(this);

    public override object? Invoke(params object[] arguments)
    {
        // TODO Use MethodBaseInvoker
        var retval = _ctorInfo.Invoke(arguments);
        return _reflector is null ? retval : _reflector.Expose(retval);
    }

    public override string ToString() => PSBaseMemberFormatter.Format(_ctorInfo);

    internal PSBaseConstructor(ConstructorInfo info, string? name)
    {
        ArgumentNullException.ThrowIfNull(info);
        SetMemberName(string.IsNullOrEmpty(name) ? "new" : name);
        _ctorInfo = info;
    }

    private PSBaseConstructor(PSBaseConstructor other)
    {
        SetMemberName(other.Name);
        _ctorInfo = other._ctorInfo;
        _reflector = other._reflector;
    }

    // IPSBaseMemberInfo

    MemberInfo IPSBaseMemberInfo.MemberInfo => _ctorInfo;
    Type IPSBaseMemberInfo.ReturnType => _ctorInfo.ReflectedType;
    bool IPSBaseMemberInfo.IsStatic => _ctorInfo.IsStatic;
    bool IPSBaseMemberInfo.CanWrite => false;
    Reflector? IPSBaseMemberInfo.Reflector { get => _reflector; set => _reflector = value; }
}
