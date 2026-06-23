# Copyright (c) Matthias Wolf, Mawosoft.

<#
.SYNOPSIS
    Creates a new instance of [Mawosoft.PSReflector.Reflector].
.NOTES
    This merely exists to force an installed module to load. You can achieve the same with
    'Import-Module' or 'using module' and then use the class constructor.
.OUTPUTS
    [Mawosoft.PSReflector.Reflector] - A new instance of this type.
#>
function New-PSReflector {
    [OutputType([Mawosoft.PSReflector.Reflector])]
    param()
    [Mawosoft.PSReflector.Reflector]::new()
}
