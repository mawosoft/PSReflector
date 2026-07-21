// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable CS0169 // Field is never used
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members

public abstract class AbstractBaseClass
{
    private static int StaticPrivateField1;
    private static int StaticPrivateSameNameField2;
    private protected static int StaticNonPrivateField3;
    public static int StaticPublicField4;

    private static int StaticPrivateProperty1 { get; set; }
    private static int StaticPrivateSameNameProperty2 { get; set; }
    private protected static int StaticNonPrivateProperty3 { get; set; }
    public static int StaticPublicProperty4 { get; set; }
    public static int StaticMixedProperty5 { get; internal set; }

    private static void StaticPrivateMethod1() { }
    private static void StaticPrivateSameNameMethod2() { }
    private protected static void StaticNonPrivateMethod3() { }
    public static void StaticPublicMethod4() { }

    private int PrivateField1;
    private int PrivateSameNameField2;
    private protected int NonPrivateField3;
    public int PublicField4;

    private int PrivateProperty1 { get; set; }
    private int PrivateSameNameProperty2 { get; set; }
    private protected int NonPrivateProperty3 { get; set; }
    public int PublicProperty4 { get; set; }
    public int MixedProperty5 { get; internal set; }

    private void PrivateMethod1() { }
    private void PrivateSameNameMethod2() { }
    private protected void NonPrivateMethod3() { }
    public void PublicMethod4() { }

    protected AbstractBaseClass() { }
}

public class ConcreteBaseClass : AbstractBaseClass
{
    private static int StaticPrivateSameNameField2;
    private static int StaticPrivateSameNameProperty2 { get; set; }
    private static void StaticPrivateSameNameMethod2(int _) { }

    private int PrivateSameNameField2;
    private int PrivateSameNameProperty2 { get; set; }
    private void PrivateSameNameMethod2(int _) { }

    private static int StaticPrivateField10;
    private static int StaticPrivateProperty10 { get; set; }
    private static void StaticPrivateMethod10() { }

    private int PrivateField10;
    private int PrivateProperty10 { get; set; }
    private void PrivateMethod10() { }


    public ConcreteBaseClass() { }
    internal ConcreteBaseClass(int _) { }

    internal void Method1() { }
}

public class DerivedClass : ConcreteBaseClass
{
    private static int StaticPrivateField20;
    private static int StaticPrivateProperty20 { get; set; }
    private static void StaticPrivateMethod20() { }

    private int PrivateField20;
    private int PrivateProperty20 { get; set; }
    private void PrivateMethod20() { }

    public DerivedClass() { }
    internal DerivedClass(int _) { }
    private DerivedClass(int _, int _1) { }
}
