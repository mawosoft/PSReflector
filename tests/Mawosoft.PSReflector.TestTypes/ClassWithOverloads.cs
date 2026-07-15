// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

#pragma warning disable CA1708 // Identifiers should differ by more than case
#pragma warning disable CA1822 // Mark members as static
public class ClassWithOverloads
{
    public ClassWithOverloads() { }
    internal ClassWithOverloads(int _, string _1) { }
    internal ClassWithOverloads(int[] _) { }

    internal void Method1() { }

    internal void Overload1(int _) { }
    internal void Overload1(int _, int _1) { }

    internal void IgnoreCase1(int _) { }
    internal void Ignorecase1(int _) { }
    internal void Ignorecase1(int _, int _1) { }
}
