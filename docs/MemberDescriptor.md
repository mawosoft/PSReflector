# MemberDescriptor Class

Describes a non-public type member to register for exposure. Types and their members are registered via [Reflector.Register()].

```cs
namespace Mawosoft.PSReflector;

public sealed class MemberDescriptor {
  public MemberDescriptor();
  public MemberDescriptor(string name);
  public MemberDescriptor(MemberInfo memberInfo);

  public string? Name { get; set; }
  public string? PSName { get; set; }
  public int? ParamCount { get; set; }
  public MemberInfo? MemberInfo { get; set; }
  public bool CanWrite { get; set; }
}
```

## Constructors

| Name                                    | Description                                       |
|-----------------------------------------|---------------------------------------------------|
| MemberDescriptor()                      | Default. Supports PowerShell [hashtable notation].|
| MemberDescriptor(string name)           | Supports PowerShell conversion from `string`.     |
| MemberDescriptor(MemberInfo memberInfo) | Supports PowerShell conversion from [MemberInfo]. |

In PowerShell, you don't need to instantiate *MemberDescriptor* directly. You can rely on automatic conversion instead.

```powershell
[System.Reflection.MemberInfo]$memberInfo = # Some member obtained via reflection.
[Mawosoft.PSReflector.MemberDescriptor[]]$descriptors = @(
    @{ Name = 'Bar'; PSName = 'Buzz' }, # Describe a member by multiple properties.
    'Foo', # Describe a member by name.
    $memberInfo # Describe a member via reflection.
)
```
## Properties

| Name       | Description          |
|------------|----------------------|
| Name       | The original member name. Constructors are named `.ctor`. Explicit interface implementations must be referred to by the fully qualified name. |
| PSName     | The name to use for the member in PowerShell. The default is the same as *Name* or `new` for constructors. |
| ParamCount | The number of parameters. Can disambiguate constructor and method overloads. The default is unspecified. |
| MemberInfo | A [MemberInfo] obtained via reflection. If specified, it takes precedence over *Name* and *ParamCount*. |
| CanWrite   | Controls write access if the described member is a property or field. The default is read-only. |

## Remarks

The term *non-public* refers to members with an accessibility level of `private`, `protected`, `internal`, or a combination thereof.
The following member types are supported:

- Instance and static fields.
- Instance and static parameterless properties.
- Instance and static methods.
- Instance constructors.

The following member types must be described via their accessor methods:

- Indexer and parameterized properties.
- Properties where only the setter is non-public.
- Events.

Constructors and methods with multiple overloads must be disambiguated.
This applies to both the identification of the original member and the name to use for the exposed member in PowerShell.
If name and parameter count are not sufficient to identify the original member, you must provide a [MemberInfo] obtained via reflection.
If you want to expose multiple overloads, you must assign each of them a different *PSName*.

[MemberInfo]: https://learn.microsoft.com/dotnet/api/system.reflection.memberinfo
[hashtable notation]: https://learn.microsoft.com/powershell/module/microsoft.powershell.core/about/about_object_creation#create-objects-from-hash-tables
[Reflector.Register()]: Reflector.md#register
