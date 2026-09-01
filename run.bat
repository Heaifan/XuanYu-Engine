@echo off
setlocal
title XuanYu Engine Editor v0.2.28.46-rz

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

rem [0/3] Kill previous editor instance and shutdown build servers to avoid
rem PDB/DLL file locks (CS2012). Only target this editor and MSBuild servers;
rem NEVER use "taskkill /IM dotnet.exe" (would kill unrelated .NET tasks).
echo [0/3] Closing previous editor instance...
taskkill /IM XuanYu.Editor.App.exe /T /F >nul 2>&1 || ver >nul
"D:\MyApp\sdk-dotnet\dotnet.exe" build-server shutdown >nul 2>&1 || ver >nul
%SystemRoot%\System32\timeout.exe /t 1 /nobreak >nul 2>&1 || ver >nul

echo.
echo [1/3] Restoring packages...
call "D:\MyApp\sdk-dotnet\dotnet.exe" restore "%PROJECT%" --configfile ".\NuGet.Config" -nologo
if errorlevel 1 goto fail

echo.
echo [2/3] Building app...
set "MSBUILDDISABLENODEREUSE=1"
call "D:\MyApp\sdk-dotnet\dotnet.exe" build "%PROJECT%" --no-restore -nologo -clp:Summary=false -p:UseSharedCompilation=false
if errorlevel 1 goto fail

echo.
echo [3/3] Starting editor...
echo.
call "D:\MyApp\sdk-dotnet\dotnet.exe" run --project "%PROJECT%" --no-build
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
