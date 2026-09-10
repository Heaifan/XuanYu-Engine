@echo off
setlocal
set VERSION=0.2.28.60-rz
title XuanYu Engine Editor v0.2.28.60-rz

cd /d "%~dp0"
if errorlevel 1 (
    echo [ERROR] Cannot enter repository root.
    pause
    exit /b 1
)

rem Each machine may point XUANYU_DOTNET at its own SDK executable (D:, E:, etc.).
rem Example: setx XUANYU_DOTNET "X:\MyApp\sdk-dotnet\dotnet.exe"
set "DOTNET_EXE="
if defined XUANYU_DOTNET call :try_dotnet "%XUANYU_DOTNET%"
if not defined DOTNET_EXE call :try_dotnet "%~dp0sdk-dotnet\dotnet.exe"
if not defined DOTNET_EXE call :try_dotnet "%~dp0.dotnet\dotnet.exe"
rem Probe the portable MyApp SDK layout without assuming D: or E:.
if not defined DOTNET_EXE for %%R in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do if not defined DOTNET_EXE call :try_dotnet "%%R:\MyApp\sdk-dotnet\dotnet.exe"
if not defined DOTNET_EXE for /f "delims=" %%D in ('where dotnet 2^>nul') do if not defined DOTNET_EXE call :try_dotnet "%%D"
if not defined DOTNET_EXE (
    echo [ERROR] .NET SDK not found.
    echo Set XUANYU_DOTNET to the machine's SDK dotnet.exe path, or add it to PATH.
    pause
    exit /b 1
)
echo Using .NET SDK: "%DOTNET_EXE%"

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
call "%DOTNET_EXE%" build-server shutdown >nul 2>&1 || ver >nul
%SystemRoot%\System32\timeout.exe /t 1 /nobreak >nul 2>&1 || ver >nul

echo.
echo [1/3] Restoring packages...
call "%DOTNET_EXE%" restore "%PROJECT%" --configfile ".\NuGet.Config" -nologo
if errorlevel 1 goto fail

echo.
echo [2/3] Building app...
set "MSBUILDDISABLENODEREUSE=1"
call "%DOTNET_EXE%" build "%PROJECT%" --no-restore -nologo -clp:Summary=false -m:1 -nr:false -p:UseSharedCompilation=false
if errorlevel 1 goto fail

echo.
echo [3/3] Starting editor...
echo.
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

:try_dotnet
if defined DOTNET_EXE exit /b 0
if not exist "%~1" exit /b 0
call "%~1" --list-sdks >nul 2>&1
if errorlevel 1 exit /b 0
set "DOTNET_EXE=%~1"
exit /b 0
