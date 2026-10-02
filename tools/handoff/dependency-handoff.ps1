[CmdletBinding()] param(
    [ValidateSet('state', 'edge', 'evidence', 'release', 'acquire', 'modify', 'complete', 'regression', 'integration', 'deadlock', 'inspect')][string]$Mode = 'inspect',
    [string]$RepositoryRoot = (Get-Location).Path, [string]$Lane, [string]$ToLane, [string]$FromLane,
    [string]$File, [string]$EvidenceId, [string]$ImplementationStatus, [switch]$OwnScopePass,
    [switch]$HandoffReady, [string]$Reason = '', [string]$Wait = '', [string[]]$UnknownDirty = @()
)
$d = $PSScriptRoot
. (Join-Path $d 'dependency-handoff.common.ps1')
$root = [IO.Path]::GetFullPath($RepositoryRoot)
function Lane($s, [string]$Name) { Find-One $s.lanes 'lane' $Name }
function FileRecord($s, [string]$Name) { Find-One $s.files 'file' $Name }
function Save-($s) { Write-State $root $s; $s }
try {
    $s = Read-State $root
    if ($Mode -eq 'state') {
        $x = Lane $s $Lane
        if (!$x) { $x = [pscustomobject]@{ lane = $Lane; implementation = ''; ownScopePass = $false; handoffReady = $false; regressionRequired = $false }; $s.lanes = Add-Item $s.lanes $x }
        if ($ImplementationStatus) { $x.implementation = $ImplementationStatus }
        if ($OwnScopePass) { $x.ownScopePass = $true }
        if ($HandoffReady) { $x.handoffReady = $true }
        Save- $s | ConvertTo-Json -Depth 12; exit 0
    }
    if ($Mode -eq 'edge') {
        $f = Relative-File $root $File
        $s.edges = Add-Item $s.edges ([pscustomobject]@{ fromLane = $FromLane; toLane = $ToLane; file = $f })
        Save- $s | ConvertTo-Json -Depth 12; exit 0
    }
    if ($Mode -eq 'evidence') {
        $f = Relative-File $root $File
        $s.evidence = Add-Item $s.evidence ([pscustomobject]@{ id = $EvidenceId; lane = $Lane; files = @($f); status = 'FRESH'; regressionRequired = $false })
        Save- $s | ConvertTo-Json -Depth 12; exit 0
    }
    if ($Mode -eq 'release') {
        if ($UnknownDirty.Count) { Fail UNKNOWN_DIRTY_BLOCKED 'Transfer requires a known dirty set.' }
        $f = Relative-File $root $File; $l = Lane $s $Lane
        if (!$l -or $l.implementation -ne 'IMPLEMENTATION_COMPLETE' -or !$l.ownScopePass -or !$l.handoffReady) { Fail RELEASE_NOT_READY 'Owner must be implementation-complete, own-scope-pass, and handoff-ready.' }
        $old = FileRecord $s $f
        if ($old -and $old.status -eq 'ACTIVE' -and $old.owner -ne $Lane) { Fail OWNERSHIP_CONFLICT $f }
        $record = [pscustomobject]@{ file = $f; owner = $Lane; status = 'RELEASED'; availableTo = $ToLane; oldRevision = File-Fingerprint $root $f; oldFingerprint = File-Fingerprint $root $f }
        $s.files = @($s.files | Where-Object file -ne $f) + $record
        $s.transfers = Add-Item $s.transfers ([pscustomobject]@{ fromLane = $Lane; toLane = $ToLane; file = $f; oldRevision = $record.oldRevision; oldFingerprint = $record.oldFingerprint; transferPoint = 'RELEASE'; reason = $Reason; regressionImpact = 'DEPENDENCY_CONSUMERS_STALE_ON_MODIFY' })
        Save- $s | ConvertTo-Json -Depth 12; exit 0
    }
    if ($Mode -eq 'acquire') {
        $f = Relative-File $root $File; $old = FileRecord $s $f
        if (!$old -or $old.status -ne 'RELEASED') { Fail ACQUIRE_BEFORE_RELEASE $f }
        if ($old.availableTo -ne $Lane) { Fail OWNERSHIP_CONFLICT $f }
        if ((File-Fingerprint $root $f) -ne $old.oldFingerprint) { Fail PRE_ACQUIRE_MODIFICATION $f }
        $old.owner = $Lane; $old.status = 'ACTIVE'; $old.availableTo = ''
        Save- $s | ConvertTo-Json -Depth 12; exit 0
    }
    if ($Mode -eq 'modify') {
        $f = Relative-File $root $File; $old = FileRecord $s $f
        if (!$old -or $old.status -ne 'ACTIVE' -or $old.owner -ne $Lane) { Fail WRITE_DENIED "Lane $Lane does not own $f." }
        $new = File-Fingerprint $root $f
        foreach ($e in @($s.evidence)) {
            if (@($e.files) -contains $f -and $e.lane -ne $Lane) { $e.status = 'STALE_BY_DEPENDENCY_CHANGE'; $e.regressionRequired = $true; $l = Lane $s $e.lane; if ($l) { $l.regressionRequired = $true } }
        }
        $old.oldRevision = $new; $old.oldFingerprint = $new
        Save- $s | ConvertTo-Json -Depth 12; 'DEPENDENCY_CHANGE APPLIED; STALE_BY_DEPENDENCY_CHANGE'; exit 0
    }
    if ($Mode -eq 'complete') { $l = Lane $s $Lane; if (!$l) { Fail LANE_MISSING $Lane }; $l.implementation = 'COMPLETE'; Save- $s | ConvertTo-Json -Depth 12; exit 0 }
    if ($Mode -eq 'regression') { foreach ($e in @($s.evidence | Where-Object lane -eq $Lane)) { $e.status = 'FRESH'; $e.regressionRequired = $false }; $l = Lane $s $Lane; if ($l) { $l.regressionRequired = $false }; Save- $s | ConvertTo-Json -Depth 12; 'REGRESSION PASS'; exit 0 }
    if ($Mode -eq 'integration') { if (@($s.evidence | Where-Object { $_.status -ne 'FRESH' -or $_.regressionRequired }).Count) { Fail INTEGRATION_GATE_BLOCKED 'Stale evidence remains.' }; 'INTEGRATION GATE PASS'; exit 0 }
    if ($Mode -eq 'deadlock') { . (Join-Path $d 'dependency-handoff.deadlock.ps1'); Test-Deadlock $s $Wait; exit 0 }
    $s | ConvertTo-Json -Depth 16; exit 0
} catch { Write-Error $_.Exception.Message; exit 1 }
