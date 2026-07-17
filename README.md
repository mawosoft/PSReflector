# PSReflector

[![PSGallery](https://img.shields.io/powershellgallery/v/PSReflector.svg?logo=powershell&label=PSGallery&color=orange&logoColor=white)](https://www.powershellgallery.com/packages/PSReflector/)
[![CI/CD](https://github.com/mawosoft/PSReflector/actions/workflows/ci.yml/badge.svg)](https://github.com/mawosoft/PSReflector/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

PowerShell module to declaratively expose non-public type members.

*PSReflector* greatly simplifies common reflection operations. This is done in two steps:

- Instead of calling methods like `GetProperty()`, `GetField()`, or `GetMethod()` and dealing with `BindingFlags` and parameter types, you simply specify a type and a list of non-public members you want to access. In most cases, you have to provide only the member names. For overloads, adding the parameter count is usually sufficient. All this is done via the [Reflector.Register()] method.

- Instead of calling the `GetValue()`, `SetValue()`, and `Invoke()` reflection methods, you pass the object whose type you've previously registered to the [Reflector.Expose()] method, which wraps it in a [PSObject] with the registered members added as public instance properties and methods. You then use the familiar dot-notation to access them, e.g., `$myObj.Foo`. In the case of static classes, you can pass the type itself to [Expose()].

*PSReflector* can expose static and instance members, including instance constructors. If a registered member returns an object of a registered type, it will already have passed through [Expose()] and you can access its non-public members right away.

To learn more, see the [Documentation](docs/PSReflector.md).

## Installation

You can install PSReflector from the PowerShell Gallery.

```powershell
Install-PSResource -Name PSReflector
```

### CI Feed

To install the latest build from the [CI feed](https://dev.azure.com/mawosoft-de/public/_packaging?_a=feed&feed=public):

```powershell
  Register-PSResourceRepository -Name 'mawosoft-nightly' -Uri 'https://pkgs.dev.azure.com/mawosoft-de/public/_packaging/public/nuget/v2/'
  Install-PSResource -Name PSReflector -Repository 'mawosoft-nightly' -Prerelease
```

[PSObject]: https://learn.microsoft.com/dotnet/api/system.management.automation.psobject
[Mawosoft.PSReflector.Reflector]: docs/Reflector.md
[Reflector.Register()]: docs/Reflector.md#register
[Reflector.Expose()]: docs/Reflector.md#expose
[Expose()]: docs/Reflector.md#expose
