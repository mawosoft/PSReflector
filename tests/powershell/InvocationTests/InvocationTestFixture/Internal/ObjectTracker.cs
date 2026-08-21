// Copyright (c) Matthias Wolf, Mawosoft.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Internal;
using InvocationTestFixture.Logging;

namespace InvocationTestFixture.Internal;

internal sealed class ObjectTracker
{
    private readonly Dictionary<ReferenceEqualsWrapper, int> _tracker = new()
    {
        { new ReferenceEqualsWrapper(null), (int)SpecialObjectId.NullNull },
        { new ReferenceEqualsWrapper(AutomationNull.Value), (int)SpecialObjectId.AutoNull },
        { new ReferenceEqualsWrapper(DBNull.Value), (int)SpecialObjectId.DbNull },
        { new ReferenceEqualsWrapper(Type.Missing), (int)SpecialObjectId.Missing },
    };

    private int _nextId;

    /// <summary>
    /// Gets the unique id for a tracked <paramref name="obj"/>.
    /// Returns 0 if <paramref name="obj"/> hasn't been tracked yet.
    /// </summary>
    /// <remarks>
    /// Caller is responsible for unwrapping a <see cref="PSObject"/> or <see cref="PSReference"/> beforehand.<br/>
    /// Negative id values indicate special objects. See <see cref="SpecialObjectId"/>.<br/>
    /// </remarks>
    public int GetId(object? obj)
    {
        return _tracker.TryGetValue(new ReferenceEqualsWrapper(obj), out var id) ? id : 0;
    }

    /// <summary>
    /// Gets the unique id for <paramref name="obj"/>, adding it to the tracked objects if necessary.
    /// </summary>
    /// <remarks>
    /// Caller is responsible for unwrapping a <see cref="PSObject"/> or <see cref="PSReference"/> beforehand.<br/>
    /// Negative id values indicate special objects. See <see cref="SpecialObjectId"/>.<br/>
    /// This will never return 0 to avoid mix-up with uninitialized fields/properties.<br/>
    /// </remarks>
    public int GetOrAddId(object? obj)
    {
        var w = new ReferenceEqualsWrapper(obj);
        if (!_tracker.TryGetValue(w, out var id))
        {
            id = ++_nextId;
            _tracker.Add(w, id);
        }
        return id;
    }
}
