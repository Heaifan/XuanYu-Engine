param(
    [string]$Source = '',
    [string]$OutputDirectory = '',
    [int]$Replicates = 5,
    [int]$Frames = 30
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if ([string]::IsNullOrWhiteSpace($Source)) { $Source = Join-Path $root 'artifacts\diagnostics\wp2-p4-r3\n24e121.hgt' }
$Source = (Resolve-Path -LiteralPath $Source).Path
$expected = 'BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1'
if ((Get-FileHash $Source -Algorithm SHA256).Hash -ne $expected) { throw 'HGT SHA-256 不匹配。' }
$allowed = [IO.Path]::GetFullPath((Join-Path $root 'artifacts\diagnostics\xyc-b2'))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) { $OutputDirectory = Join-Path $allowed (Get-Date -Format 'yyyyMMdd-HHmmss') }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not $OutputDirectory.StartsWith($allowed + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) { throw '输出必须位于 artifacts\diagnostics\xyc-b2 下。' }
if (Test-Path $OutputDirectory) { throw '输出目录已存在，拒绝覆盖。' }
if ($Replicates -lt 5 -or $Frames -lt 30) { throw '至少需要 5 个独立进程和 30 帧。' }
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$bootstrap = & (Join-Path $root 'scripts\xye-bootstrap.ps1')
if ($LASTEXITCODE -ne 0) { throw 'Bootstrap 未通过。' }
$dotnet = & (Join-Path $root 'scripts\resolve-dotnet.ps1')
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($dotnet)) { throw 'SDK resolver 失败。' }
& (Join-Path $root 'scripts\xye-dotnet.ps1') build (Join-Path $PSScriptRoot 'XuanYu.OffscreenTerrain.csproj') --no-restore -m 1 -nr false -p BuildInParallel=false -p UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'Offscreen benchmark 构建失败。' }
$head = (& git -C $root rev-parse HEAD).Trim()
$version = (Select-String -Path (Join-Path $root 'Directory.Build.props') -Pattern '<InformationalVersion>([^<]+)</InformationalVersion>').Matches.Groups[1].Value
$env:XYC_BUILD_ID = "HEAD=$head;VERSION=$version;DIRTY=YES;B2=1"
$env:XYE_PROBE_VIEWPORT = '1'; $env:XYE_TERRAIN_REPLAY = '1'
$dll = Join-Path $PSScriptRoot 'bin\Debug\net10.0\XuanYu.OffscreenTerrain.dll'
$configs = @(
    @{ Name='1025-auto'; Samples=1025; Lod='baseline'; Chunk=240 },
    @{ Name='2049-auto'; Samples=2049; Lod='baseline'; Chunk=240 },
    @{ Name='2049-lod-bias-1'; Samples=2049; Lod='lod-bias-1'; Chunk=240 },
    @{ Name='2049-lod0'; Samples=2049; Lod='lod0'; Chunk=240 },
    @{ Name='2049-chunk240-lod0'; Samples=2049; Lod='lod0'; Chunk=240 },
    @{ Name='2049-chunk480-lod0'; Samples=2049; Lod='lod0'; Chunk=480 },
    @{ Name='2049-chunk480-auto'; Samples=2049; Lod='baseline'; Chunk=480 }
)
$runs = [Collections.Generic.List[object]]::new(); $errors = [Collections.Generic.List[string]]::new()
function Invoke-Benchmark($config, [int]$replicate) {
    $name = $config.Name; $sample = $config.Samples; $lod = $config.Lod; $chunk = $config.Chunk
    $folder = Join-Path $OutputDirectory "$name\replicate-$replicate"
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
    $env:XYE_TERRAIN_AB = $lod; $env:XYE_TERRAIN_CHUNK_CELLS = "$chunk"
    $before = (& nvidia-smi --query-gpu=timestamp,name,driver_version,clocks.gr,clocks.mem,temperature.gpu,power.draw,utilization.gpu,memory.used --format=csv,noheader,nounits 2>&1 | Out-String).Trim()
    $started = [DateTimeOffset]::UtcNow; $timer = [Diagnostics.Stopwatch]::StartNew()
    & $dotnet $dll --benchmark --source $Source --out $folder --replicate 1 --samples "$sample" --pose near-oblique --warmup 3 --frames $Frames 2>&1 | Tee-Object (Join-Path $folder 'run.log')
    $code = $LASTEXITCODE; $timer.Stop()
    $after = (& nvidia-smi --query-gpu=timestamp,name,driver_version,clocks.gr,clocks.mem,temperature.gpu,power.draw,utilization.gpu,memory.used --format=csv,noheader,nounits 2>&1 | Out-String).Trim()
    [pscustomobject]@{ config=$name; replicate=$replicate; order=$runs.Count+1; startedUtc=$started.ToString('O'); elapsedSeconds=$timer.Elapsed.TotalSeconds; exitCode=$code; gpuBefore=$before; gpuAfter=$after } | ConvertTo-Json -Compress | Add-Content (Join-Path $OutputDirectory 'process-runs.jsonl')
    $runs.Add([pscustomobject]@{config=$name;replicate=$replicate;order=$runs.Count+1;exitCode=$code})
    if ($code -ne 0) { $errors.Add("$name replicate $replicate exit=$code") }
}
for ($i = 1; $i -le $Replicates; $i++) { Invoke-Benchmark $configs[0] $i; Invoke-Benchmark $configs[1] $i }
foreach ($config in $configs | Select-Object -Skip 2) { for ($i = 1; $i -le $Replicates; $i++) { Invoke-Benchmark $config $i } }
$runs | Export-Csv (Join-Path $OutputDirectory 'run-order.csv') -NoTypeInformation -Encoding utf8
if ($errors.Count -gt 0) { $errors | Set-Content (Join-Path $OutputDirectory 'run-errors.txt'); throw ($errors -join '; ') }
$python = 'C:\Users\Heai\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe'
& $python (Join-Path $PSScriptRoot 'Analyze-XYC-B2.py') $OutputDirectory
if ($LASTEXITCODE -ne 0) { throw 'B2 图表和质量分析失败。' }
Write-Output "B2 REPORT: $(Join-Path $OutputDirectory 'report.md')"
