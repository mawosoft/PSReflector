// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using System.Reflection;
using System.Text;

using static Mawosoft.PSReflector.PSBaseMemberFormatter;

namespace Mawosoft.PSReflector;

public sealed class PSBaseProperty : PSPropertyInfo
{
    private readonly PropertyInfo _propertyInfo;
    private readonly bool _canWrite;
    private readonly bool _isStatic;
    private readonly Reflector? _reflector;
    private string? _definition;

    public override bool IsSettable => _canWrite;
    public override bool IsGettable => true;
    public override PSMemberTypes MemberType => PSMemberTypes.CodeProperty;
    public override string TypeNameOfValue => _propertyInfo.PropertyType.ToString();
    public override PSMemberInfo Copy()
        => new PSBaseProperty(_propertyInfo, Name, canWrite: _canWrite, isStatic: _isStatic, definition: _definition, reflector: _reflector);

    public override object? Value
    {
        get
        {
            var baseObject = _isStatic ? null : this.GetBaseObject();
            var retval = _propertyInfo.GetValue(baseObject);
            return _reflector is null ? retval : _reflector.Wrap(retval);
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
        if (_definition is null)
        {
            var sb = new StringBuilder();
            if (_isStatic) sb.Append("static ");
            Format(sb, _propertyInfo.PropertyType);
            sb.Append(' ').Append(_propertyInfo.Name).Append(" {get;");
            if (_canWrite) sb.Append("set;");
            sb.Append('}');
            _definition = sb.ToString();
        }
        return _definition;
    }

    internal PSBaseProperty(PropertyInfo info, string? name, bool canWrite, Reflector? reflector)
    {
        ArgumentNullException.ThrowIfNull(info);
        var getter = info.GetMethod ?? throw new ArgumentException(null, nameof(info));
        SetMemberName(string.IsNullOrEmpty(name) ? info.Name : name);
        _propertyInfo = info;
        _canWrite = canWrite && info.CanWrite;
        _isStatic = getter.IsStatic;
        _reflector = reflector;
    }

    private PSBaseProperty(PropertyInfo info, string name, bool canWrite, bool isStatic, string? definition, Reflector? reflector)
    {
        SetMemberName(name);
        _propertyInfo = info;
        _canWrite = canWrite;
        _isStatic = isStatic;
        _definition = definition;
        _reflector = reflector;
    }
}
