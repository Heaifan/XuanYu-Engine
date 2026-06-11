<#
.SYNOPSIS
    使用 spirv-val 验证 basic_3d.vert.spv / basic_3d.frag.spv。

.DESCRIPTION
    标准 SPIR-V 验证脚本。本机必须安装 spirv-val（Vulkan SDK 或 SPIRV-Tools）。
    找不到工具或 .spv 缺失时输出中文提示并返回非 0 退出码。
    不会假装通过验证。

.NOTES
    从仓库根目录执行：
    powershell -ExecutionPolicy Bypass -File tools/shaders/validate_basic_3d.ps1
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Push-Location $repoRoot

try
{
    # 1. 检查 spirv-val
    $spirvVal = Get-Command "spirv-val" -ErrorAction SilentlyContinue
    if (-not $spirvVal)
    {
        Write-Host "错误：未检测到 spirv-val。" -ForegroundColor Red
        Write-Host "请安装 Vulkan SDK 或将 spirv-val 所在目录加入 PATH。" -ForegroundColor Yellow
        exit 1
    }

    Write-Host "检测到 spirv-val：$($spirvVal.Source)" -ForegroundColor Green

    # 2. 检查 .spv 文件是否存在
    $vertSpv = Join-Path $repoRoot "FluidWarfare.Render.Vulkan\Shaders\Compiled\basic_3d.vert.spv"
    $fragSpv = Join-Path $repoRoot "FluidWarfare.Render.Vulkan\Shaders\Compiled\basic_3d.frag.spv"

    $missing = @()
    if (-not (Test-Path $vertSpv)) { $missing += "basic_3d.vert.spv" }
    if (-not (Test-Path $fragSpv)) { $missing += "basic_3d.frag.spv" }

    if ($missing.Count -gt 0)
    {
        Write-Host "错误：未找到以下文件：" -ForegroundColor Red
        foreach ($m in $missing) { Write-Host "  - $m" -ForegroundColor Red }
        Write-Host "请先执行编译脚本：" -ForegroundColor Yellow
        Write-Host "  powershell -ExecutionPolicy Bypass -File tools/shaders/compile_basic_3d.ps1" -ForegroundColor Yellow
        exit 1
    }

    # 3. 验证顶点着色器
    Write-Host "验证顶点着色器：basic_3d.vert.spv" -ForegroundColor Cyan
    & $spirvVal.Source $vertSpv
    if ($LASTEXITCODE -ne 0)
    {
        Write-Host "错误：basic_3d.vert.spv 验证未通过。" -ForegroundColor Red
        exit 1
    }
    Write-Host "  SPIR-V 验证通过：basic_3d.vert.spv" -ForegroundColor Green

    # 4. 验证片段着色器
    Write-Host "验证片段着色器：basic_3d.frag.spv" -ForegroundColor Cyan
    & $spirvVal.Source $fragSpv
    if ($LASTEXITCODE -ne 0)
    {
        Write-Host "错误：basic_3d.frag.spv 验证未通过。" -ForegroundColor Red
        exit 1
    }
    Write-Host "  SPIR-V 验证通过：basic_3d.frag.spv" -ForegroundColor Green

    Write-Host ""
    Write-Host "全部 SPIR-V 验证通过。可以进行下一步。" -ForegroundColor Green
}
finally
{
    Pop-Location
}
