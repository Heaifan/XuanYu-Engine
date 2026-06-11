@echo off
chcp 65001 >nul
title FluidWarfare Editor

echo ========================================
echo    FluidWarfare Editor - 构建并启动
echo ========================================
echo.

echo [1/2] 正在构建解决方案...
dotnet build FluidWarfare.sln -nologo -clp:Summary=false 2>&1
if %ERRORLEVEL% neq 0 (
    echo.
    echo [失败] 构建出错，请检查上方错误信息。
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] 正在启动 Editor...
echo.
dotnet run --project FluidWarfare.Editor.Windows --no-build
if %ERRORLEVEL% neq 0 (
    echo.
    echo [失败] Editor 启动失败。
    pause
    exit /b %ERRORLEVEL%
)
