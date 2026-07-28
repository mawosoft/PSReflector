// Copyright (c) Matthias Wolf, Mawosoft.

using System.Reflection;

namespace ReflectionBenchmarks;

public static class TestClassMemberAccess
{
    public static TestClassMemberAccess<TMember, TTarget, TAccessor> Create<TMember, TTarget, TAccessor>(TestClassMember<TMember> member, TTarget target, TAccessor accessor) where TMember : MemberInfo
    {
        return new TestClassMemberAccess<TMember, TTarget, TAccessor>(member, target, accessor);
    }
}

public class TestClassMemberAccess<TMember, TTarget, TAccessor> : TestClassMember<TMember> where TMember : MemberInfo
{
    public readonly TTarget Target;
    public readonly TAccessor Accessor;

    public TestClassMemberAccess(TestClassMember<TMember> member, TTarget target, TAccessor accessor) : base(member)
    {
        Target = target;
        Accessor = accessor;
    }
}
