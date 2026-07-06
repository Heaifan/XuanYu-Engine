@echo off
setlocal
title XuanYu Engine Editor

pushd "%~dp0" || (
    echo [ERROR] Cannot enter project directory.
    pause
    exit /b 1
)

echo ========================================
echo    XuanYu Engine Editor - Build and Run
echo ========================================
echo.

set "BUILD_TARGET=.\XuanYu.Editor.UI\XuanYu.Editor.UI.csproj"
set "RUN_ARGS=--project .\XuanYu.Editor.UI\XuanYu.Editor.UI.csproj --no-build"
set "TARGET_NAME=Editor UI"

if exist "XuanYu.Engine.sln" (
    set "BUILD_TARGET=XuanYu.Engine.sln"
    set "RUN_ARGS=--project XuanYu.Engine.Editor.Windows --no-build"
    set "TARGET_NAME=solution"
)

echo [1/3] Restoring %TARGET_NAME% packages...
call dotnet restore "%BUILD_TARGET%" --configfile ".\NuGet.Config" -nologo
if errorlevel 1 (
    echo.
    echo [ERROR] Restore failed. Check the messages above.
    pause
    popd
    exit /b %ERRORLEVEL%
)

echo.
echo [2/3] Building %TARGET_NAME%...
call dotnet build "%BUILD_TARGET%" --no-restore -nologo -clp:Summary=false
if errorlevel 1 (
    echo.
    echo [ERROR] Build failed. Check the messages above.
    pause
    popd
    exit /b %ERRORLEVEL%
)

echo.
echo [3/3] Starting Editor...
echo.
echo --- dotnet output start ---
call dotnet run %RUN_ARGS%
set "EDITOR_EXIT_CODE=%ERRORLEVEL%"
echo --- dotnet output end ---
echo.

if not "%EDITOR_EXIT_CODE%"=="0" (
    echo [ERROR] Editor failed to start. Exit code: %EDITOR_EXIT_CODE%
    pause
    popd
    exit /b %EDITOR_EXIT_CODE%
)

popd
exit /b 0
