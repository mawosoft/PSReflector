// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS0649 // Field is never assigned to
public class AutoExposeTestClass
{
    internal INonGenericInterface? INonGenericInterfaceField;
    internal IGenericInterface<int>? IGenericInterfaceField;
    internal IExplicitInterface? IExplicitInterfaceField;
    internal AbstractBaseClass? AbstractBaseClassField;
    internal DerivedClass? DerivedClassField;

    internal INonGenericInterface? INonGenericInterfaceProperty { get; set; }
    internal IGenericInterface<int>? IGenericInterfaceProperty { get; set; }
    internal IExplicitInterface? IExplicitInterfaceProperty { get; set; }
    internal AbstractBaseClass? AbstractBaseClassProperty { get; set; }
    internal DerivedClass? DerivedClassProperty { get; set; }

    internal INonGenericInterface? INonGenericInterfaceMethod(INonGenericInterface? instance) => instance;
    internal IGenericInterface<int>? IGenericInterfaceMethod(IGenericInterface<int>? instance) => instance;
    internal IExplicitInterface? IExplicitInterfaceMethod(IExplicitInterface? instance) => instance;
    internal AbstractBaseClass? AbstractBaseClassMethod(AbstractBaseClass? instance) => instance;
    internal DerivedClass? DerivedClassMethod(DerivedClass? instance) => instance;
}
