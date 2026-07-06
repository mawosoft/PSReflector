// Copyright (c) Matthias Wolf, Mawosoft.

namespace Mawosoft.PSReflector.TestTypes;

/// <summary>
/// Defines functionality to access members by name.
/// </summary>
public interface IMemberAccessor
{
    /// <summary>Gets the value of the named member.</summary>
    public T GetMemberValue<T>(string memberName);

    /// <summary>Sets the value of the named member.</summary>
    /// <returns>
    /// The value of the member. If the member is writable, this is the new value, otherwise it is the old value.
    /// </returns>
    public T SetMemberValue<T>(string memberName, T value);
}
