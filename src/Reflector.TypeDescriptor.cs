// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

namespace Mawosoft.PSReflector;

internal sealed class TypeDescriptor
{
    public Type Type { get; set; }
    public List<(string PSName, MemberInfo MemberInfo, bool CanWrite)> MemberDescriptors { get; set; } = [];
    public List<PSMemberInfo> InstanceMembers { get; set; } = null!;
    public List<PSMemberInfo> TypeMembers { get; set; } = null!;
    public int Version { get; set; }
    public TypeDescriptor(Type type) => Type = type;
}
