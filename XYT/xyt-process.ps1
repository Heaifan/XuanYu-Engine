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
