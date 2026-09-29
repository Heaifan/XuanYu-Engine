[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$module = Join-Path $PSScriptRoot 'xyt-witness.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('xyt-witness-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $root | Out-Null

try {
    . $module
    function New-Case([hashtable]$Changes) {
        $case = [ordered]@{
            WitnessId='WIT-SELF-001'; BugId='BUG-1'; IncidentId='INC-1'
            Capability='selftest'; PreFixCommit='1111111111111111111111111111111111111111'
            PostFixCommit='2222222222222222222222222222222222222222'
            TestId='TEST-RED-GREEN'; PreFixTestId='TEST-RED-GREEN'; PostFixTestId='TEST-RED-GREEN'
            TestSetVersion='xyt-tests-r1'; PreFixResult='FAIL'; PostFixResult='PASS'
            PreFixEvidence='pre.json'; PostFixEvidence='post.json'
        }
        foreach ($key in $Changes.Keys) { $case[$key] = $Changes[$key] }
        return [pscustomobject]$case
    }
    function Assert-Status([string]$name, [hashtable]$changes, [string]$expected) {
        $result = Resolve-XytWitness -Record (New-Case $changes)
        if ($result.Status -ne $expected) { throw "$name expected $expected, got $($result.Status)" }
        Write-Output "PASS $name"
    }
    Assert-Status 'red-green' @{} 'GREEN_CONFIRMED'
    Assert-Status 'pre-fix-unexpected-pass' @{ PreFixResult='PASS' } 'INVALID'
    Assert-Status 'post-fix-fail' @{ PostFixResult='FAIL' } 'INCOMPLETE'
    Assert-Status 'different-test-id' @{ PostFixTestId='TEST-OTHER' } 'INVALID'
    Assert-Status 'missing-commit' @{ PreFixCommit='' } 'INCOMPLETE'
    Assert-Status 'red-only' @{ PostFixResult='NOT_EXECUTED'; PostFixEvidence='' } 'RED_CONFIRMED'

    $input = Join-Path $root 'input.json'
    $jsonPath = Join-Path $root 'report.json'
    $mdPath = Join-Path $root 'report.md'
    (New-Case @{}) | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $input -Encoding utf8
    $report = Write-XytWitnessReport -InputPath $input -JsonPath $jsonPath -MarkdownPath $mdPath
    if (-not (Test-Path $jsonPath) -or -not (Test-Path $mdPath)) { throw 'report files were not saved' }
    if ((Get-Content -Raw $jsonPath | ConvertFrom-Json).Status -ne 'GREEN_CONFIRMED') { throw 'saved JSON status mismatch' }
    if ((Get-Content -Raw $mdPath) -notmatch 'GREEN_CONFIRMED') { throw 'saved Markdown status missing' }
    Write-Output 'PASS evidence-save'
    Write-Output 'XYT WITNESS SELFTEST: PASS'
}
finally { Remove-Item -LiteralPath $root -Recurse -Force -ErrorAction SilentlyContinue }
