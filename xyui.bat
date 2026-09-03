@echo off
setlocal
title XYUI.Avalonia.Gallery
echo Starting XYUI.Avalonia.Gallery...
set "PROJECT=%~dp0xyui\avalonia\gallery\XYUI.Avalonia.Gallery\XYUI.Avalonia.Gallery.csproj"
if not exist "%PROJECT%" (
    echo [ERROR] Gallery project not found: %PROJECT%
    exit /b 1
)

set "DOTNET="
if exist "D:\MyApp\sdk-dotnet\dotnet.exe" set "DOTNET=D:\MyApp\sdk-dotnet\dotnet.exe"
if not defined DOTNET if exist "E:\MyApp\sdk-dotnet\dotnet.exe" set "DOTNET=E:\MyApp\sdk-dotnet\dotnet.exe"
if not defined DOTNET if exist "C:\Program Files\dotnet\dotnet.exe" set "DOTNET=C:\Program Files\dotnet\dotnet.exe"
if not defined DOTNET for /f "delims=" %%D in ('where dotnet 2^>nul') do if not defined DOTNET set "DOTNET=%%D"
if not defined DOTNET (
    echo [ERROR] dotnet.exe not found in D:\MyApp, E:\MyApp, C:\Program Files, or PATH.
    exit /b 1
)

echo Using dotnet: %DOTNET%
call "%DOTNET%" run --project "%PROJECT%"
set "exitCode=%errorlevel%"
if not "%exitCode%"=="0" echo [ERROR] Gallery exited with code %exitCode%.
exit /b %exitCode%
