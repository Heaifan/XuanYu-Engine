[CmdletBinding()]
param(
    [string]$RegistryPath = '',
    [string[]]$CapabilityTags = @(),
    [string]$TaskName = 'UNSPECIFIED'
)

$ErrorActionPreference = 'Stop'
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
if ([string]::IsNullOrWhiteSpace($RegistryPath)) { $RegistryPath = Join-Path $scriptRoot '..\..\docs\governance\deferred-capabilities.json' }
$allowedStatuses = @('DEFERRED-SAFE', 'ARMED', 'BLOCKING', 'CLOSED')
$requiredFields = @('DebtId', 'Title', 'CurrentTemporarySolution', 'DeferredCapability', 'ReasonForDeferral', 'ValidEnvelope', 'ActivationTriggers', 'BlockedCapabilities', 'RequiredClosure', 'TestDebt', 'IntroducedAt', 'LastReviewedAt', 'Status', 'Evidence', 'ClosureEvidence')

function Get-Text($Value) { if ($null -eq $Value) { return '' }; return ([string]$Value).Trim() }
function Get-Array($Value) { if ($null -eq $Value) { return @() }; return @($Value) }
function Get-Tags($Value) { return @(Get-Array $Value | ForEach-Object { (Get-Text $_).ToLowerInvariant() } | Where-Object { $_ }) }
function Has-TagIntersection([string[]]$Left, [string[]]$Right) { foreach ($tag in $Left) { if ($Right -contains $tag) { return $true } }; return $false }
function Join-Values($Items) { return ((Get-Array $Items | ForEach-Object { Get-Text $_ } | Where-Object { $_ }) -join ', ') }
function Fail([string]$Message) { throw $Message }

$registry = $null
$errors = New-Object System.Collections.Generic.List[string]
try {
    if (-not (Test-Path -LiteralPath $RegistryPath -PathType Leaf)) { Fail "Registry not found: $RegistryPath" }
    $registry = Get-Content -Raw -LiteralPath $RegistryPath | ConvertFrom-Json
    if ($null -eq $registry -or $null -eq $registry.debts) { Fail 'Registry must contain a debts array' }
    $debts = @( $registry.debts )
    $ids = @($debts | ForEach-Object { Get-Text $_.DebtId })
    if (($ids | Where-Object { $_ } | Select-Object -Unique).Count -ne $ids.Count) { Fail 'DebtId must be non-empty and unique' }
    foreach ($debt in $debts) {
        foreach ($field in $requiredFields) { if ($null -eq $debt.PSObject.Properties[$field]) { $errors.Add("$($debt.DebtId): missing $field") } }
        foreach ($field in @('Title', 'CurrentTemporarySolution', 'DeferredCapability', 'ReasonForDeferral', 'ValidEnvelope', 'LastReviewedAt')) { if (-not (Get-Text $debt.$field)) { $errors.Add("$($debt.DebtId): $field is empty") } }
        foreach ($field in @('RequiredClosure', 'TestDebt', 'Evidence')) { if ((Get-Array $debt.$field).Count -eq 0) { $errors.Add("$($debt.DebtId): $field is empty") } }
        if ($allowedStatuses -notcontains (Get-Text $debt.Status)) { $errors.Add("$($debt.DebtId): invalid Status") }
        if ((Get-Array $debt.ActivationTriggers).Count -eq 0) { $errors.Add("$($debt.DebtId): ActivationTriggers is empty") }
        foreach ($trigger in (Get-Array $debt.ActivationTriggers)) {
            if (-not (Get-Text $trigger.TriggerId) -or -not (Get-Text $trigger.Description) -or (Get-Tags $trigger.CapabilityTags).Count -eq 0) { $errors.Add("$($debt.DebtId): invalid ActivationTrigger") }
        }
        if ((Get-Text $debt.Status) -eq 'BLOCKING' -and (Get-Array $debt.BlockedCapabilities).Count -eq 0) { $errors.Add("$($debt.DebtId): BLOCKING requires BlockedCapabilities") }
        if ((Get-Text $debt.Status) -eq 'CLOSED' -and (Get-Array $debt.ClosureEvidence).Count -eq 0) { $errors.Add("$($debt.DebtId): CLOSED requires ClosureEvidence") }
        foreach ($capability in (Get-Array $debt.BlockedCapabilities)) { if (-not (Get-Text $capability.Name) -or (Get-Tags $capability.CapabilityTags).Count -eq 0) { $errors.Add("$($debt.DebtId): invalid BlockedCapability") } }
        foreach ($field in @('Commit', 'Version', 'Date')) { if (-not (Get-Text $debt.IntroducedAt.$field)) { $errors.Add("$($debt.DebtId): IntroducedAt.$field is empty") } }
    }
    if ($errors.Count -gt 0) { Fail ($errors -join '; ') }
} catch {
    Write-Output 'DEFERRED CAPABILITY GATE = FAIL'
    Write-Output "DEBT ID = REGISTRY"
    Write-Output 'STATUS = INVALID'
    Write-Output 'TRIGGER = NOT EVALUATED'
    Write-Output 'VALID ENVELOPE = FAIL'
    Write-Output 'BLOCKED CAPABILITY = NOT EVALUATED'
    Write-Output 'REQUIRED CLOSURE = NOT EVALUATED'
    Write-Output "ERROR = $($_.Exception.Message)"
    exit 1
}

$overall = 'PASS'
foreach ($debt in @($registry.debts)) {
    $declaredStatus = Get-Text $debt.Status
    $triggerIds = @(Get-Array $debt.ActivationTriggers | Where-Object { Has-TagIntersection (Get-Tags $_.CapabilityTags) (Get-Tags $CapabilityTags) } | ForEach-Object { Get-Text $_.TriggerId })
    $effectiveStatus = $declaredStatus
    if ($declaredStatus -ne 'CLOSED' -and $triggerIds.Count -gt 0) { $effectiveStatus = 'BLOCKING' }
    $blocked = @(Get-Array $debt.BlockedCapabilities | Where-Object { Has-TagIntersection (Get-Tags $_.CapabilityTags) (Get-Tags $CapabilityTags) } | ForEach-Object { Get-Text $_.Name })
    $isBlocked = $effectiveStatus -eq 'BLOCKING' -and $blocked.Count -gt 0
    if ($isBlocked) { $overall = 'BLOCKED' }
    $triggerText = if ($triggerIds.Count -gt 0) { $triggerIds -join ', ' } elseif ($declaredStatus -eq 'BLOCKING') { 'DECLARED-BLOCKING' } else { 'NONE' }
    $gateStatus = if ($isBlocked) { 'BLOCKED' } else { 'PASS' }
    Write-Output "DEFERRED CAPABILITY GATE = $gateStatus"
    Write-Output "DEBT ID = $(Get-Text $debt.DebtId)"
    Write-Output "STATUS = $effectiveStatus"
    Write-Output "TRIGGER = $triggerText"
    Write-Output "VALID ENVELOPE = PASS"
    Write-Output "BLOCKED CAPABILITY = $(if ($blocked.Count -gt 0) { $blocked -join ', ' } else { 'NONE' })"
    Write-Output "REQUIRED CLOSURE = $(Join-Values $debt.RequiredClosure)"
}
if ($overall -eq 'BLOCKED') { exit 2 }
exit 0
