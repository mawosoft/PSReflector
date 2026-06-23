# PSReflector

[![PSGallery](https://img.shields.io/powershellgallery/v/PSReflector.svg?logo=powershell&label=PSGallery&color=orange&logoColor=white)](https://www.powershellgallery.com/packages/PSReflector/)
[![CI/CD](https://github.com/mawosoft/PSReflector/actions/workflows/ci.yml/badge.svg)](https://github.com/mawosoft/PSReflector/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Powershell module to declaratively expose non-public type members.

## TODO Features/Usage/Examples

TODO

## Installation

You can install PSReflector from the PowerShell Gallery.

```powershell
Install-Module -Name PSReflector
```

### CI Feed

To install the latest build from the [CI feed](https://dev.azure.com/mawosoft-de/public/_packaging?_a=feed&feed=public):

```powershell
  Register-PSRepository -Name mawosoft-nightly -SourceLocation https://pkgs.dev.azure.com/mawosoft-de/public/_packaging/public/nuget/v2/
  Install-Module -Name PSReflector -Repository mawosoft-nightly -AllowPrerelease -Force
```
