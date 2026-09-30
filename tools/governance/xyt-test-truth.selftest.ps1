$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$tool = Join-Path $PSScriptRoot 'xyt-test-truth.ps1'
$registry = Join-Path $root 'docs/governance/xyt-test-truth-registry.json'
$schema = Join-Path $root 'docs/governance/xyt-test-truth-schema.json'
$validate = (& pwsh -NoLogo -NoProfile -File $tool -Operation Validate -RegistryPath $registry -SchemaPath $schema) -join "`n"
if ($LASTEXITCODE -ne 0 -or $validate -notmatch 'SCHEMA: PASS') { throw 'truth registry validation failed' }
$summary = (& pwsh -NoLogo -NoProfile -File $tool -Operation Summary -RegistryPath $registry -SchemaPath $schema | ConvertFrom-Json)
if ($summary.TOTAL -ne 8 -or $summary.REVIEWED -ne 8 -or $summary.UNREVIEWED -ne 0) { throw 'Wave T-A candidate record count/review status is invalid' }
if ($summary.KEEP -ne 8 -or $summary.ESCALATE -ne 0) { throw 'Wave T-A decision summary is invalid' }
if ($summary.WRONG_ORACLE -ne 0 -or $summary.MISLEADING_CLAIM -ne 0 -or
    $summary.TIER_OVERCLAIM -ne 0 -or $summary.RED_INSENSITIVE -ne 0) { throw 'Wave T-A truth liabilities are non-zero' }
$records = Get-Content -Raw $registry | ConvertFrom-Json
if (@($records.capabilityMapping).Count -ne 5) { throw 'capability coverage mapping must contain five requested capabilities' }
if (@($records.records | Where-Object TestId -eq 'camera.frame-all.observation-center').Notes -notmatch 'Cursor Anchor is NOT A CURRENT CONTRACT') { throw 'camera cursor-anchor boundary is missing' }
if (@($records.records | Where-Object TestId -eq 'terrain.context.hidden-region-host-slot').TruthStatus -ne 'VERIFIED') { throw 'Terrain Context must be VERIFIED after same-test RED/GREEN witness' }
if (@($records.records | Where-Object TestId -eq 'terrain.context.hidden-region-host-slot').WitnessStatus -ne 'GREEN_CONFIRMED') { throw 'Terrain Context witness must be GREEN_CONFIRMED' }
$gate = (& pwsh -NoLogo -NoProfile -File $tool -Operation MergeGate -RegistryPath $registry -SchemaPath $schema) -join "`n"
if ($gate -notmatch 'MERGE ELIGIBILITY: YES') { throw 'resolved Wave T-A truth records must pass Truth MergeGate' }
Write-Output 'XYT TEST TRUTH SELFTEST: PASS'
