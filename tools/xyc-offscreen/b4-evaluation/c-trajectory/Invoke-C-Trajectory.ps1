param(
    [switch]$ExecuteGpu,
    [ValidateSet('baseline','sse-4','both')][string]$Variant = 'both',
    [string]$Source = '',
    [string]$ErrorTable = ''
)
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
if ([string]::IsNullOrWhiteSpace($env:XYC_RUN_ID)) { $env:XYC_RUN_ID = [Guid]::NewGuid().ToString('N') }
if ([string]::IsNullOrWhiteSpace($Source)) { $Source = Join-Path $repo 'artifacts\diagnostics\wp2-p4-r3\n24e121.hgt' }
if ([string]::IsNullOrWhiteSpace($ErrorTable)) { $ErrorTable = Join-Path $repo 'artifacts\diagnostics\xyc-b3\20261015-202400\error-table.json' }
$Source = (Resolve-Path -LiteralPath $Source).Path; $ErrorTable = (Resolve-Path -LiteralPath $ErrorTable).Path
$diagnostics = Join-Path $repo 'artifacts\diagnostics\xyc-c-trajectory'
$run = $diagnostics
$head = if ($env:XYE_SOURCE_HEAD) { $env:XYE_SOURCE_HEAD } else { (& git -C $repo rev-parse HEAD).Trim() }
$dirty = if ($env:XYE_SOURCE_DIRTY) { $env:XYE_SOURCE_DIRTY } elseif (@(git -C $repo status --porcelain).Count -gt 0) { 'YES' } else { 'NO' }
$bootstrap = & (Join-Path $repo 'scripts\xye-bootstrap.ps1')
if ($LASTEXITCODE -ne 0) { throw 'Bootstrap failed' }
$dotnet = & (Join-Path $repo 'scripts\resolve-dotnet.ps1')
if ($LASTEXITCODE -ne 0) { throw 'SDK resolver failed' }
$project = Join-Path $PSScriptRoot 'XuanYu.B4Trajectory.csproj'
& (Join-Path $repo 'scripts\xye-build.ps1') -Project 'tools/xyc-offscreen/b4-evaluation/c-trajectory/XuanYu.B4Trajectory.csproj' -Configuration Debug
if ($LASTEXITCODE -ne 0) { throw 'B4 trajectory runner build failed' }
if (-not $ExecuteGpu) { Write-Output "PREPARED_ONLY=$PSScriptRoot"; return }
New-Item -ItemType Directory -Force -Path $diagnostics | Out-Null
if (Test-Path -LiteralPath $run) { throw "Refusing to overwrite existing run: $run" }
New-Item -ItemType Directory -Path $run | Out-Null
$dll = Join-Path $PSScriptRoot 'bin\Debug\net10.0\XuanYu.B4Trajectory.dll'
$variants = if ($Variant -eq 'both') { @('baseline','sse-4') } else { @($Variant) }
foreach ($item in $variants) {
    $folder = Join-Path $run $item; New-Item -ItemType Directory -Path $folder | Out-Null
    & $dotnet $dll --source $Source --trajectory (Join-Path $PSScriptRoot 'trajectory.json') `
        --out $folder --variant $item --error-table $ErrorTable --head $head --dirty $dirty 2>&1 |
        Tee-Object (Join-Path $folder 'process.log')
    if ($LASTEXITCODE -ne 0) { throw "B4 $item capture failed with exit $LASTEXITCODE" }
}
& python (Join-Path $PSScriptRoot 'analyze_c_trajectory.py') $run --source $Source
if ($LASTEXITCODE -ne 0) { throw 'C trajectory analysis failed' }
Write-Output "C-TRAJECTORY-REPORT=$(Join-Path $run 'C-TRAJECTORY-REPORT.md')"
