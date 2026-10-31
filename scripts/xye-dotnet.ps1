$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
. (Join-Path $PSScriptRoot 'xye-local-environment.ps1')

$forwarded = [Collections.Generic.List[string]]::new()
for ($i = 0; $i -lt $args.Count; $i++) {
    if ($args[$i] -in @('-m','-nr','-p','-clp') -and $i + 1 -lt $args.Count) {
        $forwarded.Add("$($args[$i]):$($args[++$i])")
    } else { $forwarded.Add([string]$args[$i]) }
}
$operation = if ($forwarded.Count) { $forwarded[0].ToLowerInvariant() } else { '' }
$arguments = @($forwarded | Select-Object -Skip 1)
Assert-XyeInvocationPaths -RepositoryRoot $root -Arguments $arguments

$targets = [Collections.Generic.List[string]]::new()
for ($i=0; $i -lt $arguments.Count; $i++) {
    $token = [string]$arguments[$i]
    if ($token -match '(?i)^(?:-t|/t|-target|/target|--target)(?::|=)(.*)$') { $targets.Add($Matches[1]) }
    elseif ($token -match '(?i)^(?:-t|/t|-target|/target|--target)$' -and $i + 1 -lt $arguments.Count) { $targets.Add([string]$arguments[++$i]) }
}
if (@($targets | ForEach-Object { $_ -split '[;,]' } | Where-Object { $_.Trim() -ieq 'Rebuild' }).Count) {
    throw 'Rebuild is forbidden; use scripts/xye-build.ps1 to reuse canonical outputs.'
}
if ($operation -eq 'test' -and ('--no-build' -notin $arguments -or '--no-restore' -notin $arguments)) {
    throw 'Tests must use --no-build --no-restore. Run scripts/xye-build.ps1 once first.'
}
if ($operation -eq 'build' -and '--no-restore' -notin $arguments) {
    throw 'Build must use --no-restore. Run scripts/xye-build.ps1 for the unified Restore/Build.'
}
$outputProperties = @('ArtifactsPath','BaseOutputPath','OutputPath','OutDir','PublishDir',
    'PackageOutputPath','BaseIntermediateOutputPath','IntermediateOutputPath',
    'MSBuildProjectExtensionsPath','RestorePackagesPath','NuGetPackageRoot',
    'RestoreFallbackFolders','RestoreAdditionalProjectSources','VSTestResultsDirectory',
    'TestResultsDirectory','BaseTestResultsDirectory','VSTestLogger')
for ($i=0; $i -lt $arguments.Count; $i++) {
    $token = [string]$arguments[$i]
    if ($token -match '(?i)^(?:--artifacts-path|--output|-o)(?:$|[=:].*)') {
        throw 'Custom output or NuGet cache paths are forbidden; use canonical repository-local outputs.'
    }
    $property = [regex]::Match($token, '(?i)^(?:-p|/p|-property|/property|--property)(?::|=)(.*)$')
    if ($property.Success) {
        foreach ($assignment in ($property.Groups[1].Value -split '[;,]')) {
            if ($outputProperties -contains ($assignment -split '=',2)[0].Trim()) {
                throw 'Custom output or NuGet cache paths are forbidden; use canonical repository-local outputs.'
            }
        }
    } elseif ($token -match '(?i)^(?:-p|/p|-property|/property|--property)$' -and $i + 1 -lt $arguments.Count) {
        foreach ($assignment in ([string]$arguments[++$i] -split '[;,]')) {
            if ($outputProperties -contains ($assignment -split '=',2)[0].Trim()) {
                throw 'Custom output or NuGet cache paths are forbidden; use canonical repository-local outputs.'
            }
        }
    }
}

$targetPathPattern = '(?i)\.(?:csproj|sln|slnx)$'
foreach ($candidate in $arguments) {
    if ([string]$candidate -match $targetPathPattern) {
        $path = if ([IO.Path]::IsPathRooted([string]$candidate)) { [string]$candidate } else { Join-Path $root ([string]$candidate) }
        Assert-XyeLocalPath -RepositoryRoot $root -Path $path | Out-Null
    }
}

$environmentState = Enter-XyeLocalEnvironment -RepositoryRoot $root
$exitCode = 1
$locationPushed = $false
try {
    Push-Location -LiteralPath $root
    $locationPushed = $true
    $resolver = Join-Path $PSScriptRoot 'resolve-dotnet.ps1'
    $dotnetOutput = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $resolver 2>&1)
    if ($LASTEXITCODE -ne 0 -or $dotnetOutput.Count -eq 0) {
        throw "SDK resolver failed before any dotnet command: $($dotnetOutput -join "`n")"
    }
    $dotnet = [string]$dotnetOutput[-1]
    if ([string]::IsNullOrWhiteSpace($dotnet)) { throw 'SDK resolver returned an empty executable path.' }
    if ($operation -eq 'test' -and -not ($arguments -match '(?i)^--results-directory(?:=|$)')) {
        $arguments += @('--results-directory',(Join-Path $environmentState.LocalRoot 'test-results'))
        $forwarded.Add('--results-directory')
        $forwarded.Add((Join-Path $environmentState.LocalRoot 'test-results'))
    }
    & $dotnet @forwarded
    $exitCode = $LASTEXITCODE
} catch {
    [Console]::Error.WriteLine($_.Exception.Message)
    $exitCode = 1
} finally {
    if ($locationPushed) { Pop-Location }
    Exit-XyeLocalEnvironment -State $environmentState
}
exit $exitCode
