// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace Mawosoft.PSReflector;

public sealed class PSBaseMethod : PSMethodInfo, IPSBaseMemberInfo
{
    private readonly MethodInfo _methodInfo;
    private object? _smaMethodInfo;
    private readonly bool _isStatic;
    private Reflector? _reflector;

    public override Collection<string> OverloadDefinitions => [ToString()];
    public override PSMemberTypes MemberType => PSMemberTypes.CodeMethod;
    public override string TypeNameOfValue => typeof(PSBaseMethod).FullName!;
    public override PSMemberInfo Copy() => new PSBaseMethod(this);

    public override object? Invoke(params object[] arguments)
    {
        var baseObject = _isStatic ? null : this.GetBaseObject();
        _smaMethodInfo ??= MethodBaseInvoker.GetMethodInformationArray(_methodInfo);
        if (!_isStatic && _methodInfo.DeclaringType.IsValueType)
        {
        }
        // This also takes care of void => AutomationNull.Value.
        var retval = MethodBaseInvoker.InvokeMethod(baseObject, _methodInfo, _smaMethodInfo, arguments);
        return _reflector is null ? retval : _reflector.Expose(retval);
        //var retval = _methodInfo.Invoke(baseObject, arguments);
        //return _methodInfo.ReturnType == typeof(void)
        //    ? AutomationNull.Value
        //    : _reflector is null ? retval : _reflector.Expose(retval);
    }

    public override string ToString() => PSBaseMemberFormatter.Format(_methodInfo);

    internal PSBaseMethod(MethodInfo info, string? name)
    {
        ArgumentNullException.ThrowIfNull(info);
        SetMemberName(string.IsNullOrEmpty(name) ? info.Name : name);
        _methodInfo = info;
        _isStatic = info.IsStatic;
    }

    private PSBaseMethod(PSBaseMethod other)
    {
        SetMemberName(other.Name);
        _methodInfo = other._methodInfo;
        _smaMethodInfo = other._smaMethodInfo;
        _isStatic = other._isStatic;
        _reflector = other._reflector;
    }

    // IPSBaseMemberInfo

    MemberInfo IPSBaseMemberInfo.MemberInfo => _methodInfo;
    Type IPSBaseMemberInfo.ReturnType => _methodInfo.ReturnType;
    bool IPSBaseMemberInfo.IsStatic => _isStatic;
    bool IPSBaseMemberInfo.CanWrite => false;
    Reflector? IPSBaseMemberInfo.Reflector { get => _reflector; set => _reflector = value; }
}
