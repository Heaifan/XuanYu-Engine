param(
    [string]$Source = '',
    [string]$OutputDirectory = '',
    [string]$ErrorTable = '',
    [int]$Replicates = 5,
    [int]$Frames = 30
)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if ([string]::IsNullOrWhiteSpace($Source)) { $Source = Join-Path $root 'artifacts\diagnostics\wp2-p4-r3\n24e121.hgt' }
if ([string]::IsNullOrWhiteSpace($ErrorTable)) { $ErrorTable = Join-Path $root 'artifacts\diagnostics\xyc-b3\dev-error-table.json' }
$Source = (Resolve-Path -LiteralPath $Source).Path; $ErrorTable = (Resolve-Path -LiteralPath $ErrorTable).Path
if ((Get-FileHash $Source -Algorithm SHA256).Hash -ne 'BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1') { throw 'HGT SHA-256 不匹配' }
if ($Replicates -lt 5 -or $Frames -lt 30) { throw '每配置至少 5 个独立进程、每进程至少 30 帧' }
$allowed = [IO.Path]::GetFullPath((Join-Path $root 'artifacts\diagnostics\xyc-b3'))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) { $OutputDirectory = Join-Path $allowed (Get-Date -Format 'yyyyMMdd-HHmmss') }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not $OutputDirectory.StartsWith($allowed + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) { throw '输出目录必须位于 artifacts\diagnostics\xyc-b3' }
if (Test-Path $OutputDirectory) { throw '拒绝覆盖已有实验目录' }
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$bootstrap = & (Join-Path $root 'scripts\xye-bootstrap.ps1'); if ($LASTEXITCODE -ne 0) { throw 'Bootstrap 未通过' }
$dotnet = & (Join-Path $root 'scripts\resolve-dotnet.ps1'); if ($LASTEXITCODE -ne 0) { throw 'SDK resolver 失败' }
& (Join-Path $root 'scripts\xye-dotnet.ps1') build (Join-Path $PSScriptRoot 'XuanYu.OffscreenTerrain.csproj') --no-restore -m 1 -nr false -p BuildInParallel=false -p UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'Offscreen build failed' }
$dll = Join-Path $PSScriptRoot 'bin\Debug\net10.0\XuanYu.OffscreenTerrain.dll'
$head = (& git -C $root rev-parse HEAD).Trim(); $env:XYC_BUILD_ID = "HEAD=$head;B3=1;DIRTY=YES"
$env:XYE_PROBE_VIEWPORT = '1'; $env:XYE_TERRAIN_REPLAY = '1'; $env:XYE_TERRAIN_SSE_ERROR_TABLE = $ErrorTable
$env:XYE_B3_FIXED_TERRAIN = '1'
$configs = @(@('baseline','baseline'), @('lod-bias-1','lod-bias-1'), @('sse-0.5','sse-0.5'), @('sse-1','sse-1'), @('sse-2','sse-2'), @('sse-4','sse-4'))
$order = [Collections.Generic.List[object]]::new(); $failures = [Collections.Generic.List[string]]::new()
for ($rep = 1; $rep -le $Replicates; $rep++) {
    $rotation = ($rep - 1) % $configs.Count
    for ($offset = 0; $offset -lt $configs.Count; $offset++) {
        $config = $configs[($offset + $rotation) % $configs.Count]; $name = $config[0]; $variant = $config[1]
        $folder = Join-Path $OutputDirectory "$name\replicate-$rep"; New-Item -ItemType Directory -Path $folder -Force | Out-Null
        $env:XYE_TERRAIN_AB = $variant; $env:XYE_PROBE_DIRECTORY = Join-Path $folder 'probes'
        (& nvidia-smi --query-gpu=timestamp,name,driver_version,clocks.gr,clocks.mem,temperature.gpu,power.draw,utilization.gpu,memory.used --format=csv,noheader,nounits 2>&1 | Out-String).Trim() | Set-Content (Join-Path $folder 'gpu-before.csv')
        $started = [DateTimeOffset]::UtcNow; $watch = [Diagnostics.Stopwatch]::StartNew()
        & $dotnet $dll --benchmark --source $Source --out $folder --replicate $rep --samples 2049 --pose near-oblique --warmup 3 --frames $Frames 2>&1 | Tee-Object (Join-Path $folder 'process.log')
        $code = $LASTEXITCODE; $watch.Stop()
        (& nvidia-smi --query-gpu=timestamp,name,driver_version,clocks.gr,clocks.mem,temperature.gpu,power.draw,utilization.gpu,memory.used --format=csv,noheader,nounits 2>&1 | Out-String).Trim() | Set-Content (Join-Path $folder 'gpu-after.csv')
        $row = [pscustomobject]@{config=$name;replicate=$rep;order=$order.Count+1;startedUtc=$started.ToString('O');elapsedSeconds=$watch.Elapsed.TotalSeconds;exitCode=$code;gpuBefore=(Get-Content (Join-Path $folder 'gpu-before.csv') -Raw).Trim();gpuAfter=(Get-Content (Join-Path $folder 'gpu-after.csv') -Raw).Trim()}
        $row | ConvertTo-Json -Compress | Add-Content (Join-Path $OutputDirectory 'process-runs.jsonl'); $order.Add($row)
        if ($code -ne 0) { $failures.Add("$name replicate $rep exit=$code") }
    }
}
$order | Select-Object config,replicate,order,startedUtc,elapsedSeconds,exitCode | Export-Csv (Join-Path $OutputDirectory 'run-order.csv') -NoTypeInformation -Encoding utf8
if ($failures.Count -gt 0) { $failures | Set-Content (Join-Path $OutputDirectory 'run-errors.txt'); throw ($failures -join '; ') }
Write-Output "B3 raw process bundle: $OutputDirectory"
