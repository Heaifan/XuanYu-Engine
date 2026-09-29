$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'xyt-evidence.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('xyt-evidence-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $root | Out-Null
$store = Join-Path $root 'evidence.json'
$changes = Join-Path $root 'changes.json'
$failures = [System.Collections.Generic.List[string]]::new()

function Assert-Equal([string]$name, $actual, $expected) {
    if ($actual -ne $expected) { [void]$failures.Add("$name expected [$expected] got [$actual]") }
}
function Invoke-Evidence([string[]]$argsList) {
    $named = @{}
    for ($i = 0; $i -lt $argsList.Count; $i += 2) { $named[$argsList[$i].TrimStart('-')] = $argsList[$i + 1] }
    $raw = @(& $scriptPath @named)
    if (-not $?) { throw ($raw -join "`n") }
    $raw -join "`n" | ConvertFrom-Json
}
function Add-Evidence([string]$capability, [string]$testId, [string]$setVersion, [string]$level, [string]$status = 'VALID') {
    [void](Invoke-Evidence @('-Operation','Register','-EvidencePath',$store,'-Commit','abc123','-Version','v1','-Capability',$capability,'-TestId',$testId,'-TestSetVersion',$setVersion,'-EvidenceLevel',$level,'-Status',$status,'-CreatedAt','2026-09-29T00:00:00Z'))
}

Add-Evidence 'Render.P0' 'T-RENDER' 'R1' 'T3'
Add-Evidence 'Terrain.P3' 'T-TERRAIN' 'R1' 'T2'
Add-Evidence 'Input.P0' 'T-INPUT' 'R1' 'T3'
Add-Evidence 'Unrelated.P3' 'T-UNRELATED' 'R1' 'T1'
Add-Evidence 'Legacy.P3' 'T-LEGACY' 'R1' 'T1' 'SUPERSEDED'

$renderChange = @(@{ Trigger = 'Render Pipeline Changed'; Capability = 'Render.P0' })
$renderChange | ConvertTo-Json | Set-Content -LiteralPath $changes -Encoding UTF8
$result = Invoke-Evidence @('-Operation','Invalidate','-EvidencePath',$store,'-ChangePath',$changes)
Assert-Equal 'render change status' (($result.Records | Where-Object Capability -eq 'Render.P0').Status) 'REVALIDATION_REQUIRED'
Assert-Equal 'unrelated P3 status' (($result.Records | Where-Object Capability -eq 'Terrain.P3').Status) 'VALID'

$docChange = @(@{ Trigger = 'Documentation Changed'; Capability = 'Render.P0' })
$docChange | ConvertTo-Json | Set-Content -LiteralPath $changes -Encoding UTF8
$docResult = Invoke-Evidence @('-Operation','Invalidate','-EvidencePath',$store,'-ChangePath',$changes)
Assert-Equal 'unsupported document trigger' (($docResult.Records | Where-Object Capability -eq 'Input.P0').Status) 'VALID'

$majorChange = @(@{ Trigger = 'Test Set Major Version Changed'; Capability = 'Input.P0'; TestSetVersion = 'R2' })
$majorChange | ConvertTo-Json | Set-Content -LiteralPath $changes -Encoding UTF8
$majorResult = Invoke-Evidence @('-Operation','Invalidate','-EvidencePath',$store,'-ChangePath',$changes)
Assert-Equal 'test set major change' (($majorResult.Records | Where-Object Capability -eq 'Input.P0').Status) 'REVALIDATION_REQUIRED'

$oracleChange = @(@{ Trigger = 'Test Oracle Changed'; Capability = 'Terrain.P3' })
$oracleChange | ConvertTo-Json | Set-Content -LiteralPath $changes -Encoding UTF8
$oracleResult = Invoke-Evidence @('-Operation','Invalidate','-EvidencePath',$store,'-ChangePath',$changes)
Assert-Equal 'oracle change' (($oracleResult.Records | Where-Object Capability -eq 'Terrain.P3').Status) 'REVALIDATION_REQUIRED'

$plan = Invoke-Evidence @('-Operation','Plan','-EvidencePath',$store)
Assert-Equal 'plan includes invalidated input' (@($plan.Items | Where-Object TestId -eq 'T-INPUT').Count) 1
Assert-Equal 'plan includes invalidated render' (@($plan.Items | Where-Object TestId -eq 'T-RENDER').Count) 1
Assert-Equal 'plan excludes valid history' (@($plan.Items | Where-Object TestId -eq 'T-UNRELATED').Count) 0

$closure = Invoke-Evidence @('-Operation','Closure','-EvidencePath',$store,'-CriticalCapability','Render.P0','-Phase','Development')
Assert-Equal 'development remains open' $closure.Decision 'ALLOW_DEVELOPMENT'
$closure = Invoke-Evidence @('-Operation','Closure','-EvidencePath',$store,'-CriticalCapability','Render.P0','-Phase','Closure')
Assert-Equal 'closure blocks stale critical evidence' $closure.Decision 'BLOCKED'

if ($failures.Count -gt 0) { $failures | ForEach-Object { "FAIL: $_" }; exit 1 }
'XYT EVIDENCE SELFTEST: PASS'
'Cases: 8'
