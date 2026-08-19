// Copyright (c) Matthias Wolf, Mawosoft.

using System.Management.Automation;

namespace InvocationTestFixture.Logging;

public readonly struct ObjectInfo
{
    private const string NilXml = """
        <Objs Version="1.1.0.1" xmlns="http://schemas.microsoft.com/powershell/2004/04">
          <Nil>{0}</Nil>
        </Objs>
        """;

    public ObjectId Id => field == 0 && Value is null ? ObjectId.Missing : field;

    public string? Value { get; }

    internal ObjectInfo(ObjectId id, object? obj)
    {
        Id = id;
        Value = id == ObjectId.Missing
            ? null
            : id < 0 ? string.Format(null, NilXml, id) : PSSerializer.Serialize(obj);
    }

    public override string ToString()
    {
        return Id < 0 || Value is null
        ? Id.ToString()
        : $"{Id}: {PSSerializer.Deserialize(Value)}";
    }
}
