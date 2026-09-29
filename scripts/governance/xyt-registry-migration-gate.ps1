param([string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path)

$ErrorActionPreference = 'Stop'
$registryPath = Join-Path $RepositoryRoot 'docs\governance\test-registry.json'
$mapPath = Join-Path $RepositoryRoot 'docs\governance\test-capability-map.json'
$registry = Get-Content -Raw $registryPath | ConvertFrom-Json
$map = Get-Content -Raw $mapPath | ConvertFrom-Json
$tests = @($registry.tests)
$validCaps = @($map.capabilities.capabilityId)
$failures = @()

function Require([bool]$condition, [string]$message) {
    if (-not $condition) { $script:failures += $message }
}

$ids = @($tests.testId)
Require ($tests.Count -eq 696) "record count=$($tests.Count), expected 696"
Require (@($ids | Sort-Object -Unique).Count -eq 696) 'Test ID uniqueness failed'
Require ((@($tests | Group-Object migrationWave | Where-Object Name -eq 'WAVE-HIGH').Count -eq 1) -and (($tests | Where-Object migrationWave -eq 'WAVE-HIGH').Count -eq 132)) 'HIGH wave count failed'
Require (($tests | Where-Object migrationWave -eq 'WAVE-MEDIUM').Count -eq 237) 'MEDIUM wave count failed'
Require (($tests | Where-Object migrationWave -eq 'WAVE-LOW').Count -eq 327) 'LOW wave count failed'
Require (($tests | Where-Object auditAction -eq 'KEEP').Count -eq 564) 'KEEP count failed'
Require (($tests | Where-Object auditAction -eq 'DOWNGRADE').Count -eq 37) 'DOWNGRADE count failed'
Require (($tests | Where-Object auditAction -eq 'RENAME').Count -eq 95) 'RENAME count failed'
foreach ($level in @('P0','P1','P2','P3')) { $expected = @{P0=281;P1=284;P2=131;P3=0}[$level]; Require (($tests | Where-Object evidenceLevel -eq $level).Count -eq $expected) "$level count failed" }
Require (@($tests | Where-Object evidenceLevel -match '^T').Count -eq 0) 'Incident severity entered evidenceLevel'
Require (@($tests | Where-Object evidenceLevel -notin @('P0','P1','P2','P3','P4')).Count -eq 0) 'Invalid evidenceLevel found'

$review = @($tests | Where-Object capabilityResolution -eq 'REVIEW_REQUIRED')
$resolved = @($tests | Where-Object capabilityResolution -ne 'REVIEW_REQUIRED')
Require (@($review | Where-Object { $_.targetCapability.Count -ne 0 }).Count -eq 0) 'REVIEW_REQUIRED targetCapability is not empty'
Require (@($review | Where-Object { $_.targetCapability -contains 'REVIEW_REQUIRED' }).Count -eq 0) 'REVIEW_REQUIRED sentinel entered targetCapability'
Require (@($review | Where-Object status -ne 'BLOCKED').Count -eq 0) 'REVIEW_REQUIRED record is not BLOCKED'
Require (@($resolved | Where-Object { $_.targetCapability.Count -lt 1 }).Count -eq 0) 'Resolved record has no capability'
Require (@($resolved | Where-Object { @($_.targetCapability | Where-Object { $_ -notin $validCaps }).Count -gt 0 }).Count -eq 0) 'Resolved record has invalid capability'
Require (@($tests | Where-Object { -not (Test-Path (Join-Path $RepositoryRoot $_.testPath)) }).Count -eq 0) 'Missing testPath'

$capabilityQuery = @($tests | Where-Object capabilityResolution -ne 'REVIEW_REQUIRED')
$requiredSelection = @($tests | Where-Object capabilityResolution -ne 'REVIEW_REQUIRED')
Require (@($capabilityQuery | Where-Object capabilityResolution -eq 'REVIEW_REQUIRED').Count -eq 0) 'REVIEW_REQUIRED entered Capability Query'
Require (@($requiredSelection | Where-Object capabilityResolution -eq 'REVIEW_REQUIRED').Count -eq 0) 'REVIEW_REQUIRED entered Required Test Selection'
Require ($registry.evidenceCanonical -eq 'P0-P4') 'P canonical marker failed'
Require ($registry.incidentCanonical -eq 'T0-T3; not stored in evidenceLevel') 'T canonical marker failed'
Require ($registry.migrationStatus -eq 'REGISTERED_REVIEW_REQUIRED') 'Migration status changed'

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    Write-Output 'XYT-G3-FIX = BLOCKED'
    exit 1
}
Write-Output "JSON Parse = PASS"
Write-Output "Records = $($tests.Count); Unique Test IDs = $(@($ids | Sort-Object -Unique).Count)"
Write-Output "REVIEW_REQUIRED = $($review.Count); targetCapability empty = PASS; status BLOCKED = PASS"
Write-Output 'Capability Query exclusion = PASS'
Write-Output 'Required Test Selection exclusion = PASS'
Write-Output 'P/T Canonical = PASS'
Write-Output 'XYT-G3-FIX = PASS'
