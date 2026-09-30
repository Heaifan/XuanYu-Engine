[CmdletBinding()]
param([string]$RepoPath = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path)

$ErrorActionPreference = 'Stop'
$roots = @(
    'XuanYu.Core.Tests', 'XuanYu.World.Tests', 'XuanYu.WarCore.Tests',
    'xyui/avalonia/tests/XYUI.Avalonia.Tests')
$attribute = '^\s*\[(Fact|Theory)(Attribute)?(\([^]]*\))?\]'

function Get-Counts([string]$root, [string]$mode) {
    if ($mode -eq 'HEAD') {
        $paths = @(git -C $RepoPath ls-tree -r --name-only HEAD |
            Where-Object { $_.StartsWith("$root/") -and $_.EndsWith('.cs') })
        $lines = @(git -C $RepoPath grep -n -E $attribute HEAD -- "$root/*.cs")
        $fact = @($lines | Where-Object { $_ -match '\[Fact' }).Count
        $theory = @($lines | Where-Object { $_ -match '\[Theory' }).Count
    } else {
        $paths = @(Get-ChildItem (Join-Path $RepoPath $root) -Recurse -Filter '*.cs' -File |
            ForEach-Object { $_.FullName })
        $fact = 0; $theory = 0
        foreach ($path in $paths) {
            $text = Get-Content -Raw $path
            $fact += ([regex]::Matches($text, "(?m)$attribute") |
                Where-Object { $_.Groups[1].Value -eq 'Fact' }).Count
            $theory += ([regex]::Matches($text, "(?m)$attribute") |
                Where-Object { $_.Groups[1].Value -eq 'Theory' }).Count
        }
    }
    [pscustomobject]@{
        Root = $root; Mode = $mode; CsFiles = $paths.Count
        Fact = $fact; Theory = $theory; Definitions = $fact + $theory
    }
}

$rows = foreach ($root in $roots) {
    Get-Counts $root 'HEAD'
    Get-Counts $root 'CANDIDATE'
}
$rows | ConvertTo-Json -Depth 4
