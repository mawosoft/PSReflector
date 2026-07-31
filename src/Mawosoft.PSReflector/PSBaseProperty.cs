// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

public sealed class PSBaseProperty : PSPropertyInfo, IPSBaseMemberInfo
{
    private readonly PropertyInfo _propertyInfo;
    private readonly bool _canWrite;
    private readonly bool _isStatic;
    private Reflector? _reflector;

    public override bool IsSettable => _canWrite;
    public override bool IsGettable => true;
    public override PSMemberTypes MemberType => PSMemberTypes.CodeProperty;
    public override string TypeNameOfValue => _propertyInfo.PropertyType.ToString();
    public override PSMemberInfo Copy() => new PSBaseProperty(this);

    public override object? Value
    {
        get
        {
            var baseObject = _isStatic ? null : this.GetBaseObject();
            var retval = _propertyInfo.GetValue(baseObject);
            return _reflector is null ? retval : _reflector.Expose(retval);
        }

        set
        {
            if (!_canWrite) throw new MemberAccessException();
            var baseObject = _isStatic ? null : this.GetBaseObject();
            _propertyInfo.SetValue(baseObject, value);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (_isStatic) sb.Append("static ");
        PSBaseMemberFormatter.Format(sb, _propertyInfo.PropertyType);
        sb.Append(' ').Append(_propertyInfo.Name).Append(" {get;");
        if (_canWrite) sb.Append("set;");
        sb.Append('}');
        return sb.ToString();
    }

    internal PSBaseProperty(PropertyInfo info, string? name, bool canWrite)
    {
        ArgumentNullException.ThrowIfNull(info);
        var getter = info.GetMethod ?? throw new ArgumentException(null, nameof(info));
        SetMemberName(string.IsNullOrEmpty(name) ? info.Name : name);
        _propertyInfo = info;
        _canWrite = canWrite && info.CanWrite;
        _isStatic = getter.IsStatic;
    }

    private PSBaseProperty(PSBaseProperty other)
    {
        SetMemberName(other.Name);
        _propertyInfo = other._propertyInfo;
        _canWrite = other._canWrite;
        _isStatic = other._isStatic;
        _reflector = other._reflector;
    }

    // IPSBaseMemberInfo

    MemberInfo IPSBaseMemberInfo.MemberInfo => _propertyInfo;
    Type IPSBaseMemberInfo.ReturnType => _propertyInfo.PropertyType;
    bool IPSBaseMemberInfo.IsStatic => _isStatic;
    bool IPSBaseMemberInfo.CanWrite => _canWrite;
    Reflector? IPSBaseMemberInfo.Reflector { get => _reflector; set => _reflector = value; }
}
