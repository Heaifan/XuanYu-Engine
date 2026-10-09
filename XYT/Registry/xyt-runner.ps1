[CmdletBinding()]
param([string]$ChangedListPath='', [string]$DiffRange='', [string]$OutputPath='', [string]$SupplementPath='', [string[]]$AgentTests=@())
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$routes = Get-Content -Raw (Join-Path $PSScriptRoot 'xyt-test-routing.json') | ConvertFrom-Json
function Get-XytChangedFiles {
    if ($ChangedListPath) { return @(Get-Content -LiteralPath $ChangedListPath | Where-Object { $_.Trim() }) }
    if ($DiffRange) { return @(& git -C $repo diff --name-only $DiffRange | Where-Object { $_.Trim() }) }
    throw 'Provide ChangedListPath or DiffRange.'
}
function Resolve-XytTests([string]$Test) {
    if ($Test -notmatch '[*?]') { return @($Test) }
    $rooted = Join-Path $repo ($Test -replace '/', '\')
    $prefix = $Test.Split('*')[0].TrimEnd('/') -replace '/', '\'
    $base = Join-Path $repo $prefix
    if (-not (Test-Path $base)) { return @() }
    $rootPath = Resolve-Path $base
    @(Get-ChildItem -LiteralPath $rootPath -Recurse -File -Filter '*.selftest.ps1' | ForEach-Object { $_.FullName.Substring($repo.Length + 1).Replace('\','/') })
}
$required = [Collections.Generic.List[string]]::new(); $changed=@(); $disputes=[Collections.Generic.List[string]]::new()
foreach ($file in (Get-XytChangedFiles)) {
    $path = $file.Trim().Replace('\','/'); $rules=@($routes.rules | Where-Object { $path -like $_.pattern })
    if (!$rules.Count) { [void]$disputes.Add("$path`: no test route") }
    foreach ($rule in $rules) { foreach ($test in @($rule.tests)) { foreach ($resolved in (Resolve-XytTests $test)) { [void]$required.Add($resolved) } } }
    $changed += [pscustomobject]@{ path=$path; routes=@($rules.pattern) }
}
foreach ($test in $AgentTests) { if ($test.Trim()) { [void]$required.Add($test.Trim()) } }
$items=if($SupplementPath -and (Test-Path -LiteralPath $SupplementPath)){@((Get-Content -Raw $SupplementPath|ConvertFrom-Json).items)}else{@()}
foreach($item in $items){if(!$item.test -or !$item.reason -or !$item.source){[void]$disputes.Add('Supplement requires test, reason, and source')}elseif($item.test){[void]$required.Add([string]$item.test)}}
$result=[pscustomobject]@{schema='XYT-C/2';status=$(if($disputes.Count){'REVIEW_REQUIRED'}else{'PASS'});changedFiles=$changed;requiredTests=@($required|Select-Object -Unique);disputes=@($disputes);agentAdditions=@($AgentTests|Where-Object{$_.Trim()}|Select-Object -Unique);supplementalTests=@($items|ForEach-Object{[string]$_.test}|Where-Object{$_}|Select-Object -Unique)}
$json=$result|ConvertTo-Json -Depth 10
if($OutputPath){$json|Set-Content -LiteralPath $OutputPath -Encoding UTF8}else{$json}
"XYT STATUS = $($result.status)"; "XYT REQUIRED TESTS = $(@($result.requiredTests).Count)"
if($disputes.Count){'XYT DISPUTES = '+($disputes -join '; '); exit 2}
