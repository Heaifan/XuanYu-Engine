$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$resolver = Join-Path $repo 'scripts\resolve-dotnet.ps1'
$wrapper = Join-Path $repo 'scripts\xuanyu-dotnet.ps1'
$temp = Join-Path ([IO.Path]::GetTempPath()) "xuanyu-dotnet-contract-$([Guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $temp | Out-Null
$fake = Join-Path $temp 'fake-dotnet.cmd'
$log = Join-Path $temp 'args.log'
@"
@echo off
if "%1"=="--list-sdks" (
  echo 9.0.100 [fake]
  exit /b 0
)
echo %*>>"$log"
exit /b 0
"@ | Set-Content -LiteralPath $fake -Encoding ascii

function Invoke-Script([string]$path, [string[]]$args = @()) {
    $shell = (Get-Process -Id $PID).Path
    & $shell -NoProfile -ExecutionPolicy Bypass -File $path @args 2>&1
    $script:LastScriptExitCode = $LASTEXITCODE
}

$oldDotnet = $env:XUANYU_DOTNET
$oldPath = $env:PATH
$shell = (Get-Process -Id $PID).Path
try {
    $env:XUANYU_DOTNET = $fake
    $resolved = (Invoke-Script $resolver)
    if ($resolved.Count -ne 1 -or $resolved -ne $fake) { throw 'XUANYU_DOTNET priority contract failed.' }

    $env:XUANYU_DOTNET = Join-Path $temp 'missing-dotnet.cmd'
    $env:PATH = $temp
    $oldPreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    & $shell -NoProfile -ExecutionPolicy Bypass -File $resolver -NoFallback 2>$null | Out-Null
    $ErrorActionPreference = $oldPreference
    $failureExitCode = $LASTEXITCODE
    if ($failureExitCode -eq 0) { throw 'Resolver must fail when no valid SDK candidate exists.' }

    $env:XUANYU_DOTNET = $fake
    & $shell -NoProfile -ExecutionPolicy Bypass -File $wrapper build Foo.csproj --no-restore '-m:1' '-p:BuildInParallel=false' | Out-Null
    $actualArgs = Get-Content -Raw $log
    if ($actualArgs -notmatch 'build Foo.csproj --no-restore -m:1 -p:BuildInParallel=false') {
        throw "Wrapper argument pass-through failed: [$actualArgs]"
    }

    $run = Get-Content -Raw (Join-Path $repo 'run.bat')
    if ($run -notmatch 'resolve-dotnet\.ps1' -or $run -match ':try_dotnet|where dotnet|sdk-dotnet') {
        throw 'run.bat still owns a private SDK discovery algorithm.'
    }
    Write-Output 'resolve-dotnet contract: PASS'
} finally {
    if ($null -eq $oldDotnet) { Remove-Item Env:XUANYU_DOTNET -ErrorAction SilentlyContinue } else { $env:XUANYU_DOTNET = $oldDotnet }
    $env:PATH = $oldPath
    Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue
}
