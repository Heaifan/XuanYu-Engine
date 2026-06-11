<#
.SYNOPSIS
    使用 glslangValidator 编译 basic_3d.vert / basic_3d.frag 到 SPIR-V。

.DESCRIPTION
    标准 Shader 编译脚本。本机必须安装 glslangValidator（Vulkan SDK 或独立安装）。
    找不到工具时输出中文错误提示并返回非 0 退出码，不会生成伪造 .spv。

    安装方式：
    - Vulkan SDK: https://vulkan.lunarg.com/sdk/home
    - 独立 glslang: https://github.com/KhronosGroup/glslang/releases

    注意：禁止使用 tools/gen_spirv（手写 SPIR-V 编码器）作为替代。
    手写 SPIR-V 已确认为 Milestone 8.1 闪退根因。

.NOTES
    从仓库根目录执行：
    powershell -ExecutionPolicy Bypass -File tools/shaders/compile_basic_3d.ps1
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
Push-Location $repoRoot

try
{
    # 1. 检查 glslangValidator
    $glslang = Get-Command "glslangValidator" -ErrorAction SilentlyContinue
    if (-not $glslang)
    {
        Write-Host "错误：未检测到 glslangValidator。" -ForegroundColor Red
        Write-Host "请安装 Vulkan SDK 或将 glslangValidator 所在目录加入 PATH。" -ForegroundColor Yellow
        Write-Host "下载地址：https://vulkan.lunarg.com/sdk/home" -ForegroundColor Yellow
        exit 1
    }

    Write-Host "检测到 glslangValidator：$($glslang.Source)" -ForegroundColor Green

    # 2. 确保 Compiled 目录存在
    $compiledDir = Join-Path $repoRoot "FluidWarfare.Render.Vulkan\Shaders\Compiled"
    New-Item -ItemType Directory -Force -Path $compiledDir | Out-Null

    # 3. 编译顶点着色器
    $vertSrc = Join-Path $repoRoot "FluidWarfare.Render.Vulkan\Shaders\basic_3d.vert"
    $vertOut = Join-Path $compiledDir "basic_3d.vert.spv"

    Write-Host "编译顶点着色器：basic_3d.vert" -ForegroundColor Cyan
    & $glslang.Source -V -o $vertOut $vertSrc
    if ($LASTEXITCODE -ne 0)
    {
        Write-Host "错误：顶点着色器编译失败。" -ForegroundColor Red
        exit 1
    }
    Write-Host "  输出：$vertOut" -ForegroundColor Green

    # 4. 编译片段着色器
    $fragSrc = Join-Path $repoRoot "FluidWarfare.Render.Vulkan\Shaders\basic_3d.frag"
    $fragOut = Join-Path $compiledDir "basic_3d.frag.spv"

    Write-Host "编译片段着色器：basic_3d.frag" -ForegroundColor Cyan
    & $glslang.Source -V -o $fragOut $fragSrc
    if ($LASTEXITCODE -ne 0)
    {
        Write-Host "错误：片段着色器编译失败。" -ForegroundColor Red
        exit 1
    }
    Write-Host "  输出：$fragOut" -ForegroundColor Green

    Write-Host ""
    Write-Host "编译完成。" -ForegroundColor Green
    Write-Host "下一步执行验证：" -ForegroundColor Yellow
    Write-Host "  powershell -ExecutionPolicy Bypass -File tools/shaders/validate_basic_3d.ps1" -ForegroundColor Yellow
}
finally
{
    Pop-Location
}
