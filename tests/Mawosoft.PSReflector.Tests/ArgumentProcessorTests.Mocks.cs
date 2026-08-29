// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Mawosoft.PSReflector;

public partial class ArgumentProcessorTests
{
    private sealed class MockParameterInit
    {
        public ParameterAttributes Attributes { get; set; }
        public string? Name { get; set; }
        public Type? ParameterType { get; set; }
        public object? DefaultValue { get; set; }
        public bool IsParamArray { get; set; }
        public ICollection<object>? CustomAttributes { get; set; }

    }

    private sealed class MockParameterInfo : ParameterInfo
    {
        private readonly List<object> _customAttributes;

        public MockParameterInfo(MockParameterInit init, int position, MemberInfo member)
        {
            PositionImpl = position;
            MemberImpl = member;
            AttrsImpl = init.Attributes;
            NameImpl = init.Name;
            ClassImpl = init.ParameterType;
            DefaultValueImpl = init.DefaultValue;
            _customAttributes = [.. init.CustomAttributes ?? []];
            if (init.IsParamArray && !IsDefined(typeof(ParamArrayAttribute), false))
            {
                _customAttributes.Add(new ParamArrayAttribute());
            }
        }

        public override object? DefaultValue => DefaultValueImpl;
        public override object? RawDefaultValue => DefaultValueImpl;
        public override bool HasDefaultValue => DefaultValueImpl != Type.Missing;

        public override object[] GetCustomAttributes(bool inherit) => _customAttributes.ToArray();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            => _customAttributes.FindAll(ca => ca?.GetType() == attributeType).ToArray();

        public override bool IsDefined(Type attributeType, bool inherit)
            => _customAttributes.Find(ca => ca?.GetType() == attributeType) is not null;
    }

    private sealed class MockMethodBase : MethodBase
    {
        private readonly ParameterInfo[] _parameters;
        private readonly MemberTypes _memberType;

        public MockMethodBase(MockParameterInit[] parameters, MemberTypes memberType = MemberTypes.Method)
        {
            _parameters = new ParameterInfo[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                _parameters[i] = new MockParameterInfo(parameters[i], i, this);
            }

            _memberType = memberType;
        }

        public override Type? DeclaringType => null;
        public override MemberTypes MemberType => _memberType;
        public override ParameterInfo[] GetParameters() => (ParameterInfo[])_parameters.Clone();

        public override string ToString()
        {
            var sb = new StringBuilder(64);
            sb.Append(_memberType.ToString()[0]).Append('(');
            bool comma = false;
            for (int i = 0; i < _parameters.Length; i++)
            {
                if (comma) sb.Append(", ");
                comma = true;
                var pi = _parameters[i];
                if (pi.IsDefined(typeof(ParamArrayAttribute))) sb.Append("params ");
                var attr = pi.Attributes & (ParameterAttributes.In | ParameterAttributes.Out | ParameterAttributes.Optional);
                if (attr != default) sb.Append('[').Append(attr).Append("] ");
                sb.Append(pi.ParameterType.Name);

            }
            sb.Append(')');
            return sb.ToString();
        }

        // Nothing else is implemented

        [ExcludeFromCodeCoverage] public override MethodAttributes Attributes => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override string Name => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override Type? ReflectedType => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) => throw new NotImplementedException();
        [ExcludeFromCodeCoverage] public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();
    }
}
