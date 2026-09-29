[CmdletBinding()]
param([string]$LedgerPath)
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($LedgerPath)) { $LedgerPath = Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) 'docs\governance\version-events.tsv' }
if (-not (Test-Path $LedgerPath)) { throw "Ledger not found: $LedgerPath" }
$rows = @(Import-Csv -Delimiter "`t" -Path $LedgerPath | Where-Object {
    $_.Type -in @('FEATURE','FIX','STABILIZATION','GOVERNANCE') -and (!$_.Status -or $_.Status -eq 'APPLIED')
})
$features = @($rows | Where-Object Type -eq FEATURE).Count
$fixes = @($rows | Where-Object Type -eq FIX).Count
$ratio = if ($features) { '{0:N2}' -f ($fixes / $features) } else { 'N/A' }
Write-Output 'DEVELOPMENT FRICTION REVIEW'
Write-Output "Feature Count: $features"
Write-Output "Fix Count: $fixes"
Write-Output "Fix / Feature: $ratio"
Write-Output 'Domain        Feature  Fix  Fix/Feature'
foreach ($group in @($rows | Group-Object Domain | Sort-Object Name)) {
    $f = @($group.Group | Where-Object Type -eq FEATURE).Count
    $x = @($group.Group | Where-Object Type -eq FIX).Count
    $r = if ($f) { '{0:N2}' -f ($x / $f) } else { 'N/A' }
    Write-Output ('{0,-13} {1,7} {2,4} {3,11}' -f $group.Name,$f,$x,$r)
}
Write-Output 'Historical Coverage: HISTORICAL PARTIAL (only ledgered evidence is counted)'
Write-Output "Knowledge Created: $(@($rows | Where-Object { $_.Knowledge -and $_.Knowledge -ne '-' }).Count)"
Write-Output "EXP Created: $(@($rows | Where-Object { $_.EXP -and $_.EXP -ne '-' }).Count)"
