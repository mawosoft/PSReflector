// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

public sealed class PSBaseProperty : PSPropertyInfo, IPSBaseMemberInfo
{
    private readonly PropertyInfo _propertyInfo;
    private readonly Func<object?, object?>? _getter;
    private readonly Action<object?, object?>? _setter;
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
            var retval = _getter is null ? _propertyInfo.GetValue(baseObject) : _getter(baseObject);
            return _reflector is null ? retval : _reflector.Expose(retval);
        }

        set
        {
            if (!_canWrite) throw new MemberAccessException();
            var baseObject = _isStatic ? null : this.GetBaseObject();
            var convertedValue = LanguagePrimitives.ConvertTo(value, _propertyInfo.PropertyType,
                System.Globalization.CultureInfo.InvariantCulture);
            if (_setter is null)
                _propertyInfo.SetValue(baseObject, convertedValue);
            else
                _setter(baseObject, convertedValue);
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
        var setter = info.SetMethod;
        SetMemberName(string.IsNullOrEmpty(name) ? info.Name : name);
        _propertyInfo = info;
        _canWrite = canWrite && setter is not null;
        _isStatic = getter.IsStatic;
        var instanceParam = Expression.Parameter(typeof(object));
        var instanceType = _propertyInfo.DeclaringType;
        Expression? instance = _isStatic
            ? null
            : instanceType.IsValueType
                ? Expression.Unbox(instanceParam, instanceType)
                : Expression.Convert(instanceParam, instanceType);
        _getter = Expression.Lambda<Func<object?, object?>>(
            Expression.Convert(Expression.Call(instance, getter), typeof(object)),
            instanceParam).Compile();
        if (_canWrite)
        {
            var valueParam = Expression.Parameter(typeof(object));
            _setter = Expression.Lambda<Action<object?, object?>>(
                Expression.Call(instance, setter, Expression.Convert(valueParam, _propertyInfo.PropertyType)),
                instanceParam,
                valueParam).Compile();
        }
    }

    private PSBaseProperty(PSBaseProperty other)
    {
        SetMemberName(other.Name);
        _propertyInfo = other._propertyInfo;
        _getter = other._getter;
        _setter = other._setter;
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
