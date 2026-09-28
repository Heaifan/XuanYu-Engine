# Shared governance Guard runtime. Dot-source this file before using any Guard helper.
if ($null -eq $global:__XyeGuardContext) {
    $global:__XyeGuardContext = [pscustomobject]@{
        Failures = New-Object System.Collections.Generic.List[string]
        Standalone = $true
        MainRun = $false
    }
}

function Initialize-GuardBootstrap([string]$Root, [switch]$NewRun) {
    if ($NewRun) {
        $global:__XyeGuardContext = [pscustomobject]@{
            Failures = New-Object System.Collections.Generic.List[string]
            Standalone = $false
            MainRun = $true
        }
    }
    if ($Root) { Set-Location -LiteralPath $Root }
    return $global:__XyeGuardContext
}

function Add-Failure([string]$message) {
    $global:__XyeGuardContext.Failures.Add($message)
}

function Read-Text([string]$path) {
    Get-Content -LiteralPath $path -Raw -Encoding utf8
}

function Get-SourceFiles([string]$dir) {
    @($(git ls-files "$dir/*") + $(git ls-files --others --exclude-standard "$dir/*")) |
        Where-Object { $_ -match '\.(cs|axaml|js)$' -and (Test-Path -LiteralPath $_) } |
        Sort-Object -Unique |
        ForEach-Object { Get-Item -LiteralPath $_ }
}

function Get-ProjectReferences([string]$path) {
    $matches = [regex]::Matches((Read-Text $path), '<ProjectReference\s+Include="([^"]+)"')
    foreach ($match in $matches) { $match.Groups[1].Value }
}

function Assert-Contains([string]$path, [string]$needle, [string]$label) {
    if ((Read-Text $path).IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
        Add-Failure "$label missing: $needle"
    }
}

function Assert-NotContains([string]$path, [string[]]$needles, [string]$label) {
    $text = Read-Text $path
    foreach ($needle in $needles) {
        if ($text.IndexOf($needle, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            Add-Failure "$label forbidden: $needle ($path)"
        }
    }
}

function Complete-GuardRun([string]$successMessage, [switch]$Child) {
    if ($Child -and $global:__XyeGuardContext.MainRun) { return }
    if ($global:__XyeGuardContext.Failures.Count -gt 0) {
        $global:__XyeGuardContext.Failures | ForEach-Object { Write-Error $_ }
        exit 1
    }
    Write-Host $successMessage
}
