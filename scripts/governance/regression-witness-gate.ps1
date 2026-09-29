[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $InputPath
)

$ErrorActionPreference = 'Stop'
$required = @('BugId','BugSymptom','PreFixBaseline','WitnessTest','PreFixResult','PostFixBaseline','PostFixResult','RequiredEvidenceTier','HarnessStatus','RootCauseClassification','RegressionScope')
$valid = @{
    PreFixResult = @('PASS','FAIL','NOT_EXECUTED','INVALID')
    PostFixResult = @('PASS','FAIL','NOT_EXECUTED','INVALID')
    RequiredEvidenceTier = @('E1','E2','E3')
    HarnessStatus = @('PASS','FAIL','NOT_RUN')
    RootCauseClassification = @('PRODUCT','TEST_HARNESS','MIXED','UNKNOWN')
}

function Fail-Gate([string] $Message) { throw "REGRESSION WITNESS GATE: $Message" }
if (-not (Test-Path -LiteralPath $InputPath -PathType Leaf)) { Fail-Gate "input not found: $InputPath" }
$record = Get-Content -Raw -LiteralPath $InputPath | ConvertFrom-Json
foreach ($name in $required) {
    if ($null -eq $record.PSObject.Properties[$name] -or [string]::IsNullOrWhiteSpace([string]$record.$name)) { Fail-Gate "missing required field: $name" }
}
foreach ($name in $valid.Keys) {
    if ($valid[$name] -notcontains [string]$record.$name) { Fail-Gate "invalid ${name}: $($record.$name)" }
}
$retroactive = $false
if ($null -ne $record.PSObject.Properties['Retroactive']) { $retroactive = [bool]$record.Retroactive }
if ($retroactive -and [string]$record.PreFixResult -ne 'NOT_EXECUTED') { Fail-Gate 'RETROACTIVE requires PRE-FIX NOT_EXECUTED' }
if ($retroactive -and ($null -eq $record.PSObject.Properties['RetroactiveReason'] -or [string]::IsNullOrWhiteSpace([string]$record.RetroactiveReason))) { Fail-Gate 'RETROACTIVE requires RetroactiveReason' }
$witness = 'INCOMPLETE'
$status = 'BLOCKED'
if ($record.HarnessStatus -eq 'FAIL') { $witness = 'EVIDENCE INVALID'; $status = 'EVIDENCE INVALID' }
elseif ($record.HarnessStatus -eq 'NOT_RUN') { $witness = 'INCOMPLETE'; $status = 'BLOCKED' }
elseif ($record.RootCauseClassification -eq 'UNKNOWN') { $witness = 'INCOMPLETE'; $status = 'BLOCKED' }
elseif ($retroactive) { $witness = 'RETROACTIVE'; $status = 'SAVED / WITNESS != COMPLETE' }
elseif ($record.PreFixResult -eq 'FAIL' -and $record.PostFixResult -eq 'PASS') { $witness = 'COMPLETE'; $status = 'PASS' }
elseif ($record.PreFixResult -eq 'PASS' -or $record.PostFixResult -eq 'PASS') { $witness = 'COVERAGE TEST'; $status = 'BLOCKED' }
else { $witness = 'CHARACTERIZATION TEST'; $status = 'BLOCKED' }
if ($record.RootCauseClassification -eq 'UNKNOWN') { $status = 'BLOCKED' }
Write-Output "REGRESSION WITNESS: $witness"
Write-Output "PRE-FIX RESULT: $($record.PreFixResult)"
Write-Output "POST-FIX RESULT: $($record.PostFixResult)"
Write-Output "RETROACTIVE: $retroactive"
Write-Output "HARNESS INTEGRITY: $($record.HarnessStatus)"
Write-Output "ROOT CAUSE CLASSIFICATION: $($record.RootCauseClassification)"
Write-Output "REQUIRED EVIDENCE TIER: $($record.RequiredEvidenceTier)"
Write-Output "GATE STATUS: $status"
if ($record.RootCauseClassification -eq 'MIXED') { Write-Output 'ROOT CAUSE NOTE: PRODUCT + TEST_HARNESS; neither finding is suppressed' }
