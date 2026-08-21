// Copyright (c) Matthias Wolf, Mawosoft.

namespace InvocationTestFixture.Arguments;

public interface IArgument
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}

public record class ClassArg : IArgument
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public ClassArg() { }
    public ClassArg(string? name, string? value)
    {
        Name = name;
        Value = value;
    }
}

public record struct StructArg(string? Name, string? Value) : IArgument;

public enum EnumArg
{
    Default,
    // Values to use by caller
    One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    // Values to use by modifying method
    ModifiedOne, ModifiedTwo, ModifiedThree, ModifiedFour, ModifiedFive,
    ModifiedSix, ModifiedSeven, ModifiedEight, ModifiedNine, ModifiedTen,
}
