// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

#pragma warning disable CA1822 // Mark members as static

public abstract class AbstractBaseClass
{
    protected AbstractBaseClass() { }
}

public class ConcreteBaseClass : AbstractBaseClass
{
    public ConcreteBaseClass() { }
    internal ConcreteBaseClass(int _) { }

    internal void Method1() { }
}

public class DerivedClass : ConcreteBaseClass
{
    public DerivedClass() { }
    internal DerivedClass(int _) { }
}
