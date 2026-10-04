[CmdletBinding()]
param(
    [string]$Root = '',
    [string]$CurrentMonth = '2026-10'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($Root)) { $Root = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path }

function Stop-Gate([string]$Message) { Write-Error "CHANGELOG GATE: FAIL-CLOSED: $Message"; exit 1 }
function Read-Utf8NoBom([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { Stop-Gate "Missing file: $Path" }
    $Bytes = [IO.File]::ReadAllBytes($Path)
    if (($Bytes.Length -ge 2) -and ($Bytes[0] -eq 0xFF) -and ($Bytes[1] -eq 0xFE)) { Stop-Gate "UTF-16 BOM detected: $Path" }
    if (($Bytes.Length -ge 3) -and ($Bytes[0] -eq 0xEF) -and ($Bytes[1] -eq 0xBB) -and ($Bytes[2] -eq 0xBF)) { Stop-Gate "UTF-8 BOM detected: $Path" }
    try { return [Text.UTF8Encoding]::new($false, $true).GetString($Bytes) }
    catch { Stop-Gate "Invalid UTF-8 bytes: $Path" }
}
function Get-MonthHeading([string]$Text) { @([regex]::Matches($Text, '(?m)^# (20\d\d-\d\d)\s*$') | % { $_.Groups[1].Value }) }
function Get-EntryBody([string]$Text) {
    $Match = [regex]::Match($Text, '(?m)^## .+$')
    if (-not $Match.Success) { Stop-Gate 'No changelog entries found after month metadata.' }
    $Text.Substring($Match.Index).TrimEnd("`r", "`n")
}

$RootPath = Join-Path $Root 'changelog.md'; $ArchiveDir = Join-Path $Root 'docs/archive/changelog'
$CurrentDate = [DateTime]::ParseExact(($CurrentMonth + '-01'), 'yyyy-MM-dd', $null)
$PreviousMonth = $CurrentDate.AddMonths(-1).ToString('yyyy-MM')
$RootText = Read-Utf8NoBom $RootPath; $RootMonths = Get-MonthHeading $RootText
if (($RootMonths -join ',') -ne "$CurrentMonth,$PreviousMonth") { Stop-Gate "Root month window is '$($RootMonths -join ',')', expected '$CurrentMonth,$PreviousMonth'." }

$PreviousArchive = Join-Path $ArchiveDir ("changelog-$PreviousMonth.md"); $PreviousArchiveText = Read-Utf8NoBom $PreviousArchive
if ((Get-EntryBody $RootText) -notmatch '(?s)^## ') { Stop-Gate 'Root has no parseable entries.' }
$RootPrevious = [regex]::Match($RootText, "(?ms)^# $([regex]::Escape($PreviousMonth))\s*$.*?(?=^# |\z)").Value
if (-not $RootPrevious) { Stop-Gate "Root mirror section missing: $PreviousMonth" }
if ((Get-EntryBody $RootPrevious) -ne (Get-EntryBody $PreviousArchiveText)) { Stop-Gate "Root/$PreviousMonth does not exactly mirror the archive body." }

$ArchiveFiles = @(Get-ChildItem -LiteralPath $ArchiveDir -Filter 'changelog-*.md' -File); $ArchiveMonths = @()
foreach ($File in $ArchiveFiles) {
    if ($File.Name -notmatch '^changelog-(20\d\d-\d\d)\.md$') { Stop-Gate "Invalid archive filename: $($File.Name)" }
    $Month = $Matches[1]; if ($ArchiveMonths -contains $Month) { Stop-Gate "Duplicate archive month: $Month" }; $ArchiveMonths += $Month
    $ArchiveText = Read-Utf8NoBom $File.FullName
    $Declared = [regex]::Match($ArchiveText, '(?m)^<!-- Records: (\d+); Month: (20\d\d-\d\d) -->$')
    if (-not $Declared.Success -or $Declared.Groups[2].Value -ne $Month) { Stop-Gate "Archive metadata missing or mismatched: $($File.Name)" }
    $EntryCount = @([regex]::Matches((Get-EntryBody $ArchiveText), '(?m)^## ')).Count
    if ([int]$Declared.Groups[1].Value -ne $EntryCount) { Stop-Gate "Archive record count mismatch: $($File.Name) declared=$($Declared.Groups[1].Value) actual=$EntryCount" }
    $EntryBody = Get-EntryBody $ArchiveText; $Dates = @([regex]::Matches($EntryBody, '(?<!\d)2026-(\d\d)-\d\d(?!\d)') | % { $_.Groups[1].Value })
    foreach ($MonthNumber in ($Dates | Select-Object -Unique)) { if ($MonthNumber -ne $Month.Substring(5, 2)) { Stop-Gate "Archive boundary crosses month: $($File.Name) contains 2026-$MonthNumber" } }
}
if (-not ($ArchiveMonths -contains $PreviousMonth)) { Stop-Gate "Previous-month archive missing: $PreviousMonth" }
$Sorted = @($ArchiveMonths | Sort-Object)
for ($Index = 1; $Index -lt $Sorted.Count; $Index++) {
    $Expected = ([DateTime]::ParseExact(($Sorted[$Index - 1] + '-01'), 'yyyy-MM-dd', $null)).AddMonths(1).ToString('yyyy-MM')
    if ($Sorted[$Index] -ne $Expected) { Stop-Gate "Archive inventory has a month gap between $($Sorted[$Index - 1]) and $($Sorted[$Index])." }
}
foreach ($Month in $ArchiveMonths) { if ($Month -gt $PreviousMonth) { Stop-Gate "Archive is newer than previous month: $Month" } }

Write-Output 'CHANGELOG GATE: PASS'; Write-Output 'Encoding: UTF-8 without BOM'
Write-Output "Root month window: $CurrentMonth + $PreviousMonth"; Write-Output "Previous mirror: $PreviousMonth exact entry-body match"
Write-Output "Archive inventory: $($Sorted -join ', ')"; Write-Output 'Archive boundary: PASS'; Write-Output 'Parseability: PASS'; Write-Output 'No silent loss witness: archive record counts verified'
