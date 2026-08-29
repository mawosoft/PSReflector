// Copyright (c) Matthias Wolf, Mawosoft.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mawosoft.PSReflector.TestHelpers;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

[assembly: TestFramework(typeof(DebuggerAwareTestFramework))]

namespace Mawosoft.PSReflector.TestHelpers;

internal sealed class DebuggerAwareTestFramework : XunitTestFramework
{
    private readonly string? _configFileName;

    public DebuggerAwareTestFramework() { }
    public DebuggerAwareTestFramework(string? configFileName) : base(configFileName) => _configFileName = configFileName;
    protected override ITestFrameworkExecutor CreateExecutor(Assembly assembly) => new DebuggerAwareTestFrameworkExecutor(new XunitTestAssembly(assembly, _configFileName));

    private sealed class DebuggerAwareTestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public DebuggerAwareTestFrameworkExecutor(IXunitTestAssembly testAssembly) : base(testAssembly) { }
        public override ValueTask RunTestCases(IReadOnlyCollection<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions, CancellationToken cancellationToken)
        {
            if (Debugger.IsAttached) executionOptions.SetParallelMode(ParallelMode.None);
            return base.RunTestCases(testCases, executionMessageSink, executionOptions, cancellationToken);
        }
    }
}
