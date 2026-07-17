# PSReflector Module

PowerShell module to declaratively expose non-public type members.

*PSReflector* greatly simplifies common reflection operations. This is done in two steps:

- Instead of calling methods like `GetProperty()`, `GetField()`, or `GetMethod()` and dealing with `BindingFlags` and parameter types, you simply specify a type and a list of non-public members you want to access. In most cases, you have to provide only the member names. For overloads, adding the parameter count is usually sufficient. All this is done via the [Reflector.Register()] method.

- Instead of calling the `GetValue()`, `SetValue()`, and `Invoke()` reflection methods, you pass the object whose type you've previously registered to the [Reflector.Expose()] method, which wraps it in a [PSObject] with the registered members added as public instance properties and methods. You then use the familiar dot-notation to access them, e.g., `$myObj.Foo`. In the case of static classes, you can pass the type itself to [Expose()].

*PSReflector* can expose static and instance members, including instance constructors. If a registered member returns an object of a registered type, it will already have passed through [Expose()] and you can access its non-public members right away.

## Installation

You can install PSReflector from the PowerShell Gallery.

```powershell
Install-PSResource -Name PSReflector
```

## PSReflector Cmdlets

### New-Reflector

Creates a new instance of the *Reflector* class.

```powershell
New-Reflector
```

#### Outputs

[Mawosoft.PSReflector.Reflector]

#### Notes

Using this cmdlet forces an installed *PSReflector* module to load. Alternatively, you can use `Import-Module` or `using module` and invoke the class constructor.

[PSObject]: https://learn.microsoft.com/dotnet/api/system.management.automation.psobject
[Mawosoft.PSReflector.Reflector]: Reflector.md
[Reflector.Register()]: Reflector.md#register
[Reflector.Expose()]: Reflector.md#expose
[Expose()]: Reflector.md#expose
