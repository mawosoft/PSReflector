# Reflector Class

Provides functionality to register types and their non-public members for exposure, allowing them to be accessed like public members.

```cs
namespace Mawosoft.PSReflector;

public sealed class Reflector {
  public Reflector();
  public void Register(Type[] types, MemberDescriptor[] members, bool caseSensitive = false);
  public object? Expose(object? obj);
  public object? Expose(object? obj, Type fallbackType);
}
```
## Constructors

| Name        | Description                                      |
|-------------|--------------------------------------------------|
| Reflector() | Creates a new instance of the *Reflector* class. |

The *Reflector* class is intended to be instantiated once and used throughout your PowerShell module or script.
The preferred way to create an instance is by using the [New-Reflector] cmdlet.
This forces an installed *PSReflector* module to load.

## Methods

### Register

```cs
public void Register(Type[] types, MemberDescriptor[] members, bool caseSensitive = false);
```

Registers non-public type members to be exposed like public members via [Expose()].

| Parameter     | Description          |
|---------------|----------------------|
| types         | An array of types whose non-public members are described by *members*. |
| members       | An array of [MemberDescriptor] instances that describe the non-public members to expose. |
| caseSensitive | `true` to perform a case-sensitive search for member names. The default is `false`, which performs a case-insensitive search to match PowerShell's behavior.<br/>If a case-insensitive search is ambiguous, a single case-sensitive match will resolve the ambiguity. |

#### Remarks

Types must be concrete (not abstract classes or interfaces), though static classes are permitted. Furthermore, you cannot register the types [Type] or [PSObject] or types derived from them.

You usually invoke this method once for each type you want to register, but passing an array of types allows you to conveniently register shared interface members across several implementing types in a single call.

You can invoke this method multiple times to cumulatively register different members for the same type.

### Expose

```cs
public object? Expose(object? obj);
public object? Expose(object? obj, Type fallbackType);
```

Returns *obj* as an extended [PSObject] with registered non-public members exposed as public instance properties and methods.

| Parameter    | Description          |
|--------------|----------------------|
| obj          | The object whose non-public members to expose. |
| fallbackType | A previously registered type to use as a fallback if the type of *obj* is not registered. |

#### Returns

- If *obj* is an instance of a registered type, or if it can be cast to *fallbackType*, it is returned as an extended [PSObject] with *obj* as its [BaseObject].
  In addition to the regular members of *obj*, it exposes all registered non-public **instance and static** members (from either its own type or the fallback type) as public **instance** properties and methods.
  Non-public constructors are not added in this case.

- If *obj* is a registered type itself, it is also returned as an extended [PSObject].
  This object is extended with only the registered non-public static members and non-public constructors, but still as instance properties and methods.

- If *obj* is something else (including `null`), it is returned unchanged.

#### Remarks

The returned *PSObject* has the same identity as *obj*. Both will compare as reference-equal in PowerShell, but only the former exposes the extended members.

```powershell
# All of the following evaluate as true.
$returned -eq $obj
[Object]::ReferenceEquals($returned, $obj)
$returned.psobject.BaseObject -eq $obj.psobject.BaseObject # Also ref-equal.
$returned.psobject -ne $obj.psobject
```

## Examples

### Example 1: Create an instance of the Reflector class

You probably want to store the created instance in a variable at script-scope to make it available throughout your module or script.

Via cmdlet:
```powershell
$reflector = New-Reflector
```

Via constructor and `using` statement:
```powershell
using module PSReflector
$reflector = [Mawosoft.PSReflector.Reflector]::new()
```

Via constructor and `Import-Module` cmdlet:
```powershell
# Import by path, assuming the module is not installed.
Import-Module -Name '/path/to/module/PSReflector.psd1'
$reflector = [Mawosoft.PSReflector.Reflector]::new()
```

### Example 2: Register and use interdependent classes

```powershell
$reflector.Register([System.Management.Automation.SessionState], 'Internal')
$reflector.Register(
    [psobject].Assembly.GetType('System.Management.Automation.SessionStateInternal'),
    @(
        'CurrentScope'
        'GlobalScope'
        'ModuleScope'
    ))
$reflector.Register(
    [psobject].Assembly.GetType('System.Management.Automation.SessionStateScope'),
    @(
        'Parent'
        'ScriptScope'
    ))
# Explicitly expose the registered SessionState members.
$sessionState = $reflector.Expose($ExecutionContext.SessionState)
# The Internal property has a registered return type.
# As such, the returned value already has its registered members implicitly exposed.
$sessionStateInternal = $sessionState.Internal
# The same principle applies to the CurrentScope property.
$scope = $sessionStateInternal.CurrentScope
while ($scope) {
    # Do something
    $scope = $scope.Parent
}
```

### Example 3: Register an explicit interface member across multiple types

```powershell
using namespace System.Management.Automation.Language

$reflector.Register(
    @(
      [TypeName]
      [GenericTypeName]
      [ArrayTypeName]
      [ReflectionTypeName]
    ),
    @{
        Name   = 'System.Management.Automation.Language.ISupportsTypeCaching.CachedType'
        PSName = 'CachedType'
    })
# Register an additional member for a previously registered type.
$reflector.Register([TypeName], '_typeDefinitionAst')
```

[PSObject]: https://learn.microsoft.com/dotnet/api/system.management.automation.psobject
[BaseObject]: https://learn.microsoft.com/dotnet/api/system.management.automation.psobject.baseobject
[Type]: https://learn.microsoft.com/dotnet/api/system.type
[MemberDescriptor]: MemberDescriptor.md
[New-Reflector]: PSReflector.md#new-reflector
[Expose()]: #expose
[Register()]: #register
