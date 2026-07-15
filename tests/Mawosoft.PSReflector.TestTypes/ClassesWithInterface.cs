// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

#pragma warning disable CA1822 // Mark members as static

public interface INonGenericInterface
{
    public void NonGenericMethod();
}

public interface IGenericInterface<T>
{
    public T GenericMethod(T value);
}

public interface IExplicitInterface
{
    internal string ExplicitMethod();
}

public class ClassWithNonGenericInterface : INonGenericInterface
{
    internal void Method1() { }
    public void NonGenericMethod() { }
}

public class ClassWithGenericInterface : IGenericInterface<int>
{
    internal void Method1() { }
    public int GenericMethod(int value) => value;
}

public sealed class ClassWithExplicitInterface : IExplicitInterface
{
    internal void Method1() { }
    string IExplicitInterface.ExplicitMethod() => nameof(ClassWithExplicitInterface);
}

public sealed class ClassWithMultipleInterfaces : INonGenericInterface, IGenericInterface<int>, IExplicitInterface
{
    internal void Method1() { }
    public void NonGenericMethod() { }
    public int GenericMethod(int value) => value;
    string IExplicitInterface.ExplicitMethod() => nameof(ClassWithMultipleInterfaces);
}
