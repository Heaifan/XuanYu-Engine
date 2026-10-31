$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$files = @('scripts/xye-build.ps1','scripts/xye-dotnet.ps1','scripts/xye-local-environment.ps1')
foreach ($relative in $files) {
    $tokens = $null; $errors = $null
    [Management.Automation.Language.Parser]::ParseFile((Join-Path $root $relative),[ref]$tokens,[ref]$errors) | Out-Null
    if ($errors.Count) { throw "PowerShell parse failed for $relative`: $($errors -join '; ')" }
    Write-Output "PASS: PowerShell parse $relative"
}

$wrapper = Get-Content -LiteralPath (Join-Path $root 'scripts/xye-dotnet.ps1') -Raw
if ($wrapper.IndexOf('Enter-XyeLocalEnvironment') -gt $wrapper.IndexOf('resolve-dotnet.ps1')) {
    throw 'Test entry resolves the SDK before applying its isolated environment.'
}
if ($wrapper -notmatch 'Assert-XyeInvocationPaths' -or $wrapper -notmatch 'Exit-XyeLocalEnvironment') {
    throw 'Test entry is missing independent path safety or environment restoration.'
}
$build = Get-Content -LiteralPath (Join-Path $root 'scripts/xye-build.ps1') -Raw
if ($build -match 'fallbackPackages|legacyPackages|RestoreFallbackFolders=\$') {
    throw 'Build entry still contains an external NuGet fallback path.'
}
if ($build -notmatch 'RestoreFallbackFolders=') { throw 'Build restore does not explicitly disable fallback folders.' }
Write-Output 'PASS: both entrypoints independently guard execution; external fallback removed.'

. (Join-Path $root 'scripts/xye-local-environment.ps1')
$before = @{}
foreach ($name in @('DOTNET_CLI_HOME','NUGET_PACKAGES','NUGET_HTTP_CACHE_PATH','NUGET_SCRATCH',
    'NUGET_PLUGINS_CACHE_PATH','TEMP','TMP','DOTNET_CLI_TELEMETRY_OPTOUT','DOTNET_SKIP_FIRST_TIME_EXPERIENCE',
    'MSBUILDDISABLENODEREUSE','DOTNET_CLI_USE_MSBUILD_SERVER','UseSharedCompilation')) {
    $before[$name] = [Environment]::GetEnvironmentVariable($name,'Process')
}
$state = $null
try {
    $state = Enter-XyeLocalEnvironment -RepositoryRoot $root
    foreach ($name in @('DOTNET_CLI_HOME','NUGET_PACKAGES','NUGET_HTTP_CACHE_PATH','NUGET_SCRATCH',
        'NUGET_PLUGINS_CACHE_PATH','TEMP','TMP')) {
        $actual = [Environment]::GetEnvironmentVariable($name,'Process')
        Assert-XyeLocalPath -RepositoryRoot $root -Path $actual | Out-Null
    }
    $resultRoot = Join-Path $root '.xye-local/test-results'
    Assert-XyeInvocationPaths -RepositoryRoot $root -Arguments @('--results-directory',$resultRoot)
    $outsideRejected = $false
    try { Assert-XyeInvocationPaths -RepositoryRoot $root -Arguments @('--results-directory','C:\xye-gate-outside') }
    catch { $outsideRejected = $true }
    if (-not $outsideRejected) { throw 'External test-results path was not rejected.' }
    $loggerRejected = $false
    try { Assert-XyeInvocationPaths -RepositoryRoot $root -Arguments @('--logger','trx;LogFileName=C:\xye-gate-outside.trx') }
    catch { $loggerRejected = $true }
    if (-not $loggerRejected) { throw 'External logger path was not rejected.' }
    $fallbackRejected = $false
    try { Assert-XyeInvocationPaths -RepositoryRoot $root -Arguments @('-p:RestoreFallbackFolders=C:\packages') }
    catch { $fallbackRejected = $true }
    if (-not $fallbackRejected) { throw 'External NuGet fallback override was not rejected.' }
    foreach ($unsafe in @(
        @('-p:VSTestResultsDirectory=C:\xye-gate-outside'),
        @('-flp:LogFile=C:\xye-gate-outside.log'),
        @('-bl:C:\xye-gate-outside.binlog')
    )) {
        $rejected = $false
        try { Assert-XyeInvocationPaths -RepositoryRoot $root -Arguments $unsafe }
        catch { $rejected = $true }
        if (-not $rejected) { throw "Unsafe output was not rejected: $($unsafe -join ' ')" }
    }
    $junction = Join-Path $state.LocalRoot ('gate-selftest-' + [Guid]::NewGuid().ToString('N'))
    try {
        New-Item -ItemType Junction -Path $junction -Target $resultRoot -ErrorAction Stop | Out-Null
        $junctionRejected = $false
        try { Assert-XyeInvocationPaths -RepositoryRoot $root -Arguments @('--results-directory',(Join-Path $junction 'nested')) }
        catch { $junctionRejected = $true }
        if (-not $junctionRejected) { throw 'A reparse-point output path was not rejected.' }
    } finally {
        if (Test-Path -LiteralPath $junction) { Remove-Item -LiteralPath $junction -Force -ErrorAction Stop }
    }
    Write-Output 'PASS: local environment, in-repo paths, and external path/fallback refusal simulation.'
} finally {
    Exit-XyeLocalEnvironment -State $state
}
foreach ($name in $before.Keys) {
    if ([Environment]::GetEnvironmentVariable($name,'Process') -cne $before[$name]) {
        throw "Caller environment was not restored for $name."
    }
}
Write-Output 'PASS: caller process environment restored; no .NET SDK was invoked.'
