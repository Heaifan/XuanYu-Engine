$ErrorActionPreference = 'Stop'
$entry = Join-Path $PSScriptRoot 'dependency-handoff.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('handoff-h3-' + [guid]::NewGuid().ToString('N'))
$passed = 0
$total = 16
function Invoke-H3([string[]]$Arguments) {
    $ErrorActionPreference = 'Continue'
    $out = @(& pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry @Arguments 2>&1)
    [pscustomobject]@{ Code = $LASTEXITCODE; Text = ($out -join "`n") }
}
function Assert([bool]$Condition, [string]$Message) {
    if (!$Condition) { throw $Message }
    $script:passed++
}
try {
    New-Item -ItemType Directory -Force -Path $root | Out-Null
    $shared = Join-Path $root 'Shared.cs'
    $bar = Join-Path $root 'Bar.cs'
    $foo = Join-Path $root 'Foo.cs'
    Set-Content -LiteralPath $shared -Value 'v1'
    Set-Content -LiteralPath $bar -Value 'bar'
    Set-Content -LiteralPath $foo -Value 'foo'
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'state', '-Lane', 'A', '-ImplementationStatus', 'IMPLEMENTATION_COMPLETE', '-OwnScopePass', '-HandoffReady') | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'state', '-Lane', 'B', '-ImplementationStatus', 'IMPLEMENTATION_COMPLETE', '-OwnScopePass', '-HandoffReady') | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'state', '-Lane', 'C', '-ImplementationStatus', 'IMPLEMENTATION_COMPLETE', '-OwnScopePass', '-HandoffReady') | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'edge', '-FromLane', 'A', '-ToLane', 'B', '-File', $shared) | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'edge', '-FromLane', 'C', '-ToLane', 'B', '-File', $bar) | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'evidence', '-EvidenceId', 'A-E1', '-Lane', 'A', '-File', $shared) | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'evidence', '-EvidenceId', 'A-E2', '-Lane', 'A', '-File', $foo) | Out-Null
    $r = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'release', '-Lane', 'A', '-ToLane', 'B', '-File', $shared, '-Reason', 'shared implementation')
    Assert ($r.Code -eq 0) 'A to B release failed'
    $a = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'acquire', '-Lane', 'B', '-File', $shared)
    Assert ($a.Code -eq 0) 'B acquire failed'
    Set-Content -LiteralPath $shared -Value 'v2'
    $m = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'modify', '-Lane', 'B', '-File', $shared)
    Assert ($m.Code -eq 0 -and $m.Text -match 'STALE_BY_DEPENDENCY_CHANGE') 'evidence was not stale'
    $state = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'inspect')
    Assert ($state.Code -eq 0 -and $state.Text -match 'IMPLEMENTATION_COMPLETE') 'implementation status regressed'
    $sharedState = $state.Text | ConvertFrom-Json
    $transfer = @($sharedState.transfers | Where-Object file -eq 'Shared.cs') | Select-Object -First 1
    $laneA = @($sharedState.lanes | Where-Object lane -eq 'A') | Select-Object -First 1
    Assert ($transfer.fromLane -eq 'A' -and $transfer.toLane -eq 'B' -and $transfer.transferPoint -eq 'RELEASE' -and $transfer.oldFingerprint) 'transfer audit record incomplete'
    Assert ($laneA.regressionRequired -eq $true) 'regression required was not raised'
    $old = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'modify', '-Lane', 'A', '-File', $shared)
    Assert ($old.Code -ne 0 -and $old.Text -match 'WRITE_DENIED') 'old owner was allowed to modify'
    $unknown = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'release', '-Lane', 'C', '-ToLane', 'B', '-File', $bar, '-UnknownDirty', 'mystery.bin')
    Assert ($unknown.Code -ne 0 -and $unknown.Text -match 'UNAUTHORIZED_DIRTY_BLOCKED') 'unauthorized dirty did not block transfer'
    $before = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'acquire', '-Lane', 'B', '-File', $bar)
    Assert ($before.Code -ne 0 -and $before.Text -match 'ACQUIRE_BEFORE_RELEASE') 'acquire before release was allowed'
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'release', '-Lane', 'C', '-ToLane', 'B', '-File', $bar) | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'acquire', '-Lane', 'B', '-File', $bar) | Out-Null
    Set-Content -LiteralPath $bar -Value 'bar2'
    $unrelated = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'modify', '-Lane', 'B', '-File', $bar)
    $jsonText = $unrelated.Text.Substring(0, $unrelated.Text.LastIndexOf("`nDEPENDENCY_CHANGE"))
    $afterBar = $jsonText | ConvertFrom-Json
    $fooEvidence = @($afterBar.evidence | Where-Object id -eq 'A-E2') | Select-Object -First 1
    Assert ($unrelated.Code -eq 0 -and $fooEvidence.status -eq 'FRESH') 'unrelated file polluted A evidence'
    $pre = Join-Path $root 'PreAcquire.cs'; Set-Content -LiteralPath $pre -Value 'pre1'
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'edge', '-FromLane', 'C', '-ToLane', 'B', '-File', $pre) | Out-Null
    Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'release', '-Lane', 'C', '-ToLane', 'B', '-File', $pre) | Out-Null
    Set-Content -LiteralPath $pre -Value 'pre2'
    $changed = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'acquire', '-Lane', 'B', '-File', $pre)
    Assert ($changed.Code -ne 0 -and $changed.Text -match 'PRE_ACQUIRE_MODIFICATION') 'pre-acquire modification was allowed'
    Assert ((Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'complete', '-Lane', 'B')).Code -eq 0) 'B completion failed'
    Assert ((Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'regression', '-Lane', 'A')).Code -eq 0) 'A regression failed'
    Assert ((Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'integration')).Code -eq 0) 'integration gate failed after regression'
    $cycle = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'deadlock', '-Wait', 'A>B,B>A')
    Assert ($cycle.Code -ne 0 -and $cycle.Text -match 'CIRCULAR_HANDOFF_DEPENDENCY') 'two-node cycle was not detected'
    $three = Invoke-H3 @('-RepositoryRoot', $root, '-Mode', 'deadlock', '-Wait', 'A>B,B>C,C>A')
    Assert ($three.Code -ne 0 -and $three.Text -match 'CIRCULAR_HANDOFF_DEPENDENCY' -and $three.Text -match 'A\s+->\s+B\s+->\s+C\s+->\s*A') ("three-node cycle was not detected: {0}" -f $three.Text)
    'H3 SELFTEST PASS {0}/{1}' -f $passed, $total
} finally {
    if (Test-Path -LiteralPath $root) { Remove-Item -LiteralPath $root -Recurse -Force }
}
