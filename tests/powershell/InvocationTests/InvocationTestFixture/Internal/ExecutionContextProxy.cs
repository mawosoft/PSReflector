// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace InvocationTestFixture.Internal;

internal readonly struct ExecutionContextProxy
{
    private static readonly PropertyInfo s_piExecutionContext =
        new StrictType(typeof(Runspace)).GetProperty(SMA.Members.ExecutionContext);
    private static readonly PropertyInfo s_piDollarErrorVariable =
        new StrictType(typeof(PSObject).Assembly, SMA.Types.ExecutionContext)
        .GetProperty(SMA.Members.DollarErrorVariable);

    public object? Target { get; }

    public ExecutionContextProxy(object? executionContext)
    {
        Target = executionContext is null || executionContext.GetType().FullName == SMA.Types.ExecutionContext
            ? executionContext
            : throw new ArgumentException(null, nameof(executionContext));
    }

    public static ExecutionContextProxy FromDefaultRunspace()
    {
        var runspace = Runspace.DefaultRunspace;
        return runspace is null ? default : new ExecutionContextProxy(s_piExecutionContext.GetValue(runspace));
    }

    public ArrayList? DollarErrorVariable => Target is null
        ? null
        : s_piDollarErrorVariable.GetValue(Target) as ArrayList;
}
