[CmdletBinding()]
param([string]$RepoRoot)
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($RepoRoot)) { $RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot) }
. (Join-Path $PSScriptRoot 'version-lib.ps1')
$props = [xml](Get-Content -Raw (Join-Path $RepoRoot 'Directory.Build.props'))
$version = ([string]$props.Project.PropertyGroup.InformationalVersion | Select-Object -First 1).Trim()
$parsed = ConvertTo-ProcessVersion $version
$branch = (git -C $RepoRoot branch --show-current).Trim()
$head = (git -C $RepoRoot rev-parse HEAD).Trim()
$short = (git -C $RepoRoot rev-parse --short=8 HEAD).Trim()
$dirty = [bool](git -C $RepoRoot status --porcelain)
$remoteOutput = git -C $RepoRoot rev-parse --abbrev-ref --symbolic-full-name '@{upstream}' 2>$null
if ($LASTEXITCODE -eq 0 -and $remoteOutput) { $remote = ([string]$remoteOutput).Trim() }
else { $remote = ''; $global:LASTEXITCODE = 0 }
$remoteHead = if ($remote) { (git -C $RepoRoot rev-parse $remote).Trim() } else { 'UNAVAILABLE' }
$counts = @{}
foreach ($line in (Get-Content (Join-Path $RepoRoot 'changelog.md') | Where-Object { $_ -match '^##\s+(v\S+)' })) {
    $key = ([regex]::Match($line, '^##\s+(v\S+)')).Groups[1].Value
    $counts[$key] = 1 + ($counts[$key] | ForEach-Object { $_ })
}
$collisions = @($counts.GetEnumerator() | Where-Object Value -gt 1 | ForEach-Object { $_.Name })
$aheadBehind = if ($remote) { git -C $RepoRoot rev-list --left-right --count "$remote...HEAD" } else { 'N/A' }
Write-Output "Current Process Version: $version"
Write-Output "Branch: $branch"
Write-Output "HEAD: $head"
Write-Output "Short SHA: $short"
Write-Output "Dirty: $(if ($dirty) {'YES'} else {'NO'})"
Write-Output "Remote HEAD: $remoteHead"
Write-Output "Ahead / Behind: $aheadBehind"
Write-Output "Canonical Identity: $version@$short"
Write-Output "Parse: PASS"
Write-Output "Historical Collision: $(if ($collisions.Count) { 'PRESENT ' + ($collisions -join ', ') } else { 'NONE DETECTED' })"
Write-Output "Acceptance SHA Audit: REVIEW REQUIRED (legacy records may be incomplete; no historical data invented)"
Write-Output "Current Change Audit: $(if ($dirty) { 'REVIEW REQUIRED (DIRTY; classify a Version Event before formal close)' } else { 'CLEAN' })"
