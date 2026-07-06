// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

// netstandard2.0 doesn't support static interface members.
public interface ILastCallInfoProvider
{
    public CallInfo? LastCallInfo { get; set; }
    public CallInfo? LastCallInfoStatic { get; set; }
}
