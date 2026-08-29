// Copyright (c) Matthias Wolf, Mawosoft.

using System.Runtime.CompilerServices;

namespace Mawosoft.PSReflector.TestTypes;

// Note: Named 'LoggedCallInfo' to avoid confusion with System.Dynamic.CallInfo.
public sealed class LoggedCallInfo
{
    public object?[]? Arguments { get; }
    public string MemberName { get; }
    public string FilePath { get; }
    public int LineNumber { get; }

    public LoggedCallInfo(
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
