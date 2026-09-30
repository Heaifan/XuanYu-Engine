[CmdletBinding()]
param(
    [ValidateSet('Validate', 'Summary', 'Mapping', 'MergeGate', 'Invalidate')]
    [string]$Operation = 'Summary',
    [string]$RegistryPath = 'docs/governance/xyt-test-truth-registry.json',
    [string]$SchemaPath = 'docs/governance/xyt-test-truth-schema.json',
    [string]$ChangedTestId = '',
    [string]$ChangedField = ''
)

$ErrorActionPreference = 'Stop'
$registry = Get-Content -Raw $RegistryPath | ConvertFrom-Json
$schema = Get-Content -Raw $SchemaPath | ConvertFrom-Json
$decisionValues = @('KEEP','RENAME','STRENGTHEN','REWRITE','SPLIT','RETIRE','ESCALATE')
$truthValues = @('UNREVIEWED','VERIFIED','WEAK_ORACLE','WRONG_ORACLE','MISLEADING_CLAIM','TIER_OVERCLAIM','RED_INSENSITIVE','DUPLICATE','IMPLEMENTATION_COUPLED','ESCALATED')
$tierValues = @('T0','T1','T2','T3','T4')
$records = @($registry.records)

function Fail-Truth([string]$Message) { throw "TEST TRUTH INVALID: $Message" }
function Count-Value([object[]]$Items, [string]$Property, [string]$Value) { @($Items | Where-Object { [string]$_.$Property -eq $Value }).Count }

function Validate-Record($Record) {
    foreach ($name in @('TestId','Project','File','Capability','TestName','Claim','ActualProductionPath','Oracle','FixtureFakeMock','EvidenceTier','Proves','DoesNotProve','ExecutionStatus','TruthStatus','Decision','RedSensitivity','RegressionWitness','WitnessStatus','Notes')) {
        if ($null -eq $Record.PSObject.Properties[$name]) { Fail-Truth "missing $name in $($Record.TestId)" }
    }
    if ($Record.EvidenceTier -notin $tierValues) { Fail-Truth "invalid EvidenceTier in $($Record.TestId)" }
    if ($Record.TruthStatus -notin $truthValues) { Fail-Truth "invalid TruthStatus in $($Record.TestId)" }
    if ($Record.Decision -notin $decisionValues) { Fail-Truth "invalid Decision in $($Record.TestId)" }
    if ($Record.WitnessStatus -notin @('NOT_RECORDED','RED_CONFIRMED','GREEN_CONFIRMED','INCOMPLETE','INVALID','NOT_APPLICABLE')) { Fail-Truth "invalid WitnessStatus in $($Record.TestId)" }
    if (@($records | Where-Object TestId -eq $Record.TestId).Count -gt 1) { Fail-Truth "duplicate TestId $($Record.TestId)" }
}

function Get-Summary {
    $reviewed = @($records | Where-Object { $_.TruthStatus -ne 'UNREVIEWED' }).Count
    [pscustomobject]@{
        TOTAL = $records.Count; REVIEWED = $reviewed; UNREVIEWED = $records.Count - $reviewed
        KEEP = Count-Value $records Decision KEEP; RENAME = Count-Value $records Decision RENAME
        STRENGTHEN = Count-Value $records Decision STRENGTHEN; REWRITE = Count-Value $records Decision REWRITE
        SPLIT = Count-Value $records Decision SPLIT; RETIRE = Count-Value $records Decision RETIRE
        ESCALATE = Count-Value $records Decision ESCALATE; WRONG_ORACLE = Count-Value $records TruthStatus WRONG_ORACLE
        MISLEADING_CLAIM = Count-Value $records TruthStatus MISLEADING_CLAIM; WEAK_ASSERTION = Count-Value $records TruthStatus WEAK_ORACLE
        DUPLICATE = Count-Value $records TruthStatus DUPLICATE; TIER_OVERCLAIM = Count-Value $records TruthStatus TIER_OVERCLAIM
        RED_INSENSITIVE = Count-Value $records TruthStatus RED_INSENSITIVE
    }
}

switch ($Operation) {
    'Validate' {
        foreach ($record in $records) { Validate-Record $record }
        Write-Output "SCHEMA: PASS"
        Write-Output "REGISTRY RECORDS: $($records.Count)"
    }
    'Summary' {
        foreach ($record in $records) { Validate-Record $record }
        Get-Summary | ConvertTo-Json -Compress
    }
    'Mapping' {
        foreach ($capability in @($records | Group-Object Capability)) {
            [pscustomobject]@{ Capability = $capability.Name; TestIds = @($capability.Group.TestId); Count = $capability.Count } | ConvertTo-Json -Compress
        }
        if ($records.Count -eq 0) { Write-Output 'CAPABILITY MAPPING: EMPTY / NO TRUTH-REVIEWED TESTS' }
    }
    'MergeGate' {
        foreach ($record in $records) { Validate-Record $record }
        $summary = Get-Summary
        $blocked = @()
        if ($records.Count -eq 0) { $blocked += 'NO TRUTH RECORDS / INVENTORY NOT IMPORTED' }
        if ($summary.UNREVIEWED -ne 0) { $blocked += 'UNREVIEWED TESTS' }
        foreach ($field in @('WRONG_ORACLE','MISLEADING_CLAIM','WEAK_ASSERTION','TIER_OVERCLAIM','RED_INSENSITIVE')) { if ($summary.$field -ne 0) { $blocked += $field } }
        if ((Count-Value $records TruthStatus ESCALATED) -ne 0) { $blocked += 'ESCALATED_TEST_TRUTH' }
        if (@($registry.crossLaneFiles).Count -ne 0) { $blocked += 'CROSS_LANE_FILE' }
        if ($blocked.Count -eq 0) { Write-Output 'MERGE ELIGIBILITY: YES' } else { Write-Output 'MERGE ELIGIBILITY: NO'; Write-Output ('BLOCKERS: ' + ($blocked -join ', ')) }
    }
    'Invalidate' {
        if ([string]::IsNullOrWhiteSpace($ChangedTestId) -or [string]::IsNullOrWhiteSpace($ChangedField)) { Fail-Truth 'Invalidate requires ChangedTestId and ChangedField' }
        $match = @($registry.evidenceRecords | Where-Object TestId -eq $ChangedTestId)
        if ($match.Count -eq 0) { Write-Output "NO MATCH: $ChangedTestId"; exit 0 }
        foreach ($evidence in $match) { if ($evidence.Status -eq 'VALID') { $evidence.Status = 'REVALIDATION_REQUIRED'; $evidence.InvalidationTrigger = 'TEST_CHANGED'; Write-Output "REVALIDATION_REQUIRED: $($evidence.TestId) / $ChangedField" } }
        $registry | ConvertTo-Json -Depth 8 | Set-Content -Encoding utf8 $RegistryPath
    }
}
