[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('Report','Aggregate','Upload')][string]$Operation,
    [string]$RepositoryRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$OutputRoot, [string]$InputRoot, [ValidateSet('month','quarter','year')][string]$Period, [string]$At,
    [string]$TestMode, [string]$TestSetVersion, [string[]]$AffectedCapability, [ValidateSet('PASS','FAIL','BLOCKED','TIMEOUT','FLAKY')][string]$Status = 'PASS', [string[]]$Evidence, [string]$Timestamp,
    [string]$ReportPath, [string]$AggregatePath, [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
$module = Join-Path $RepositoryRoot 'XYT\Report\xyt-report.ps1'
if (-not (Test-Path -LiteralPath $module)) { throw 'XYT Report module is missing.' }
. $module
$arguments = @{ Operation = $Operation; RepositoryRoot = $RepositoryRoot; Status = $Status; DryRun = $DryRun }
foreach ($name in @('OutputRoot','InputRoot','Period','At','TestMode','TestSetVersion','Evidence','Timestamp','ReportPath','AggregatePath')) {
    $value = Get-Variable -Name $name -ValueOnly
    if ($null -ne $value -and ((@($value).Count -gt 0) -and (-not [string]::IsNullOrWhiteSpace([string]$value)))) { $arguments[$name] = $value }
}
if ($null -ne $AffectedCapability -and @($AffectedCapability).Count -gt 0) { $arguments.AffectedCapability = $AffectedCapability }
Invoke-XytReport @arguments
