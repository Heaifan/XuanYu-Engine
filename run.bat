@echo off
setlocal

cd /d "%~dp0"
if errorlevel 1 (
    echo [ERROR] Cannot enter repository root.
    pause
    exit /b 1
)

set "VERSION="
for /f "usebackq delims=" %%V in (`powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\resolve-version.ps1"`) do if not defined VERSION set "VERSION=%%V"
if not defined VERSION (
    echo [ERROR] Product version source could not be read.
    pause
    exit /b 1
)
title XuanYu Engine Editor %VERSION%

set "DOTNET_EXE="
for /f "usebackq delims=" %%D in (`powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\resolve-dotnet.ps1"`) do if not defined DOTNET_EXE set "DOTNET_EXE=%%D"
if not defined DOTNET_EXE (
    echo [ERROR] .NET SDK not found.
    pause
    exit /b 1
)
echo Using .NET SDK: "%DOTNET_EXE%"

set "PROJECT=.\XuanYu.Editor.App\XuanYu.Editor.App.csproj"

echo ========================================
echo    XuanYu Engine Editor - Build and Run
echo ========================================
echo.

rem [0/2] Close only the editor before checking/reusing its build outputs.
echo [0/2] Closing previous editor instance...
taskkill /IM XuanYu.Editor.App.exe /T /F >nul 2>&1 || ver >nul

echo.
echo [1/2] Reusing or building app outputs...
call powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\xye-build.ps1" -Project "XuanYu.Editor.App\XuanYu.Editor.App.csproj"
if errorlevel 1 goto fail

echo.
echo [2/2] Starting editor...
echo.
set "WT_DIRTY=false"
for /f "delims=" %%S in ('git status --porcelain') do set "WT_DIRTY=true"
set "GIT_HEAD="
for /f "delims=" %%H in ('git rev-parse HEAD') do if not defined GIT_HEAD set "GIT_HEAD=%%H"
set "APP_EXE=%CD%\XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe"
set "APP_TIME="
for /f "usebackq delims=" %%T in (`powershell -NoProfile -Command "(Get-Item -LiteralPath '%APP_EXE%').LastWriteTime.ToString('o')"`) do if not defined APP_TIME set "APP_TIME=%%T"
echo RuntimeBuildIdentity
echo Git HEAD=%GIT_HEAD%
echo WorkingTreeDirty=%WT_DIRTY%
echo Assembly Build Timestamp=%APP_TIME%
echo Executable Path=%APP_EXE%
call "%DOTNET_EXE%" run --project "%PROJECT%" --no-build
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
