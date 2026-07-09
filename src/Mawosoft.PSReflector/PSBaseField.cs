// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Management.Automation;
using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

public sealed class PSBaseField : PSPropertyInfo, IPSBaseMemberInfo
{
    private readonly FieldInfo _fieldInfo;
    private readonly bool _canWrite;
    private readonly bool _isStatic;
    private Reflector? _reflector;

    public override bool IsSettable => _canWrite;
    public override bool IsGettable => true;
    public override PSMemberTypes MemberType => PSMemberTypes.CodeProperty;
    public override string TypeNameOfValue => _fieldInfo.FieldType.ToString();
    public override PSMemberInfo Copy()
        => new PSBaseField(_fieldInfo, Name, canWrite: _canWrite, isStatic: _isStatic, _reflector);

    public override object? Value
    {
        get
        {
            var baseObject = _isStatic ? null : this.GetBaseObject();
            var retval = _fieldInfo.GetValue(baseObject);
            return _reflector is null ? retval : _reflector.Expose(retval);
        }

        set
        {
            if (!_canWrite) throw new MemberAccessException();
            var baseObject = _isStatic ? null : this.GetBaseObject();
            _fieldInfo.SetValue(baseObject, value);
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
    }

    private PSBaseField(FieldInfo info, string name, bool canWrite, bool isStatic, Reflector? reflector)
    {
        SetMemberName(name);
        _fieldInfo = info;
        _canWrite = canWrite;
        _isStatic = isStatic;
        _reflector = reflector;
    }

    // IPSBaseMemberInfo

    MemberInfo IPSBaseMemberInfo.MemberInfo => _fieldInfo;
    Type IPSBaseMemberInfo.ReturnType => _fieldInfo.FieldType;
    bool IPSBaseMemberInfo.IsStatic => _isStatic;
    bool IPSBaseMemberInfo.CanWrite => _canWrite;
    Reflector? IPSBaseMemberInfo.Reflector { get => _reflector; set => _reflector = value; }
}
