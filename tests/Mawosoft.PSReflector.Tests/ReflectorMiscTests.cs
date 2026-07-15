// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using Mawosoft.PSReflector.TestTypes;
using Xunit;

namespace Mawosoft.PSReflector;

public class ReflectorMiscTests
{
    [Fact]
    public void RegisteredTypes_Succeeds()
    {
        Reflector reflector = new();
        Assert.Empty(reflector.RegisteredTypes);
        HashSet<Type> types = [];
        reflector.Register([typeof(SimpleClass)], [new("Method1")]);
        types.Add(typeof(SimpleClass));
        Assert.Equal(types, [.. reflector.RegisteredTypes]);
        reflector.Register([typeof(SimpleStruct)], [new("Method1")]);
        types.Add(typeof(SimpleStruct));
        Assert.Equal(types, [.. reflector.RegisteredTypes]);
    }

    [Fact]
    public void IsTypeRegistered_Succeeds()
    {
        Reflector reflector = new();
        Assert.False(reflector.IsTypeRegistered(typeof(SimpleClass)));
        reflector.Register([typeof(SimpleClass)], [new("Method1")]);
        Assert.True(reflector.IsTypeRegistered(typeof(SimpleClass)));
        Assert.False(reflector.IsTypeRegistered(typeof(SimpleStruct)));
        reflector.Register([typeof(SimpleStruct)], [new("Method1")]);
        Assert.True(reflector.IsTypeRegistered(typeof(SimpleClass)));
        Assert.True(reflector.IsTypeRegistered(typeof(SimpleStruct)));
    }

    [Fact]
    public void AdditionalTypes_Succeeds()
    {
        Reflector reflector = new();
        Assert.Empty(reflector.AdditionalTypes);
        HashSet<Type> types = [];
        reflector.Register([typeof(DerivedClass)], [new("Method1")]);
        types.Add(typeof(AbstractBaseClass));
        Assert.Equal(types, [.. reflector.AdditionalTypes]);
        reflector.Register([typeof(ClassWithMultipleInterfaces)], [new("Method1")]);
        types.UnionWith(
        [
            typeof(INonGenericInterface),
            typeof(IGenericInterface<int>),
            typeof(IExplicitInterface)
        ]);
        Assert.Equal(types, [.. reflector.AdditionalTypes]);
    }
}
