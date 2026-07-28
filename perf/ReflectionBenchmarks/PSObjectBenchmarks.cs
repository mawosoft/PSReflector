// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace ReflectionBenchmarks;

public abstract class PSObjectBenchmarks
{
    private static readonly Func<object, bool, PSObject> s_psObjectAsPSObject2Delegate =
        (Func<object, bool, PSObject>?)
        typeof(PSObject).GetMethod(nameof(PSObject.AsPSObject), BindingFlags.NonPublic | BindingFlags.Static,
                                   binder: null, [typeof(object), typeof(bool)], modifiers: null)?
        .CreateDelegate(typeof(Func<object, bool, PSObject>))
        ?? throw new MissingMethodException(nameof(PSObject), nameof(PSObject.AsPSObject));

    protected readonly Func<object, bool, PSObject> _psObjectAsPSObject2Delegate;
    protected readonly TestClass _testClass;
    protected readonly Consumer _consumer;

    public IEnumerable<object> AsPSObject_Internal_Arguments { get; } =
        [NamedValue.Create(false, "default"), NamedValue.Create(true, "local")];

    public IEnumerable<object> Members_Arguments()
    {
        yield return NamedValue.Create(((PSObject?)null, false), "cold-default");
        yield return NamedValue.Create(((PSObject?)null, true), "cold-local");
        PSObject pso = _psObjectAsPSObject2Delegate(_testClass, false);
        pso.Members.Consume(_consumer);
        yield return NamedValue.Create((pso, false), "warm-default");
        pso = _psObjectAsPSObject2Delegate(_testClass, true);
        pso.Members.Consume(_consumer);
        yield return NamedValue.Create((pso, true), "warm-local");
    }

    protected PSObjectBenchmarks()
    {
        _psObjectAsPSObject2Delegate = s_psObjectAsPSObject2Delegate;
        _testClass = new TestClass();
        _testClass.InitMembers();
        _consumer = new Consumer();
        var pso = _psObjectAsPSObject2Delegate(new Version(), false);
        _consumer.Consume(pso);
    }
}

public class PSObjectCreateBenchmarks : PSObjectBenchmarks
{
    [Benchmark]
    public void AsPSObject_Public()
    {
        var pso = PSObject.AsPSObject(_testClass);
        _consumer.Consume(pso);
    }

    [Benchmark]
    [ArgumentsSource(nameof(AsPSObject_Internal_Arguments))]
    public void AsPSObject_Internal(NamedValue<bool> args)
    {
        var pso = _psObjectAsPSObject2Delegate(_testClass, args.Value);
        _consumer.Consume(pso);
    }
}

public class PSObjectMembersAllBenchmarks : PSObjectBenchmarks
{
    [Benchmark]
    [ArgumentsSource(nameof(Members_Arguments))]
    public void GetAllMembers(NamedValue<(PSObject? pso, bool local)> args)
    {
        var pso = _psObjectAsPSObject2Delegate(_testClass, args.Value.local);
        _consumer.Consume(pso);
        (args.Value.pso ?? pso).Members.Consume(_consumer);
    }
}

public class PSObjectMembersSingleBenchmarks : PSObjectBenchmarks
{
    [Benchmark]
    [ArgumentsSource(nameof(Members_Arguments))]
    public PSMemberInfo GetSingleMember(NamedValue<(PSObject? pso, bool local)> args)
    {
        var pso = _psObjectAsPSObject2Delegate(_testClass, args.Value.local);
        _consumer.Consume(pso);
        return (args.Value.pso ?? pso).Members["Property2"];
    }
}
