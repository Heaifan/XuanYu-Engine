param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path,
    [switch]$WhatIf
)

$ErrorActionPreference = 'Stop'
$planPath = Join-Path $RepositoryRoot 'docs\governance\xyt-legacy-registry-migration-plan.md'
$mapPath = Join-Path $RepositoryRoot 'docs\governance\test-capability-map.json'
$registryPath = Join-Path $RepositoryRoot 'docs\governance\test-registry.json'
$roots = @(
    @{ Module = 'XuanYu.Core.Tests'; Root = 'XuanYu.Core.Tests' },
    @{ Module = 'XuanYu.World.Tests'; Root = 'XuanYu.World.Tests' },
    @{ Module = 'XuanYu.WarCore.Tests'; Root = 'XuanYu.WarCore.Tests' },
    @{ Module = 'XYUI.Avalonia.Tests'; Root = 'xyui/avalonia/tests/XYUI.Avalonia.Tests' }
)

function Get-SourceUnits {
    $units = @()
    foreach ($root in $roots) {
        foreach ($path in @(rg --files (Join-Path $RepositoryRoot $root.Root) -g '*.cs' | Sort-Object -Unique)) {
            $text = Get-Content -Raw $path
            $testCount = ([regex]::Matches($text, '(?m)^\s*\[(Fact|Theory|Test|TestCase|SkippableFact|SkippableTheory)(?:\([^\]]*\))?\]\s*$')).Count
            if ($testCount -eq 0) { continue }
            $relative = [IO.Path]::GetRelativePath($RepositoryRoot, $path).Replace('\', '/')
            $units += [pscustomobject]@{
                Module = $root.Module
                Path = $relative
                Name = [IO.Path]::GetFileNameWithoutExtension($path)
                TestCount = $testCount
            }
        }
    }
    return @($units | Sort-Object Module, Path)
}

$capabilityMap = Get-Content -Raw $mapPath | ConvertFrom-Json
$validCapabilities = @($capabilityMap.capabilities.capabilityId)
$planRows = @()
foreach ($line in Get-Content $planPath) {
    if ($line -match '^\| (WAVE-(HIGH|MEDIUM|LOW)) \| (XYT-G-\d{4}) \| (.+?) \| UNREGISTERED \| (P[0-3]) \| (KEEP|DOWNGRADE|RENAME) \| (.+?) \| (HIGH|MEDIUM|LOW) \|') {
        $planRows += [pscustomobject]@{
            Wave = $Matches[1]
            TestId = $Matches[3]
            Name = $Matches[4]
            EvidenceLevel = $Matches[5]
            Action = $Matches[6]
            CapabilityLabel = $Matches[7]
            Risk = $Matches[8]
        }
    }
}
$units = Get-SourceUnits
if ($planRows.Count -ne 696 -or $units.Count -ne 696) { throw "Migration source count mismatch: plan=$($planRows.Count), source=$($units.Count)" }
if (@($planRows.TestId | Sort-Object -Unique).Count -ne 696) { throw 'Migration source has duplicate Test IDs' }

$records = @()
for ($index = 0; $index -lt 696; $index++) {
    $row = $planRows[$index]
    $unit = $units[$index]
    if ($row.Name -ne $unit.Name) { throw "G1 name/path mismatch at $($row.TestId): plan=$($row.Name), source=$($unit.Name)" }
    $evidenceType = switch ($row.EvidenceLevel) { 'P0' { 'STATIC' } 'P1' { 'UNIT' } 'P2' { 'INTEGRATION' } 'P3' { 'REAL_RUNTIME' } }
    $duration = switch ($row.EvidenceLevel) { 'P0' { 30 } 'P1' { 30 } 'P2' { 60 } 'P3' { 90 } }
    $owner = if ($unit.Module -eq 'XYUI.Avalonia.Tests') { 'XYUI' } else { 'XYE' }
    $waveName = $row.Wave.Replace('WAVE-', '')
    $records += [pscustomobject]@{
        testId = $row.TestId
        module = $unit.Module
        evidenceLevel = $row.EvidenceLevel
        targetCapability = @('REVIEW_REQUIRED')
        productInvariants = @("REVIEW_REQUIRED: G1 capability label '$($row.CapabilityLabel)' has no canonical CAP-* mapping in test-capability-map.json")
        prerequisites = @('.NET SDK Resolver Chain', 'G1 audit identity')
        blockingScope = 'TEST_SET'
        evidenceType = @($evidenceType)
        maxDurationSeconds = $duration
        testSetVersion = "TSET-LEGACY-$waveName-v1.0"
        testPath = $unit.Path
        status = 'BLOCKED'
        owner = $owner
        migrationWave = $row.Wave
        auditAction = $row.Action
        auditRisk = $row.Risk
        capabilityResolution = 'REVIEW_REQUIRED'
    }
}

$document = [ordered]@{
    schemaVersion = 'XYT-G-REG-v1.0'
    updatedAt = '2026-09-29'
    migrationSource = 'docs/governance/xyt-legacy-test-audit.md and docs/governance/xyt-legacy-registry-migration-plan.md'
    evidenceCanonical = 'P0-P4'
    incidentCanonical = 'T0-T3; not stored in evidenceLevel'
    migrationStatus = 'REGISTERED_REVIEW_REQUIRED'
    waveCounts = [ordered]@{ HIGH = 132; MEDIUM = 237; LOW = 327; TOTAL = 696 }
    actionCounts = [ordered]@{ KEEP = 564; DOWNGRADE = 37; RENAME = 95; TOTAL = 696 }
    evidenceCounts = [ordered]@{ P0 = 281; P1 = 284; P2 = 131; P3 = 0; P4 = 'MISSING'; TOTAL = 696 }
    supersededSeedTestIds = @('XYT-B-0001', 'XYT-B-0002', 'XYT-B-0003')
    tests = $records
}

foreach ($record in $records) {
    foreach ($capability in $record.targetCapability) {
        if ($capability -ne 'REVIEW_REQUIRED' -and $capability -notin $validCapabilities) { throw "Invalid capability key: $capability" }
    }
}

$json = $document | ConvertTo-Json -Depth 8
if (-not $WhatIf) { Set-Content -LiteralPath $registryPath -Value $json -Encoding UTF8 }
Write-Output "Registry records: $($records.Count)"
Write-Output "Capability review required: $(@($records | Where-Object { $_.capabilityResolution -eq 'REVIEW_REQUIRED' }).Count)"
Write-Output "WhatIf: $WhatIf"
