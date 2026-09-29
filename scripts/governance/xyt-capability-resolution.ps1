param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path,
    [switch]$WhatIf
)

$ErrorActionPreference = 'Stop'
$registryPath = Join-Path $RepositoryRoot 'docs\governance\test-registry.json'
$mapPath = Join-Path $RepositoryRoot 'docs\governance\test-capability-map.json'
$registry = Get-Content -Raw $registryPath | ConvertFrom-Json
$map = Get-Content -Raw $mapPath | ConvertFrom-Json

$newCapabilities = @(
    [pscustomobject]@{ capabilityId = 'CAP-WORLD-STATIC-CONTRACT'; name = 'World editor static UI/input contract'; module = 'XuanYu.World.Tests'; minimumEvidenceLevel = 'P0'; defaultBlockingScope = 'TEST_SET'; productInvariants = @('World/Editor UI and input ownership declarations preserve their declared contract') },
    [pscustomobject]@{ capabilityId = 'CAP-WORLD-INPUT-STATE'; name = 'World editor input state'; module = 'XuanYu.World.Tests'; minimumEvidenceLevel = 'P1'; defaultBlockingScope = 'TEST_SET'; productInvariants = @('World/Editor input and state transitions remain deterministic in the tested local domain') },
    [pscustomobject]@{ capabilityId = 'CAP-TERRAIN-HEADLESS-COMPOSITION'; name = 'Terrain headless composition'; module = 'XuanYu.World.Tests'; minimumEvidenceLevel = 'P2'; defaultBlockingScope = 'TEST_SET'; productInvariants = @('Terrain/render projection and resource composition remain coherent in headless integration; no GPU claim') },
    [pscustomobject]@{ capabilityId = 'CAP-RENDER-SOURCE-CONTRACT'; name = 'Render source contract'; module = 'XuanYu.Render'; minimumEvidenceLevel = 'P0'; defaultBlockingScope = 'TEST_SET'; productInvariants = @('Renderer source and draw-plan contracts remain structurally consistent within the declared static scope') },
    [pscustomobject]@{ capabilityId = 'CAP-RENDER-HEADLESS-COMPOSITION'; name = 'Render headless composition'; module = 'XuanYu.Render'; minimumEvidenceLevel = 'P2'; defaultBlockingScope = 'TEST_SET'; productInvariants = @('Render projection and draw-plan composition remain coherent in headless composition; no GPU claim') },
    [pscustomobject]@{ capabilityId = 'CAP-RENDER-STATE-CONTRACT'; name = 'Render state contract'; module = 'XuanYu.Render'; minimumEvidenceLevel = 'P1'; defaultBlockingScope = 'TEST_SET'; productInvariants = @('Render state and draw-plan calculations remain deterministic in the tested domain') },
    [pscustomobject]@{ capabilityId = 'CAP-XYUI-STATIC-CONTRACT'; name = 'XYUI static contract'; module = 'XYUI.Avalonia.Tests'; minimumEvidenceLevel = 'P0'; defaultBlockingScope = 'MODULE'; productInvariants = @('XYUI static tokens and control contracts remain aligned with canonical Runtime and Consumer mappings') },
    [pscustomobject]@{ capabilityId = 'CAP-XYUI-UNIT-STATE'; name = 'XYUI unit state'; module = 'XYUI.Avalonia.Tests'; minimumEvidenceLevel = 'P1'; defaultBlockingScope = 'MODULE'; productInvariants = @('XYUI pure state, geometry, and token transitions remain deterministic in the tested domain') },
    [pscustomobject]@{ capabilityId = 'CAP-CORE-STATE-CONTRACT'; name = 'Core state contract'; module = 'XuanYu.Core.Tests'; minimumEvidenceLevel = 'P1'; defaultBlockingScope = 'MODULE'; productInvariants = @('Core state and transform contracts remain deterministic for tested inputs') }
)

$existing = @($map.capabilities)
foreach ($capability in $newCapabilities) {
    if (@($existing | Where-Object capabilityId -eq $capability.capabilityId).Count -eq 0) { $existing += $capability }
}
$map.capabilities = $existing
$lookup = @{}
foreach ($capability in $map.capabilities) { $lookup[$capability.capabilityId] = $capability }

function Resolve-Key($record) {
    $label = [regex]::Match($record.productInvariants[0], "label '([^']+)'").Groups[1].Value
    if ($record.module -eq 'XuanYu.World.Tests' -and $record.evidenceLevel -eq 'P2' -and $label -eq 'UI/input/integration') { return 'CAP-WORLD-HEADLESS' }
    if ($record.module -eq 'XuanYu.World.Tests' -and $record.evidenceLevel -eq 'P2' -and $label -eq 'Render/Terrain contract') { return 'CAP-TERRAIN-HEADLESS-COMPOSITION' }
    if ($record.module -eq 'XYUI.Avalonia.Tests' -and $record.evidenceLevel -eq 'P0') { return 'CAP-XYUI-STATIC-CONTRACT' }
    if ($record.module -eq 'XYUI.Avalonia.Tests' -and $record.evidenceLevel -eq 'P1') { return 'CAP-XYUI-UNIT-STATE' }
    if ($record.module -eq 'XuanYu.World.Tests' -and $record.evidenceLevel -eq 'P0' -and $label -eq 'UI/input/integration') { return 'CAP-WORLD-STATIC-CONTRACT' }
    if ($record.module -eq 'XuanYu.World.Tests' -and $record.evidenceLevel -eq 'P1' -and $label -eq 'UI/input/integration') { return 'CAP-WORLD-INPUT-STATE' }
    if ($record.evidenceLevel -eq 'P0' -and $label -eq 'Render/Terrain contract') { return 'CAP-RENDER-SOURCE-CONTRACT' }
    if ($record.evidenceLevel -eq 'P2' -and $label -eq 'Render/Terrain contract') { return 'CAP-RENDER-HEADLESS-COMPOSITION' }
    if ($record.evidenceLevel -eq 'P1' -and $label -eq 'Render/Terrain contract') { return 'CAP-RENDER-STATE-CONTRACT' }
    if ($record.module -eq 'XuanYu.Core.Tests' -and $record.evidenceLevel -eq 'P1' -and $label -eq 'UI/input/integration') { return 'CAP-CORE-STATE-CONTRACT' }
    return $null
}

$high = @($registry.tests | Where-Object auditRisk -eq 'HIGH')
$resolved = 0
foreach ($record in $high) {
    if ($record.capabilityResolution -ne 'REVIEW_REQUIRED') { throw "HIGH record is not awaiting resolution: $($record.testId)" }
    $key = Resolve-Key $record
    if ($null -eq $key) { continue }
    if (-not $lookup.ContainsKey($key)) { throw "Resolution key missing from map: $key" }
    $record.targetCapability = @($key)
    $record.productInvariants = @($lookup[$key].productInvariants)
    $record.capabilityResolution = 'RESOLVED'
    $record.status = 'ACTIVE'
    $resolved++
}
if ($high.Count -ne 132 -or $resolved -ne 129) { throw "Resolution count mismatch: HIGH=$($high.Count), RESOLVED=$resolved" }

if (-not $WhatIf) {
    $map | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $mapPath -Encoding UTF8
    $registry | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $registryPath -Encoding UTF8
}
Write-Output "HIGH=$($high.Count) RESOLVED=$resolved REVIEW_REQUIRED=$($high.Count - $resolved) WhatIf=$WhatIf"
