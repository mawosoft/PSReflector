// Copyright (c) Matthias Wolf, Mawosoft.

using System;

namespace InvocationTestFixture.Logging;

public sealed class ArgumentInfo
{
    public ObjectInfo Before { get; internal set; }
    public ObjectInfo Enter { get; internal set; }
    public ObjectInfo Exit { get; internal set; }
    public ObjectInfo After { get; internal set; }

    public ObjectInfo this[InvocationStep step]
    {
        get => step switch
        {
            InvocationStep.Before => Before,
            InvocationStep.Enter => Enter,
            InvocationStep.Exit => Exit,
            InvocationStep.After => After,
            _ => throw new ArgumentOutOfRangeException(nameof(step)),
        };
        internal set
        {
            switch (step)
            {
                case InvocationStep.Before:
                    Before = value;
                    break;
                case InvocationStep.Enter:
                    Enter = value;
                    break;
                case InvocationStep.Exit:
                    Exit = value;
                    break;
                case InvocationStep.After:
                    After = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(step));
            }
        }
    }
}
