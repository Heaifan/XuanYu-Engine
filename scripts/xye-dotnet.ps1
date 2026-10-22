$ErrorActionPreference = 'Stop'
$resolver = Join-Path $PSScriptRoot 'resolve-dotnet.ps1'
$dotnet = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $resolver
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($dotnet)) {
    exit $(if ($LASTEXITCODE -ne 0) { $LASTEXITCODE } else { 1 })
}

$forwarded = [Collections.Generic.List[string]]::new()
for ($index = 0; $index -lt $args.Count; $index++) {
    if ($args[$index] -in @('-m', '-nr', '-p', '-clp') -and $index + 1 -lt $args.Count) {
        $forwarded.Add("$($args[$index]):$($args[$index + 1])")
        $index++
    } else { $forwarded.Add([string]$args[$index]) }
}
$operation = if ($args.Count -gt 0) { [string]$args[0] } else { '' }
$workspaceOperations = @('build','test','publish','pack','restore','clean','run')

if ($operation -in $workspaceOperations) {
    $workspaceRunner = Join-Path $PSScriptRoot 'xye-workspace.ps1'
    if ([string]::IsNullOrWhiteSpace($env:XYE_TASK_RUN_ID)) {
        $powershell = (Get-Command powershell.exe -ErrorAction Stop).Source
        $childArguments = @('-NoProfile','-ExecutionPolicy','Bypass','-File',$PSCommandPath) + @($forwarded)
        & $workspaceRunner -Action Invoke -TaskType "dotnet-$operation" `
            -RepositoryRoot (Get-Location).Path -SourceRoot (Get-Location).Path `
            -ExecutablePath $powershell -ExecutableArguments $childArguments
        exit $LASTEXITCODE
    }

    if ([string]::IsNullOrWhiteSpace($env:XYE_BUILD_DIRECTORY)) {
        throw 'XYE_BUILD_DIRECTORY is missing from the controlled task context.'
    }
    $nodeReuseFound = $false
    for ($index = 0; $index -lt $forwarded.Count; $index++) {
        if ($forwarded[$index] -match '^(?:-|/)(?:nr|nodeReuse)(?::|=|$)') {
            $forwarded[$index] = '-nr:false'
            $nodeReuseFound = $true
        }
    }
    if (-not $nodeReuseFound) { $forwarded.Add('-nr:false') }
    $buildRoot = [IO.Path]::GetFullPath($env:XYE_BUILD_DIRECTORY)
    $buildPrefix = $buildRoot.TrimEnd('\') + '\'
    Import-Module (Join-Path $PSScriptRoot 'XyeWorkspace.psm1') -Force -DisableNameChecking
    $cacheRoot = Join-Path $env:XYE_WORKSPACE_ROOT 'Cache'
    $nugetRoot = Join-Path $cacheRoot 'NuGet'
    $dotnetHome = Join-Path $cacheRoot 'Dotnet'
    $env:NUGET_PACKAGES = Join-Path $nugetRoot 'Packages'
    $env:NUGET_HTTP_CACHE_PATH = Join-Path $nugetRoot 'Http'
    $env:NUGET_PLUGINS_CACHE_PATH = Join-Path $nugetRoot 'Plugins'
    $env:NUGET_SCRATCH = Join-Path $nugetRoot 'Scratch'
    $env:DOTNET_CLI_HOME = $dotnetHome
    $hasArtifactsPath = $false
    for ($index = 0; $index -lt $forwarded.Count; $index++) {
        if ($forwarded[$index] -in @('--artifacts-path','--output','-o')) {
            if ($index + 1 -ge $forwarded.Count) { throw "Missing path after $($forwarded[$index])." }
            $path = $forwarded[$index + 1]
            $fullPath = if ([IO.Path]::IsPathRooted($path)) { [IO.Path]::GetFullPath($path) }
                else { [IO.Path]::GetFullPath((Join-Path (Get-Location).Path $path)) }
            if ($fullPath -ne $buildRoot -and -not $fullPath.StartsWith($buildPrefix, [StringComparison]::OrdinalIgnoreCase)) {
                throw "Build output path must be inside XYE_BUILD_DIRECTORY: $fullPath"
            }
            $hasArtifactsPath = $hasArtifactsPath -or $forwarded[$index] -eq '--artifacts-path'
            $index++
        } elseif ($forwarded[$index] -like '--artifacts-path=*') {
            $path = $forwarded[$index].Substring('--artifacts-path='.Length)
            $fullPath = if ([IO.Path]::IsPathRooted($path)) { [IO.Path]::GetFullPath($path) }
                else { [IO.Path]::GetFullPath((Join-Path (Get-Location).Path $path)) }
            if ($fullPath -ne $buildRoot -and -not $fullPath.StartsWith($buildPrefix, [StringComparison]::OrdinalIgnoreCase)) {
                throw "Artifacts path must be inside XYE_BUILD_DIRECTORY: $fullPath"
            }
            $hasArtifactsPath = $true
        } elseif ($forwarded[$index] -match '(?i)^(?:-p:|/(?:p|property):)(BaseOutputPath|BaseIntermediateOutputPath|MSBuildProjectExtensionsPath|IntermediateOutputPath|OutDir)=(.*)$') {
            $path = $Matches[2]
            if ([string]::IsNullOrWhiteSpace($path)) { throw "MSBuild output/intermediate path cannot be empty: $($forwarded[$index])" }
            $fullPath = if ([IO.Path]::IsPathRooted($path)) { [IO.Path]::GetFullPath($path) }
                else { [IO.Path]::GetFullPath((Join-Path (Get-Location).Path $path)) }
            if (-not $fullPath.StartsWith($buildPrefix, [StringComparison]::OrdinalIgnoreCase)) {
                throw "MSBuild output/intermediate path must be inside XYE_BUILD_DIRECTORY: $fullPath"
            }
        }
    }
    if (-not $hasArtifactsPath) {
        $forwarded.Add('--artifacts-path')
        $forwarded.Add($env:XYE_BUILD_DIRECTORY)
    }
    if ($forwarded.Contains('')) {
        $filtered = [Collections.Generic.List[string]]::new()
        foreach ($value in $forwarded) { if ($value -ne '') { $filtered.Add($value) } }
        $forwarded = $filtered
    }
}

$started = [DateTimeOffset]::UtcNow.ToString('O')
& $dotnet @forwarded
$exitCode = $LASTEXITCODE
if (-not [string]::IsNullOrWhiteSpace($env:XYE_TASK_COMMAND_LOG)) {
    $record = [pscustomobject]@{
        Executable=$dotnet; Arguments=(Protect-XyeArguments $forwarded.ToArray());
        WorkingDirectory=(Get-Location).Path; StartedUtc=$started
        CompletedUtc=[DateTimeOffset]::UtcNow.ToString('O'); ExitCode=$exitCode
    } | ConvertTo-Json -Compress
    Add-Content -LiteralPath $env:XYE_TASK_COMMAND_LOG -Value $record -Encoding utf8
}
exit $exitCode
