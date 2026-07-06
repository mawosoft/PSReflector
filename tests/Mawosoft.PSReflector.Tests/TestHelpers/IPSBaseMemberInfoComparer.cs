// Copyright (c) Matthias Wolf, Mawosoft.

using System.Collections.Generic;

namespace Mawosoft.PSReflector.TestHelpers;

internal sealed class IPSBaseMemberInfoComparer : IEqualityComparer<IPSBaseMemberInfo>
{
    public static IPSBaseMemberInfoComparer Instance { get; } = new();

    private IPSBaseMemberInfoComparer() { }

    public bool Equals(IPSBaseMemberInfo? x, IPSBaseMemberInfo? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x.Name != y.Name) return false;
        if (x.MemberInfo != y.MemberInfo) return false;
        if (x.ReturnType != y.ReturnType) return false;
        if (x.IsStatic != y.IsStatic) return false;
        if (x.CanWrite != y.CanWrite) return false;
        if (x.Reflector != y.Reflector) return false;
        return true;
    }

    public int GetHashCode(IPSBaseMemberInfo obj) => 0;
}
