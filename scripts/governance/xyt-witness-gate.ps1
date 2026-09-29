[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$InputPath,
    [string]$JsonPath,
    [string]$MarkdownPath
)

$ErrorActionPreference = 'Stop'
$module = Join-Path $PSScriptRoot '..\..\XYT\Witness\xyt-witness.ps1'
if (-not (Test-Path -LiteralPath $module -PathType Leaf)) { throw 'XYT Witness module is missing.' }
. $module
if ([string]::IsNullOrWhiteSpace($JsonPath)) { $JsonPath = [IO.Path]::ChangeExtension($InputPath, '.witness.json') }
if ([string]::IsNullOrWhiteSpace($MarkdownPath)) { $MarkdownPath = [IO.Path]::ChangeExtension($InputPath, '.witness.md') }
$report = Write-XytWitnessReport -InputPath $InputPath -JsonPath $JsonPath -MarkdownPath $MarkdownPath
Write-Output "WITNESS ID: $($report.WitnessId)"
Write-Output "WITNESS STATUS: $($report.Status)"
Write-Output "JSON REPORT: $JsonPath"
Write-Output "MARKDOWN REPORT: $MarkdownPath"
