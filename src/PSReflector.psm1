# Copyright (c) Matthias Wolf, Mawosoft.

<#
.SYNOPSIS
    Creates a new instance of [Mawosoft.PSReflector.Reflector].
.NOTES
    Using this cmdlet forces an installed PSReflector module to load.
    Alternatively, you can use 'Import-Module' or 'using module' and invoke the class constructor.
.OUTPUTS
    [Mawosoft.PSReflector.Reflector] - A new instance of this type.
#>
function New-Reflector {
    [CmdletBinding(HelpUri = 'https://github.com/mawosoft/PSReflector/blob/master/docs/PSReflector.md')]
    [OutputType([Mawosoft.PSReflector.Reflector])]
    param()
    [Mawosoft.PSReflector.Reflector]::new()
}

Export-ModuleMember -Function @('New-Reflector')
