// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Runtime.CompilerServices;

namespace InvocationTestFixture.Internal;

/// <remarks>
/// We don't have ReferenceEqualityComparer in netstandard2.0.
/// This serves the same purpose with the additional benefit of bypassing restrictions like TKey : notnull.
/// </remarks>
internal readonly struct ReferenceEqualsWrapper : IEquatable<ReferenceEqualsWrapper>
{
    private readonly object? _object;
    public ReferenceEqualsWrapper(object? obj) => _object = obj;
    public override bool Equals(object? obj) => obj is ReferenceEqualsWrapper other && Equals(other);
    public bool Equals(ReferenceEqualsWrapper other) => ReferenceEquals(_object, other._object);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(_object);
    public static bool operator ==(ReferenceEqualsWrapper left, ReferenceEqualsWrapper right) => left.Equals(right);
    public static bool operator !=(ReferenceEqualsWrapper left, ReferenceEqualsWrapper right) => !(left == right);
}
