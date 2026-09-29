function Invoke-XytChild([string]$Label, [string]$Path, [string[]]$Arguments = @()) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "$Label module is missing: $Path" }
    Write-Output "[$Label] START"
    & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $Path @Arguments
    $code = $LASTEXITCODE
    Write-Output "[$Label] EXIT=$code"
    if ($code -ne 0) { throw "$Label failed with exit code $code." }
}

function Invoke-XytSelftest([string]$Label, [string]$RelativePath) {
    Invoke-XytChild $Label (Join-Path $RepoRoot $RelativePath)
}

function Invoke-XytVersionGate([hashtable]$Arguments) {
    $path = Join-Path $RepoRoot 'scripts\governance\version-event-gate.ps1'
    $forward = @('-RepoRoot', $RepoRoot, '-ChangeType', $Arguments.ChangeType, '-EventId', $Arguments.EventId,
        '-CurrentVersion', $Arguments.CurrentVersion, '-CandidateId', $Arguments.CandidateId,
        '-CandidateFingerprint', $Arguments.CandidateFingerprint)
    if ($Arguments.FormalAcceptance) { $forward += '-FormalAcceptance' }
    if ($Arguments.RequireClean) { $forward += '-RequireClean' }
    Invoke-XytChild 'VersionGate' $path $forward
}
