$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$entry = Join-Path $PSScriptRoot 'xyt-incident.ps1'
$tmp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-incident-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tmp | Out-Null
function Invoke-Case($action, $data) {
    $input = Join-Path $tmp ([guid]::NewGuid().ToString('N') + '.json')
    $data | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $input -Encoding UTF8
    $raw = & pwsh -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry -Action $action -InputPath $input 2>&1
    if ($LASTEXITCODE -ne 0) { throw "case $action failed: $raw" }
    return ($raw -join "`n" | ConvertFrom-Json)
}
function Assert($condition, $message) { if (-not $condition) { throw $message } }
try {
    $t0 = Invoke-Case evaluate @{ severity = 'T0'; signals = @() }
    Assert ($t0.severity -eq 'T0' -and $t0.lock -eq 'T0-LOCK') 'T0 lock missing'
    $escalated = Invoke-Case evaluate @{ severity = 'T1'; signals = @('REWORK') }
    Assert ($escalated.severity -eq 'T0' -and $escalated.escalated -eq $true) 'T1 did not escalate'
    $locked = Invoke-Case dependency @{ incidentPaths = @('XYT/Incident/a'); changedPaths = @('XYT/Incident/a'); projectDependency = 'INDEPENDENT'; capabilityMapping = 'INDEPENDENT'; actualDiff = 'TOUCHED' }
    Assert ($locked.decision -eq 'DEPENDENCY-LOCK') 'dependency lock missing'
    $uncertain = Invoke-Case dependency @{ incidentPaths = @('XYT/Incident/a'); changedPaths = @('other/a'); projectDependency = 'UNKNOWN'; capabilityMapping = 'UNKNOWN'; actualDiff = 'UNKNOWN' }
    Assert ($uncertain.decision -eq 'DEPENDENCY-UNCERTAIN' -and $uncertain.notifyUser) 'uncertain dependency missing'
    $override = Invoke-Case unlock @{ severity = 'T0'; userOverride = $true }
    Assert ($override.decision -eq 'USER OVERRIDE' -and $override.lock -eq 'RELEASED') 'override failed'
    $unlocked = Invoke-Case unlock @{ severity = 'T0'; rootCauseConfirmed = $true; controlledFix = $true; minimumTests = 'PASS' }
    Assert ($unlocked.decision -eq 'UNLOCKED') 'normal unlock failed'
    $resume = Invoke-Case resume @{ upstreamLockReleased = $true; interfaceChanged = $false }
    Assert ($resume.decision -eq 'AUTO-RESUME') 'auto resume failed'
    $stayLocked = Invoke-Case resume @{ upstreamLockReleased = $true; interfaceChanged = $true }
    Assert ($stayLocked.decision -eq 'CONTINUE-LOCKED') 'changed dependency resumed incorrectly'
    'XYT INCIDENT SELFTEST: PASS'
    exit 0
}
finally { Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue }
