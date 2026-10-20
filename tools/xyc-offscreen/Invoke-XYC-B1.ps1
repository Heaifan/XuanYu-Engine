param(
    [string]$Source = '',
    [string]$OutputDirectory = ''
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if ([string]::IsNullOrWhiteSpace($env:XYE_TASK_RUN_ID) -or [string]::IsNullOrWhiteSpace($env:XYE_DIAGNOSTICS_DIRECTORY)) { throw 'Run XYC B1 through scripts\xye-workspace.ps1.' }
if ([string]::IsNullOrWhiteSpace($Source)) {
    $Source = Join-Path $repoRoot 'artifacts\diagnostics\wp2-p4-r3\n24e121.hgt'
}
$Source = (Resolve-Path -LiteralPath $Source).Path
$expectedHash = 'BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1'
if ((Get-FileHash -LiteralPath $Source -Algorithm SHA256).Hash -ne $expectedHash) {
    throw 'n24e121 HGT SHA-256 与 Issue #32 基线不匹配。'
}
$allowedRoot = Join-Path $env:XYE_DIAGNOSTICS_DIRECTORY 'xyc-r0-b1'
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = $allowedRoot
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not $OutputDirectory.Equals($allowedRoot,[StringComparison]::OrdinalIgnoreCase) -and
    -not $OutputDirectory.StartsWith($allowedRoot + [IO.Path]::DirectorySeparatorChar,
        [StringComparison]::OrdinalIgnoreCase)) { throw '输出目录必须位于当前任务 Diagnostics 下。' }
if (Test-Path -LiteralPath $OutputDirectory) { throw '输出目录已存在；为保护既有证据，本脚本拒绝覆盖。' }
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

$bootstrap = & (Join-Path $repoRoot 'scripts\xye-bootstrap.ps1')
if ($LASTEXITCODE -ne 0) { throw 'Bootstrap 未通过。' }
$dotnet = & (Join-Path $repoRoot 'scripts\resolve-dotnet.ps1')
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($dotnet)) { throw '正式 .NET Resolver 未返回 SDK。' }
& (Join-Path $repoRoot 'scripts\xye-dotnet.ps1') build (Join-Path $PSScriptRoot 'XuanYu.OffscreenTerrain.csproj') --no-restore
if ($LASTEXITCODE -ne 0) { throw 'XYC Offscreen benchmark Build 失败。' }

$head = if ($env:XYE_SOURCE_HEAD) { $env:XYE_SOURCE_HEAD } else { (& git -C $repoRoot rev-parse HEAD).Trim() }
$version = (Select-String -Path (Join-Path $repoRoot 'Directory.Build.props') -Pattern '<InformationalVersion>([^<]+)</InformationalVersion>').Matches.Groups[1].Value
$env:XYC_BUILD_ID = "HEAD=$head;VERSION=$version;DIRTY=$env:XYE_SOURCE_DIRTY"
$dll = Join-Path $env:XYE_BUILD_DIRECTORY 'bin\XuanYu.OffscreenTerrain\debug\XuanYu.OffscreenTerrain.dll'
$exitCodes = @()
for ($replicate = 1; $replicate -le 3; $replicate++) {
    $arguments = @($dll, '--benchmark', '--source', $Source, '--out', $OutputDirectory,
        '--replicate', "$replicate", '--samples', '513,1025,2049', '--pose', 'all', '--warmup', '3', '--frames', '30')
    & $dotnet @arguments 2>&1 | Tee-Object -FilePath (Join-Path $OutputDirectory "run-$replicate.log")
    $exitCodes += $LASTEXITCODE
}
$manifests = 1..3 | ForEach-Object { Join-Path $OutputDirectory "run-$_\run-manifest.json" }
if (@($manifests | Where-Object { -not (Test-Path -LiteralPath $_) }).Count -gt 0) {
    throw '至少一个独立进程未生成 manifest；保留部分证据，未聚合。'
}
& $dotnet $dll --aggregate --out $OutputDirectory 2>&1 |
    Tee-Object -FilePath (Join-Path $OutputDirectory 'aggregate.log')
if ($LASTEXITCODE -ne 0) { throw 'B1 聚合报告包含未通过或缺失配置。' }
if ($exitCodes | Where-Object { $_ -ne 0 }) { throw "部分 GPU 配置失败，原始结果保留于 $OutputDirectory。" }
Write-Output "B1 REPORT: $(Join-Path $OutputDirectory 'report.md')"
