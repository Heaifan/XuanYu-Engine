param(
    [Parameter(Mandatory = $true)][ValidateSet('evaluate', 'dependency', 'unlock', 'resume')][string]$Action,
    [Parameter(Mandatory = $true)][string]$InputPath
)
$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $InputPath -PathType Leaf)) { throw "Input not found: $InputPath" }
$input = Get-Content -Raw -LiteralPath $InputPath | ConvertFrom-Json
function Emit($value) { $value | ConvertTo-Json -Depth 8 -Compress }
function HasEscalationSignal($signals) {
    @('REWORK', 'SCOPE_EXPANSION', 'PARALLEL_BLOCK', 'FALSE_PASS', 'BAD_FIX_CHAIN', 'MAINLINE_IMPACT') | Where-Object { @($signals) -contains $_ } | Select-Object -First 1
}
switch ($Action) {
    'evaluate' {
        $severity = [string]$input.severity
        if ($severity -notin @('T0', 'T1', 'T2', 'T3')) { throw "Invalid severity: $severity" }
        $signal = HasEscalationSignal $input.signals
        $escalated = $severity -eq 'T1' -and $null -ne $signal
        if ($escalated) { $severity = 'T0' }
        Emit ([ordered]@{ severity = $severity; escalated = $escalated; escalationSignal = $signal; lock = $(if ($severity -eq 'T0') { 'T0-LOCK' } else { 'NONE' }); notify = $(if ($severity -in @('T0', 'T1')) { 'IMMEDIATE' } else { 'TASK' }); product = 'UNVERIFIED' })
    }
    'dependency' {
        $overlap = @($input.incidentPaths) | Where-Object { @($input.changedPaths) -contains $_ }
        $positive = $overlap.Count -gt 0 -or @('DEPENDENT', 'TOUCHED') -contains [string]$input.projectDependency -or @('DEPENDENT', 'TOUCHED') -contains [string]$input.capabilityMapping -or [string]$input.actualDiff -eq 'TOUCHED'
        $known = [string]$input.projectDependency -in @('DEPENDENT', 'INDEPENDENT') -and [string]$input.capabilityMapping -in @('DEPENDENT', 'INDEPENDENT') -and [string]$input.actualDiff -in @('TOUCHED', 'NOT_TOUCHED')
        $decision = if ($positive) { 'DEPENDENCY-LOCK' } elseif ($known) { 'CONTINUE' } else { 'DEPENDENCY-UNCERTAIN' }
        Emit ([ordered]@{ decision = $decision; notifyUser = $decision -eq 'DEPENDENCY-UNCERTAIN'; overlap = @($overlap); scope = if ($decision -eq 'CONTINUE') { 'UNLOCKED' } else { 'LOCKED' } })
    }
    'unlock' {
        if ([bool]$input.userOverride) { Emit ([ordered]@{ decision = 'USER OVERRIDE'; lock = 'RELEASED'; audit = 'REQUIRED' }); break }
        $ready = [bool]$input.rootCauseConfirmed -and [bool]$input.controlledFix -and [string]$input.minimumTests -eq 'PASS'
        Emit ([ordered]@{ decision = $(if ($ready) { 'UNLOCKED' } else { 'T0-LOCK' }); lock = $(if ($ready) { 'RELEASED' } else { 'T0-LOCK' }); product = $(if ($ready) { 'UNVERIFIED' } else { 'BLOCKED' }) })
    }
    'resume' {
        $released = [bool]$input.upstreamLockReleased
        $changed = [bool]$input.interfaceChanged
        $decision = if (-not $released -or $changed) { 'CONTINUE-LOCKED' } else { 'AUTO-RESUME' }
        Emit ([ordered]@{ decision = $decision; lock = $(if ($decision -eq 'AUTO-RESUME') { 'RELEASED' } else { 'T0-LOCK' }); check = 'LIGHTWEIGHT-DEPENDENCY-CHANGE' })
    }
}
