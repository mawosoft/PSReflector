// Copyright (c) Matthias Wolf, Mawosoft.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InvocationTestFixture.Arguments;

namespace InvocationTestFixture;

// TODO Do we need extra methods for conversion tests?
public sealed class DefaultMethodProvider : MethodProviderBase
{
    public DefaultMethodProvider(InvocationMode invocationMode) : base(invocationMode) { }

    public void SimpleParam(ClassArg? p1, StructArg p2, EnumArg p3) => LogEnter(this, [p1, p2, p3]);
    public void NullableParam(StructArg? p1, EnumArg? p2) => LogEnter(this, [p1, p2]);

    public void Optional_Default(
        StructArg p1 = default, StructArg? p2 = default,
        EnumArg p3 = default, EnumArg? p4 = default)
    {
        LogEnter(this, [p1, p2, p3, p4]);
    }

    public void Optional_NoDefault(
        [Optional] StructArg p1, [Optional] StructArg? p2,
        [Optional] EnumArg p3, [Optional] EnumArg? p4,
        string? p5)
    {
        LogEnter(this, [p1, p2, p3, p4, p5]);
    }

    // PowerShell only special-cases params for arrays.
    // params IEnumerable<> can work if the caller provides an explicit cast.
    // params ReadOnlySpan<> is not supported.

    public void ParamsArray_Class(params ClassArg?[] p1) => LogEnter(this, [p1]);
    public void ParamsArray_Struct(params StructArg[] p1) => LogEnter(this, [p1]);
    public void ParamsArray_Enum(params EnumArg[] p1) => LogEnter(this, [p1]);
    public void ParamsArray_NullableStruct(params StructArg?[] p1) => LogEnter(this, [p1]);
    public void ParamsArray_NullableEnum(params EnumArg?[] p1) => LogEnter(this, [p1]);

    public void ByRef_Replace_Class(in ClassArg? p1, ref ClassArg? p2, out ClassArg? p3)
    {
        Unsafe.SkipInit(out p3);
        LogEnter(this, [p1, p2, p3]);
        p2 = new ClassArg(p2?.Name, "p2 replaced");
        p3 = new ClassArg(null, "p3 replaced");
        LogExit(this, [p1, p2, p3]);
    }

    public void ByRef_Replace_Struct(in StructArg p1, ref StructArg p2, out StructArg p3)
    {
        Unsafe.SkipInit(out p3);
        LogEnter(this, [p1, p2, p3]);
        p2 = new StructArg(p2.Name, "p2 replaced");
        p3 = new StructArg(null, "p3 replaced");
        LogExit(this, [p1, p2, p3]);
    }

    public void ByRef_Replace_Enum(in EnumArg p1, ref EnumArg p2, out EnumArg p3)
    {
        Unsafe.SkipInit(out p3);
        LogEnter(this, [p1, p2, p3]);
        p2 = EnumArg.Two;
        p3 = EnumArg.Three;
        LogExit(this, [p1, p2, p3]);
    }

    public void ByRef_Replace_NullableStruct(in StructArg? p1, ref StructArg? p2, out StructArg? p3)
    {
        Unsafe.SkipInit(out p3);
        LogEnter(this, [p1, p2, p3]);
        p2 = new StructArg(p2.HasValue ? p2.Value.Name : null, "p2 replaced");
        p3 = new StructArg(null, "p3 replaced");
        LogExit(this, [p1, p2, p3]);
    }

    public void ByRef_Replace_NullableEnum(in EnumArg? p1, ref EnumArg? p2, out EnumArg? p3)
    {
        Unsafe.SkipInit(out p3);
        LogEnter(this, [p1, p2, p3]);
        p2 = EnumArg.Two;
        p3 = EnumArg.Three;
        LogExit(this, [p1, p2, p3]);
    }

    public void ByRef_Mutate(ref StructArg p1, ref StructArg? p2)
    {
        LogEnter(this, [p1, p2]);
        p1.Value = "p1 mutated";
        if (p2.HasValue)
        {
            var p2v = p2.Value;
            p2v.Value = "p2 mutated copyback";
            p2 = p2v;
        }
        LogExit(this, [p1, p2]);
    }
}
