// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Reflection;

namespace Mawosoft.PSReflector;

internal interface IPSBaseMemberInfo
{
    public MemberInfo MemberInfo { get; }
    public Type ReturnType { get; }
    public bool IsStatic { get; }
    public bool CanWrite { get; }
    public Reflector? Reflector { get; set; }
}
