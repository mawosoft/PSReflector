// Copyright (c) Matthias Wolf, Mawosoft.

using System;

namespace Mawosoft.PSReflector.TestTypes;

public abstract class LastCallInfoProvider : ILastCallInfoProvider
{
    [ThreadStatic]
    private static CallInfo? s_lastCallInfoStatic;

    public static CallInfo? LastCallInfoStatic2
    {
        get => s_lastCallInfoStatic;
        set => s_lastCallInfoStatic = value;
    }

    public CallInfo? LastCallInfo { get; set; }

    public CallInfo? LastCallInfoStatic
    {
        get => s_lastCallInfoStatic;
        set => s_lastCallInfoStatic = value;
    }
}
