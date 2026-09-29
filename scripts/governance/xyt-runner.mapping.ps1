function Resolve-XytPath($Repo, $Path, $Default) {
    if ([string]::IsNullOrWhiteSpace($Path)) { return Join-Path $Repo $Default }
    if ([IO.Path]::IsPathRooted($Path)) { return $Path }
    return Join-Path $Repo $Path
}
function Read-XytJson($Path) {
    if ([string]::IsNullOrWhiteSpace($Path)) { return $null }
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return $null }
    return Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json
}
function Get-ChangedFiles($Repo, $ListPath, $DiffRange) {
    if ($ListPath) { return @(Get-Content -LiteralPath $ListPath | Where-Object { $_.Trim() }) }
    if ($DiffRange) { return @(& git -C $Repo diff --name-only $DiffRange | Where-Object { $_.Trim() }) }
    throw 'Provide ChangedListPath or DiffRange.'
}
function Test-XytPattern($Path, $Pattern) {
    return $Path -like $Pattern
}
