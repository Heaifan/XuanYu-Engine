[CmdletBinding()]
param(
    [Parameter(Position = 0)][string]$Command = '', [string]$TestMode, [string]$TestSetVersion,
    [string]$AffectedCapability, [ValidateSet('PASS','FAIL','BLOCKED','TIMEOUT','FLAKY')][string]$Status = 'PASS',
    [string[]]$Evidence, [string]$Timestamp, [string]$OutputRoot, [string]$InputRoot,
    [ValidateSet('month','quarter','year')][string]$Period, [string]$At, [string]$ReportPath,
    [string]$AggregatePath, [string]$ChangeType, [string]$EventId, [string]$CurrentVersion,
    [string]$CandidateId, [string]$CandidateFingerprint, [string]$TestCommand,
    [switch]$DryRun, [switch]$FormalAcceptance, [switch]$RequireClean
)
$ErrorActionPreference = 'Stop'
$router = Join-Path $PSScriptRoot 'XYT\xyt-router.ps1'
if (-not (Test-Path -LiteralPath $router -PathType Leaf)) { throw "XYT router is missing: $router" }
& $router @PSBoundParameters
exit $LASTEXITCODE
