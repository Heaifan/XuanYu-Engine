[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Type,
    [string]$CurrentVersion
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
. (Join-Path $PSScriptRoot 'version-lib.ps1')
if ([string]::IsNullOrWhiteSpace($CurrentVersion)) {
    $props = [xml](Get-Content -Raw (Join-Path $root 'Directory.Build.props'))
    $CurrentVersion = [string]$props.Project.PropertyGroup.InformationalVersion | Select-Object -First 1
}
try {
    $next = Get-NextProcessVersion -Current $CurrentVersion -Type $Type
    Write-Output "Current: $CurrentVersion"
    Write-Output "Change Type: $Type"
    Write-Output "Next: $next"
    Write-Output "Reason: One $Type Version Event advances the audited fourth-field process counter; a numbered FIX chain increments its fix ordinal."
} catch {
    Write-Output 'BLOCKED'
    Write-Output "Reason: $($_.Exception.Message)"
    exit 2
}
