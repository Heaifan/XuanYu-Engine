[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$gate = Join-Path $PSScriptRoot 'regression-witness-gate.ps1'
$dir = Join-Path ([System.IO.Path]::GetTempPath()) ('regression-witness-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $dir | Out-Null
try {
    function Run-Case([string]$Name, [hashtable]$Patch, [string]$Needle) {
        $base = [ordered]@{ BugId='SELFTEST'; BugSymptom='symptom'; PreFixBaseline='pre'; WitnessTest='same.test'; PreFixResult='FAIL'; PostFixBaseline='post'; PostFixResult='PASS'; RequiredEvidenceTier='E2'; HarnessStatus='PASS'; RootCauseClassification='PRODUCT'; RegressionScope='selftest' }
        foreach ($key in $Patch.Keys) { $base[$key] = $Patch[$key] }
        $path = Join-Path $dir ($Name + '.json')
        $base | ConvertTo-Json | Set-Content -LiteralPath $path
        $output = & $gate -InputPath $path | Out-String
        if ($output -notmatch [regex]::Escape($Needle)) { throw "$Name expected $Needle, got $output" }
        Write-Output "PASS $Name"
    }
    Run-Case 'red-green' @{} 'REGRESSION WITNESS: COMPLETE'
    Run-Case 'no-pre-fix' @{ PreFixResult='NOT_EXECUTED'; PostFixResult='PASS' } 'GATE STATUS: BLOCKED'
    Run-Case 'retroactive' @{ Retroactive=$true; RetroactiveReason='fixed before witness'; PreFixResult='NOT_EXECUTED'; PostFixResult='PASS' } 'SAVED / WITNESS != COMPLETE'
    Run-Case 'harness-fail' @{ HarnessStatus='FAIL' } 'GATE STATUS: EVIDENCE INVALID'
    Run-Case 'mixed' @{ RootCauseClassification='MIXED' } 'PRODUCT + TEST_HARNESS'
    Run-Case 'unknown' @{ RootCauseClassification='UNKNOWN' } 'GATE STATUS: BLOCKED'
    Write-Output 'REGRESSION WITNESS GATE SELFTEST: PASS'
}
finally { Remove-Item -LiteralPath $dir -Recurse -Force -ErrorAction SilentlyContinue }
