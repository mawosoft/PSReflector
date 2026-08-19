// Copyright (c) Matthias Wolf, Mawosoft.

namespace InvocationTestFixture.Logging;

/// <remarks>
/// Positive values identify actual objects.<br/>
/// Negative values identify special objects.<br/>
/// None (0) is either uninitialized or used if no meaningful id can exist.<br/>
/// </remarks>
public enum ObjectId
{
    None = 0,
    NullNull = -1,
    AutoNull = -2,
    DbNull = -3,
    Missing = -4,
}
