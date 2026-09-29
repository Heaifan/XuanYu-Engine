$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$entry = Join-Path $root 'xyt.ps1'

function Assert-Contains([string]$Text, [string]$Expected) {
    if ($Text -notlike "*$Expected*") {
        throw "Expected output to contain '$Expected'. Output: $Text"
    }
}

function Invoke-Xyt([string[]]$Arguments) {
    $output = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry @Arguments 2>&1
    [pscustomobject]@{ ExitCode = $LASTEXITCODE; Output = ($output -join "`n") }
}

$quick = Invoke-Xyt @()
if ($quick.ExitCode -ne 0) { throw "Quick mode failed with $($quick.ExitCode): $($quick.Output)" }
Assert-Contains $quick.Output 'Mode: QUICK_VALIDATION'
Assert-Contains $quick.Output 'Status: EMPTY_RUN'
Assert-Contains $quick.Output 'XYT STARTUP REPORT'

$module = Invoke-Xyt @('module')
if ($module.ExitCode -ne 0) { throw "Module mode failed with $($module.ExitCode): $($module.Output)" }
Assert-Contains $module.Output 'Mode: MODULE_CLOSEOUT'

$global = Invoke-Xyt @('全局收口')
if ($global.ExitCode -ne 0) { throw "Global mode failed with $($global.ExitCode): $($global.Output)" }
Assert-Contains $global.Output 'Mode: GLOBAL_CLOSEOUT'

$invalid = Invoke-Xyt @('unknown')
if ($invalid.ExitCode -eq 0) { throw 'Unknown mode unexpectedly succeeded.' }
Assert-Contains $invalid.Output 'Status: INVALID_MODE'

$versionGate = Invoke-Xyt @('version-gate', '-ChangeType', 'FIX', '-EventId', 'missing', '-CurrentVersion', 'v0.3.0.0-r1', '-CandidateId', 'CAND', '-CandidateFingerprint', 'CAND@v0.3.0.0-r1', '-FormalAcceptance')
if ($versionGate.ExitCode -eq 0) { throw 'Version gate unexpectedly passed missing event.' }
Assert-Contains $versionGate.Output 'GATE STATUS: BLOCKED'

$routerLines = @(Get-Content -LiteralPath $entry).Count
if ($routerLines -gt 100) { throw "xyt.ps1 exceeds 5+100: $routerLines lines." }
if (-not (Test-Path -LiteralPath (Join-Path $root 'XYT\xyt-router.ps1'))) { throw 'Thin router target is missing.' }

Write-Output 'XYT SELFTEST PASS'
exit 0
