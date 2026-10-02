@echo off
setlocal
pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0handclap.ps1" %*
exit /b %ERRORLEVEL%
