$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$files = Get-ChildItem $root -Recurse -File
$forbidden = @(
    ('p'+'repare'), ('j'+'oin'), ('ad'+'vance'), ('cl'+'ose'), ('lane-'+'close'),
    ('migrate-'+'active'), ('lane-'+'state'), ('commit-'+'lock'), ('commit-'+'unlock'),
    ('maint'+'enance'), ('rep'+'air'), ('rel'+'ease'), ('cl'+'aim'), ('rev'+'oke'),
    ('cand'+'idate'), ('g'+'ate')
)
$gitOps = @(
    ('git\s+'+'fetch'), ('git\s+'+'reset'), ('git\s+'+'clean'), ('git\s+'+'checkout'),
    ('git\s+'+'merge'), ('git\s+'+'stash')
)
foreach ($file in $files) {
    $text = Get-Content -Raw $file.FullName
    $production = $file.FullName -notmatch '[\\/]tests[\\/]'
    if ($production) {
        foreach ($word in $forbidden) {
            if ($text -match ('(?i)(?<![a-z-])' + [regex]::Escape($word) + '(?![a-z-])')) {
                throw "AUTHORITY NEGATIVE FAILED: $word in $($file.FullName)"
            }
        }
    }
    foreach ($op in $gitOps) {
        if ($text -match ('(?im)' + $op)) { throw "GIT MUTATION FAILED: $op in $($file.FullName)" }
    }
    foreach ($state in @('state\.json','task-registry\.json','work-release\.json','ownership-locks\.json','candidate\.json')) {
        if ($text -match ('(?i)(Set-Content|Add-Content|WriteAllText|Move-Item).*' + $state)) {
            throw "AUTHORITY STATE WRITE FAILED: $state in $($file.FullName)"
        }
    }
}
'HANDCLAP AUTHORITY NEGATIVE SELFTEST PASS'
