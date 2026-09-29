@echo off
setlocal
cd /d "%~dp0"
if errorlevel 1 (
    echo [ERROR] Cannot enter repository root.
    exit /b 1
)

pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0xyt.ps1" %*
exit /b %errorlevel%
