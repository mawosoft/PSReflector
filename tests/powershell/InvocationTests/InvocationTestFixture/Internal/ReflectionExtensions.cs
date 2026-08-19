// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;

namespace InvocationTestFixture.Internal;

internal static class ReflectionExtensions
{
    extension(ParameterInfo[] @this)
    {
        public bool HasParamArray
            => @this.Length != 0 && @this[@this.Length - 1].IsDefined(typeof(ParamArrayAttribute), false);
    }
}
