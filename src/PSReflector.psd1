# Module manifest for module 'PSReflector'

@{

    RootModule           = 'PSReflector.psm1'
    ModuleVersion        = '@ModuleVersion@'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID                 = '22eca938-7d77-4167-b37e-5762a97d7104'
    Author               = 'Matthias Wolf'
    CompanyName          = 'mawosoft'
    Copyright            = 'Copyright (c) 2026 Matthias Wolf, Mawosoft. All rights reserved.'
    Description          = 'Powershell module to declaratively expose non-public type members.'
    PowerShellVersion    = '5.1'
    NestedModules        = @(
        'Mawosoft.PSReflector.dll'
    )
    FunctionsToExport    = @(
        'New-PSReflector'
    )
    CmdletsToExport      = @()
    VariablesToExport    = @()
    AliasesToExport      = @()
    DscResourcesToExport = @()
    PrivateData          = @{
        PSData = @{
            Tags                     = @('PSEdition_Desktop', 'PSEdition_Core', 'Reflection', 'Class')
            LicenseUri               = 'https://github.com/mawosoft/PSReflector/blob/master/LICENSE'
            ProjectUri               = 'https://github.com/mawosoft/PSReflector'
            Prerelease               = '@Prerelease@'
            RequireLicenseAcceptance = $false
        }
    }
}
