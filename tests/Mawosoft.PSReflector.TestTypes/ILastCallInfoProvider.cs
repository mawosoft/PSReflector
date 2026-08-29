// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

// netstandard2.0 doesn't support static interface members.
public interface ILastCallInfoProvider
{
    public LoggedCallInfo? LastCallInfo { get; set; }
    public LoggedCallInfo? LastCallInfoStatic { get; set; }
}
