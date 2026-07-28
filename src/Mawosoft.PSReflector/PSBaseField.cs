// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

public sealed class PSBaseField : PSPropertyInfo, IPSBaseMemberInfo
{
    private readonly FieldInfo _fieldInfo;
    private readonly Func<object?, object?>? _getter;
    private readonly Action<object?, object?>? _setter;
    private readonly bool _canWrite;
    private readonly bool _isStatic;
    private Reflector? _reflector;

    public override bool IsSettable => _canWrite;
    public override bool IsGettable => true;
    public override PSMemberTypes MemberType => PSMemberTypes.CodeProperty;
    public override string TypeNameOfValue => _fieldInfo.FieldType.ToString();
    public override PSMemberInfo Copy() => new PSBaseField(this);

    public override object? Value
    {
        get
        {
            var baseObject = _isStatic ? null : this.GetBaseObject();
            var retval = _getter is null ? _fieldInfo.GetValue(baseObject) : _getter(baseObject);
            return _reflector is null ? retval : _reflector.Expose(retval);
        }

        set
        {
            if (!_canWrite) throw new MemberAccessException();
            var baseObject = _isStatic ? null : this.GetBaseObject();
            var convertedValue = LanguagePrimitives.ConvertTo(value, _fieldInfo.FieldType,
                System.Globalization.CultureInfo.InvariantCulture);
            if (_setter is null)
                _fieldInfo.SetValue(baseObject, convertedValue);
            else
                _setter(baseObject, convertedValue);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (_isStatic) sb.Append("static ");
        if (!_canWrite) sb.Append("readonly ");
        PSBaseMemberFormatter.Format(sb, _fieldInfo.FieldType);
        sb.Append(' ').Append(_fieldInfo.Name);
        return sb.ToString();
    }

    internal PSBaseField(FieldInfo info, string? name, bool canWrite)
    {
        ArgumentNullException.ThrowIfNull(info);
        SetMemberName(string.IsNullOrEmpty(name) ? info.Name : name);
        _fieldInfo = info;
        var attr = _fieldInfo.Attributes; // This is a virtual call!
        _canWrite = canWrite && (attr & (FieldAttributes.Literal | FieldAttributes.InitOnly)) == 0;
        _isStatic = attr.HasFlag(FieldAttributes.Static);
        var instanceParam = Expression.Parameter(typeof(object));
        var instanceType = _fieldInfo.DeclaringType;
        Expression? instance = _isStatic
            ? null
            : instanceType.IsValueType
                ? Expression.Unbox(instanceParam, instanceType)
                : Expression.Convert(instanceParam, instanceType);
        var field = Expression.Field(instance, _fieldInfo);
        _getter = Expression.Lambda<Func<object?, object?>>(Expression.Convert(field, typeof(object)), instanceParam).Compile();
        if (_canWrite)
        {
            var valueParam = Expression.Parameter(typeof(object));
            _setter = Expression.Lambda<Action<object?, object?>>(
                Expression.Assign(field, Expression.Convert(valueParam, _fieldInfo.FieldType)),
                instanceParam,
                valueParam).Compile();
        }
    }

    private PSBaseField(PSBaseField other)
    {
        SetMemberName(other.Name);
        _fieldInfo = other._fieldInfo;
        _getter = other._getter;
        _setter = other._setter;
        _canWrite = other._canWrite;
        _isStatic = other._isStatic;
        _reflector = other._reflector;
    }

    // IPSBaseMemberInfo

    MemberInfo IPSBaseMemberInfo.MemberInfo => _fieldInfo;
    Type IPSBaseMemberInfo.ReturnType => _fieldInfo.FieldType;
    bool IPSBaseMemberInfo.IsStatic => _isStatic;
    bool IPSBaseMemberInfo.CanWrite => _canWrite;
    Reflector? IPSBaseMemberInfo.Reflector { get => _reflector; set => _reflector = value; }
}
