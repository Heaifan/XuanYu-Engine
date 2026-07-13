@echo off
setlocal
title XuanYu Engine Editor

chcp 65001 >nul
cd /d "%~dp0" || (
    echo 启动失败：无法切换到仓库根目录。
    pause
    exit /b 1
)

set "PROJECT=.\XuanYu.Editor.App\XuanYu.Editor.App.csproj"

echo ========================================
echo    玄域引擎编辑器 - 构建并启动
echo ========================================
echo.

echo [1/3] 还原依赖...
call dotnet restore "%PROJECT%" --configfile ".\NuGet.Config" -nologo
if errorlevel 1 goto fail

echo.
echo [2/3] 构建应用...
call dotnet build "%PROJECT%" --no-restore -nologo -clp:Summary=false
if errorlevel 1 goto fail

echo.
echo [3/3] 启动编辑器...
echo.
call dotnet run --project "%PROJECT%" --no-build
set "exitCode=%errorlevel%"

if not "%exitCode%"=="0" goto failWithCode
exit /b 0

:fail
set "exitCode=%errorlevel%"

:failWithCode
echo.
echo 启动失败，退出码：%exitCode%
pause
exit /b %exitCode%
