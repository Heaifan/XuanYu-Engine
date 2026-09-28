[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateSet('Planning', 'Execution')]
    [string]$Phase,
    [Parameter(Mandatory)]
    [string]$Domain,
    [string[]]$Keywords = @()
)

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$indexPath = Join-Path $repoRoot 'docs\knowledge\knowledge-index.md'
$indexLines = Get-Content -LiteralPath $indexPath
$terms = [System.Collections.Generic.List[string]]::new()
if ($Domain.Trim()) { [void]$terms.Add($Domain) }
 $normalizedKeywords = @($Keywords | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
foreach ($keyword in $normalizedKeywords) {
    if ($keyword -and $keyword.Trim()) { [void]$terms.Add($keyword) }
}
$hits = @(
    foreach ($line in $indexLines) {
        $matched = $false
        foreach ($term in $terms) {
            if ($line.IndexOf($term, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
                $matched = $true
                break
            }
        }
        if ($matched) { $line.Trim() }
    }
)

$ids = @()
foreach ($hit in $hits) {
    $ids += [regex]::Matches($hit, '(?:K|L|EXP|DEC)-[A-Z]+-\d{3}') | ForEach-Object Value
    foreach ($range in [regex]::Matches($hit, '(?<prefix>(?:K|L)-[A-Z]+-)(?<start>\d{3})[～~-]+(?:K|L)-[A-Z]+-(?<end>\d{3})')) {
        $start = [int]$range.Groups['start'].Value
        $end = [int]$range.Groups['end'].Value
        foreach ($number in $start..$end) { $ids += '{0}{1:D3}' -f $range.Groups['prefix'].Value, $number }
    }
}
$ids = @($ids | Sort-Object -Unique)

$paths = foreach ($id in $ids) {
    $file = if ($id -match '^K-REN-') { 'docs\knowledge\rendering.md' }
        elseif ($id -match '^L-REN-') { 'docs\knowledge\lessons.md' }
        elseif ($id -match '^INC-') { 'docs\knowledge\incidents.md' }
        elseif ($id -match '^EXP-') { 'docs\governance\agent-experience-rules.md' }
        elseif ($id -match '^ERR-') { 'docs\governance\agent-error-log.md' }
        else { 'docs\knowledge\knowledge-index.md' }
    '{0} -> {1}' -f $id, $file
}

Write-Output "KNOWLEDGE PREFLIGHT — $($Phase.ToUpperInvariant())"
Write-Output "Phase: $($Phase.ToUpperInvariant())"
Write-Output "Domain: $Domain"
Write-Output "Keywords: $([string]::Join(', ', $normalizedKeywords))"
Write-Output 'Index Hits:'
if ($hits.Count) { $hits | ForEach-Object { Write-Output "- $_" } } else { Write-Output '- none' }
Write-Output 'Candidate IDs:'
if ($ids.Count) { $ids | ForEach-Object { Write-Output "- $_" } } else { Write-Output '- none' }
Write-Output 'Paths:'
if ($paths) { $paths | ForEach-Object { Write-Output "- $_" } } else { Write-Output '- none' }
if ($ids.Count) { Write-Output 'Result: MATCHES FOUND' } else { Write-Output 'Result: NO MATCHES' }
