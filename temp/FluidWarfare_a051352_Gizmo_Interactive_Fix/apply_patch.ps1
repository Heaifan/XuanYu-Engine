param(
    [Parameter(Mandatory = $false)]
    [string]$RepoRoot = (Get-Location).Path
)

$ErrorActionPreference = 'Stop'
$PatchRoot = Join-Path $PSScriptRoot 'patch'

if (-not (Test-Path $PatchRoot)) {
    throw "未找到 patch 目录：$PatchRoot"
}

$solution = Join-Path $RepoRoot 'FluidWarfare.sln'
if (-not (Test-Path $solution)) {
    throw "目标目录不是 FluidWarfare 仓库根目录：$RepoRoot"
}

$files = Get-ChildItem $PatchRoot -Recurse -File
foreach ($file in $files) {
    $relative = $file.FullName.Substring($PatchRoot.Length).TrimStart('\', '/')
    $destination = Join-Path $RepoRoot $relative
    $destinationDirectory = Split-Path $destination -Parent

    if (-not (Test-Path $destinationDirectory)) {
        New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
    }

    Copy-Item $file.FullName $destination -Force
    Write-Host "[覆盖] $relative"
}

Write-Host "`n已复制 $($files.Count) 个文件。请执行 build/test/run 和人工 GUI 验收。"
