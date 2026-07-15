// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Linq;
using System.Management.Automation;
using Mawosoft.PSReflector.TestHelpers;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class ReflectorExposeTests
{

    public static readonly MatrixTheoryData<object?, Type?> Unregistered_MatrixData = new(
        [
            null,
            new SimpleClass(),
            new PSObject(new SimpleClass()),
            typeof(SimpleClass),
            new PSObject(typeof(SimpleClass)),
            new DerivedClass(),
            new PSObject(new DerivedClass()),
            typeof(DerivedClass),
            new PSObject(typeof(DerivedClass)),
        ],
        [null, typeof(ConcreteBaseClass)]);

    [Theory]
    [MemberData(nameof(Unregistered_MatrixData))]
    public void Unregistered_PassesThru(object? instance, Type? fallbackType)
    {
        Reflector reflector = new();
        reflector.Register([typeof(SimpleStruct)], [new("Method1")]);
        object? obj = reflector.Expose(instance);
        Assert.Same(instance, obj);
        // TODO Current impl requires fallbackType to be registered if instance is not a type.
        //      It does *not* require instance to be castable to fallbackType.
        //      Review impl and testdata.
        //if (fallbackType is not null)
        //{
        //    obj = reflector.Expose(instance, fallbackType);
        //    Assert.Same(instance, obj);
        //}
    }

    [Fact]
    public void Exposed_ReturnsNewExposed()
    {
        Reflector reflector = new();
        reflector.Register([typeof(SimpleClass)], [new("Method1")]);
        var instance = new SimpleClass();
        var pso1 = Assert.IsType<PSObject>(reflector.Expose(instance));
        var pso2 = Assert.IsType<PSObject>(reflector.Expose(pso1));
        Assert.NotSame(pso1, pso2);
        Assert.Same(instance, pso1.BaseObject);
        Assert.Same(instance, pso2.BaseObject);
        var psbm1 = Assert.Single(pso1.Members.OfType<IPSBaseMemberInfo>());
        var psbm2 = Assert.Single(pso2.Members.OfType<IPSBaseMemberInfo>());
        Assert.Equal(psbm1, psbm2, IPSBaseMemberInfoComparer.Instance);
    }
}
