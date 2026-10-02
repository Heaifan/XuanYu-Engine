@echo off
setlocal
cd /d "%~dp0\..\.."
where pwsh.exe >nul 2>&1
if not errorlevel 1 (set "HANDOFF_PWSH=pwsh.exe") else (set "HANDOFF_PWSH=powershell.exe")
"%HANDOFF_PWSH%" -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0handoff.ps1" %*
exit /b %ERRORLEVEL%
