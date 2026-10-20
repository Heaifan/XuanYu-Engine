param(
    [Parameter(Mandatory=$true)][string]$RunDirectory,
    [ValidateSet('baseline','sse-0.5','sse-1','sse-2','sse-4')][string]$Variant = 'sse-4',
    [string]$Source = '',
    [string]$ErrorTable = ''
)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if ([string]::IsNullOrWhiteSpace($env:XYE_TASK_RUN_ID) -or [string]::IsNullOrWhiteSpace($env:XYE_DIAGNOSTICS_DIRECTORY)) { throw 'Run XYC B3 visual capture through scripts\xye-workspace.ps1.' }
if ([string]::IsNullOrWhiteSpace($Source)) { $Source = Join-Path $root 'artifacts\diagnostics\wp2-p4-r3\n24e121.hgt' }
if ([string]::IsNullOrWhiteSpace($ErrorTable)) { $ErrorTable = Join-Path $root 'artifacts\diagnostics\xyc-b3\20261015-202400\error-table.json' }
$Source = (Resolve-Path -LiteralPath $Source).Path; $ErrorTable = (Resolve-Path -LiteralPath $ErrorTable).Path
$RunDirectory = (Resolve-Path -LiteralPath $RunDirectory).Path
$allowed = [IO.Path]::GetFullPath((Join-Path $env:XYE_WORKSPACE_ROOT 'Diagnostics'))
if (-not $RunDirectory.StartsWith($allowed + [IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)) { throw 'RunDirectory 必须在 XYE workspace Diagnostics 下。' }
$output = Join-Path $env:XYE_DIAGNOSTICS_DIRECTORY "trajectory-$Variant"; if (Test-Path $output) { throw '拒绝覆盖轨迹输出目录' }
$bootstrap = & (Join-Path $root 'scripts\xye-bootstrap.ps1'); if ($LASTEXITCODE -ne 0) { throw 'Bootstrap 未通过' }
$dotnet = & (Join-Path $root 'scripts\resolve-dotnet.ps1'); if ($LASTEXITCODE -ne 0) { throw 'SDK resolver 失败' }
& (Join-Path $root 'scripts\xye-dotnet.ps1') build (Join-Path $PSScriptRoot 'XuanYu.OffscreenTerrain.csproj') --no-restore -m 1 -nr false -p BuildInParallel=false -p UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'Offscreen build failed' }
$env:XYC_BUILD_ID = "HEAD=$env:XYE_SOURCE_HEAD;B3=VIS;DIRTY=$env:XYE_SOURCE_DIRTY"
$env:XYE_PROBE_VIEWPORT = '1'; $env:XYE_TERRAIN_REPLAY = '1'; $env:XYE_TERRAIN_AB = $Variant
$env:XYE_TERRAIN_SSE_ERROR_TABLE = $ErrorTable; $env:XYE_B3_FIXED_TERRAIN = '1'
$env:XYE_PROBE_DIRECTORY = Join-Path $output 'probes'
$dll = Join-Path $env:XYE_BUILD_DIRECTORY 'bin\XuanYu.OffscreenTerrain\debug\XuanYu.OffscreenTerrain.dll'
& $dotnet $dll --b3-capture --source $Source --trajectory (Join-Path $PSScriptRoot 'b3-visual\trajectory.json') --out $output 2>&1 | Tee-Object (Join-Path $env:XYE_DIAGNOSTICS_DIRECTORY "trajectory-$Variant.log")
if ($LASTEXITCODE -ne 0) { throw "B3 trajectory capture failed: $LASTEXITCODE" }
& python (Join-Path $PSScriptRoot 'b3-visual\analyze_b3.py') $output
if ($LASTEXITCODE -ne 0) { throw 'B3 visual trajectory analysis failed' }
Write-Output "B3 trajectory: $output"
