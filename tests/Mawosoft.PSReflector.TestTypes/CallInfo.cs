// Copyright (c) Matthias Wolf, Mawosoft.

using System.Runtime.CompilerServices;

namespace Mawosoft.PSReflector.TestTypes;

public sealed class CallInfo
{
    public object?[]? Arguments { get; }
    public string MemberName { get; }
    public string FilePath { get; }
    public int LineNumber { get; }

    public CallInfo(
        object?[]? arguments,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        Arguments = arguments;
        MemberName = memberName ?? "";
        FilePath = filePath ?? "";
        LineNumber = lineNumber;
    }

    public override string ToString()
    {
        return $"{MemberName}({string.Join(",", Arguments ?? [])}) at {LineNumber} in {FilePath}";
    }
}
