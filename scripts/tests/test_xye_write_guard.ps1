$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$guard = Join-Path $root 'scripts\xye-write-guard.ps1'
$claimPath = 'scripts/tests/test_xye_write_guard.ps1'
$otherPath = 'scripts/xye-build.ps1'
$claimHash = (Get-FileHash (Join-Path $root $claimPath) -Algorithm SHA256).Hash
$otherHash = (Get-FileHash (Join-Path $root $otherPath) -Algorithm SHA256).Hash
$a = 'guard-test-a-' + [guid]::NewGuid().ToString('N')
$b = 'guard-test-b-' + [guid]::NewGuid().ToString('N')
$c = 'guard-test-release-a-' + [guid]::NewGuid().ToString('N')
$d = 'guard-test-release-b-' + [guid]::NewGuid().ToString('N')
$claimDir = (& git -C $root rev-parse --absolute-git-dir).Trim() + '\xye-write-claims'
$hadClaimDir = Test-Path -LiteralPath $claimDir
try {
    & $guard -Action Claim -Owner $a -Paths $claimPath -ExpectedHashes @{ $claimPath=$claimHash } | Out-Null
    $conflictRejected = $false
    try { & $guard -Action Claim -Owner $b -Paths $claimPath | Out-Null }
    catch { $conflictRejected = $_.Exception.Message -match 'Write conflict' }
    if (-not $conflictRejected) { throw 'Concurrent same-file claim was not rejected.' }

    & $guard -Action Claim -Owner $b -Paths $otherPath -ExpectedHashes @{ $otherPath=$otherHash } | Out-Null
    $staleRejected = $false
    try { & $guard -Action Check -Owner $b -Paths $otherPath -ExpectedHashes @{ $otherPath='STALE' } | Out-Null }
    catch { $staleRejected = $_.Exception.Message -match 'Stale baseline' }
    if (-not $staleRejected) { throw 'Stale content baseline was not rejected.' }
    & $guard -Action Release -Owner $a -Paths $claimPath | Out-Null
    & $guard -Action Claim -Owner $c -Paths $claimPath -ExpectedHashes @{ $claimPath=$claimHash } | Out-Null
    & $guard -Action Release -Owner $c -Paths $claimPath | Out-Null
    & $guard -Action Claim -Owner $d -Paths $claimPath -ExpectedHashes @{ $claimPath=$claimHash } | Out-Null
    & $guard -Action Release -Owner $d -Paths $claimPath | Out-Null
    Write-Output 'PASS: same-file conflict, distinct-path claim, stale-baseline rejection, and release/re-registration were verified.'
} finally {
    & $guard -Action Release -Owner $a -Paths $claimPath | Out-Null
    & $guard -Action Release -Owner $b -Paths $otherPath | Out-Null
    if (-not $hadClaimDir -and (Test-Path -LiteralPath $claimDir) -and
        @(Get-ChildItem -LiteralPath $claimDir -Force).Count -eq 0) { Remove-Item -LiteralPath $claimDir }
}
