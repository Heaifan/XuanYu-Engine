@echo off
setlocal
title XuanYu Engine Editor v0.2.17.9-fix

cd /d "%~dp0"
if errorlevel 1 (
    echo [ERROR] Cannot enter repository root.
    pause
    exit /b 1
)

set "PROJECT=.\XuanYu.Editor.App\XuanYu.Editor.App.csproj"

echo ========================================
echo    XuanYu Engine Editor - Build and Run
echo ========================================
echo.

echo [1/3] Restoring packages...
call dotnet restore "%PROJECT%" --configfile ".\NuGet.Config" -nologo
if errorlevel 1 goto fail

echo.
echo [2/3] Building app...
call dotnet build "%PROJECT%" --no-restore -nologo -clp:Summary=false
if errorlevel 1 goto fail

echo.
echo [3/3] Starting editor...
echo.
call dotnet run --project "%PROJECT%" --no-build
set "exitCode=%errorlevel%"

if not "%exitCode%"=="0" goto failWithCode
exit /b 0

:fail
set "exitCode=%errorlevel%"

:failWithCode
echo.
echo [ERROR] Editor failed. Exit code: %exitCode%
pause
exit /b %exitCode%
