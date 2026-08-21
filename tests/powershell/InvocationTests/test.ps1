# Copyright (c) Matthias Wolf, Mawosoft.

using namespace InvocationTestFixture

[CmdletBinding()]
param()

Add-Type -LiteralPath "$PSScriptRoot/../../../artifacts/bin/Mawosoft.PSReflector/debug/Mawosoft.PSReflector.dll"
Add-Type -LiteralPath "$PSScriptRoot/../../../artifacts/bin/InvocationTestFixture/debug/InvocationTestFixture.dll"

. "$PSScriptRoot/RunMethodProvider.sourcegen.ps1"

#[MethodProviderBase]::DebugBreak()

$methodProvider = [DefaultMethodProvider]::new([InvocationMode]::Direct)
RunMethodProvider $methodProvider

$methodProvider = [DefaultMethodProvider]::new([InvocationMode]::Reflection)
RunMethodProviderWithReflection $methodProvider

$methodProvider = [StaticMethodProvider]::new([InvocationMode]::CodeMethod)
$methodProvider = [PSCodeMethodHelper]::AddCodeMethods($methodProvider)
RunMethodProvider $methodProvider

$methodProvider = [StaticMethodProvider]::new([InvocationMode]::CodeMethodForcedReflection)
$methodProvider = [PSCodeMethodHelper]::AddCodeMethods($methodProvider)
[PSCodeMethodHelper]::SetUseReflection($methodProvider, $true)
RunMethodProvider $methodProvider

$reflector = [Mawosoft.PSReflector.Reflector]::new()
$methodProvider = [NonPublicMethodProvider]::new([InvocationMode]::PSReflector)
$reflector.Register([NonPublicMethodProvider], $methodProvider.GetTestMethodNames())
$methodProvider = $reflector.Expose($methodProvider)
RunMethodProvider $methodProvider

$null = $null

<# TODO analysis
    - Logs against baseline log ([InvocationMode]::Direct)
    - Exit args/retval vs After args/retval
    - Before/After identities
#>
