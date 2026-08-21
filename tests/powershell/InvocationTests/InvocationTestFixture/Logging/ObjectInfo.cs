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

    public int Id => field == 0 && Value is null ? (int)SpecialObjectId.Missing : field;

    public string? Value { get; }

    internal ObjectInfo(int id, object? obj)
    {
        Id = id;
        Value = id == (int)SpecialObjectId.Missing
            ? null
            : id < 0 ? string.Format(null, NilXml, (SpecialObjectId)id) : PSSerializer.Serialize(obj);
    }

    public override string ToString()
    {
        return Id < 0 || Value is null
        ? ((SpecialObjectId)Id).ToString()
        : $"{Id}: {PSSerializer.Deserialize(Value)}\n{Value}";
    }
}
