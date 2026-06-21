@echo off
chcp 65001 >nul
title FluidWarfare Editor

echo ========================================
echo    FluidWarfare Editor - 构建并启动
echo ========================================
echo.

echo [1/2] 正在构建解决方案...
call dotnet build FluidWarfare.sln -nologo -clp:Summary=false
if %ERRORLEVEL% neq 0 (
    echo.
    echo [失败] 构建出错，请检查上方错误信息。
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] 正在启动 Editor...
echo.
echo --- dotnet output start ---
call dotnet run --project FluidWarfare.Editor.Windows --no-build
set EDITOR_EXIT_CODE=%ERRORLEVEL%
echo --- dotnet output end ---
echo.
if %EDITOR_EXIT_CODE% neq 0 (
    echo [失败] Editor 启动失败。退出码：%EDITOR_EXIT_CODE%
    echo 如果上方有异常堆栈，请优先关注异常类型与行号。
    pause
    exit /b %EDITOR_EXIT_CODE%
)