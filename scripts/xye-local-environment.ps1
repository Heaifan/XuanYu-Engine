function Assert-XyeLocalPath {
    param([string]$RepositoryRoot, [string]$Path)
    $root = [IO.Path]::GetFullPath($RepositoryRoot).TrimEnd([char[]]@('\','/'))
    $candidate = if ([IO.Path]::IsPathRooted($Path)) { $Path } else { Join-Path $root $Path }
    $full = [IO.Path]::GetFullPath($candidate)
    $prefix = $root + [IO.Path]::DirectorySeparatorChar
    if (-not $full.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Controlled path is outside the repository: $Path"
    }
    $current = $root
    foreach ($segment in $full.Substring($prefix.Length) -split '[\\/]+') {
        if (-not $segment) { continue }
        $current = Join-Path $current $segment
        if (Test-Path -LiteralPath $current) {
            $item = Get-Item -LiteralPath $current -Force -ErrorAction Stop
            if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
                throw "Controlled path contains a reparse point: $current"
            }
        }
    }
    return $full
}

function Enter-XyeLocalEnvironment {
    param([string]$RepositoryRoot)
    $root = [IO.Path]::GetFullPath((Resolve-Path -LiteralPath $RepositoryRoot -ErrorAction Stop).Path)
    $local = Assert-XyeLocalPath $root (Join-Path $root '.xye-local')
    $paths = [ordered]@{
        DOTNET_CLI_HOME = Join-Path $local 'dotnet'
        NUGET_PACKAGES = Join-Path $local 'nuget/packages'
        NUGET_HTTP_CACHE_PATH = Join-Path $local 'nuget/http'
        NUGET_SCRATCH = Join-Path $local 'nuget/scratch'
        NUGET_PLUGINS_CACHE_PATH = Join-Path $local 'nuget/plugins'
        TEMP = Join-Path $local 'temp'
        TMP = Join-Path $local 'temp'
    }
    foreach ($name in @($paths.Keys)) { $paths[$name] = Assert-XyeLocalPath $root $paths[$name] }
    $names = @($paths.Keys) + @('DOTNET_CLI_TELEMETRY_OPTOUT','DOTNET_SKIP_FIRST_TIME_EXPERIENCE',
        'MSBUILDDISABLENODEREUSE','DOTNET_CLI_USE_MSBUILD_SERVER','UseSharedCompilation')
    $prior = @{}
    foreach ($name in $names) { $prior[$name] = [Environment]::GetEnvironmentVariable($name, 'Process') }
    try {
        foreach ($path in @($paths.Values) + @((Join-Path $local 'logs'),(Join-Path $local 'test-results'))) {
            $safe = Assert-XyeLocalPath $root $path
            New-Item -ItemType Directory -Path $safe -Force -ErrorAction Stop | Out-Null
            Assert-XyeLocalPath $root $safe | Out-Null
        }
        foreach ($name in $paths.Keys) { [Environment]::SetEnvironmentVariable($name, $paths[$name], 'Process') }
        [Environment]::SetEnvironmentVariable('DOTNET_CLI_TELEMETRY_OPTOUT','1','Process')
        [Environment]::SetEnvironmentVariable('DOTNET_SKIP_FIRST_TIME_EXPERIENCE','1','Process')
        [Environment]::SetEnvironmentVariable('MSBUILDDISABLENODEREUSE','1','Process')
        [Environment]::SetEnvironmentVariable('DOTNET_CLI_USE_MSBUILD_SERVER','0','Process')
        [Environment]::SetEnvironmentVariable('UseSharedCompilation','false','Process')
        return [pscustomobject]@{ Previous=$prior; Root=$root; LocalRoot=$local }
    } catch {
        foreach ($name in $prior.Keys) { [Environment]::SetEnvironmentVariable($name,$prior[$name],'Process') }
        throw
    }
}

function Exit-XyeLocalEnvironment {
    param([object]$State)
    if ($null -ne $State) {
        foreach ($name in $State.Previous.Keys) {
            [Environment]::SetEnvironmentVariable($name,$State.Previous[$name],'Process')
        }
    }
}

function Assert-XyeInvocationPaths {
    param([string]$RepositoryRoot, [string[]]$Arguments)
    $pathOptions = @('--configfile','--results-directory','--diag','--logger')
    for ($i=0; $i -lt $Arguments.Count; $i++) {
        $token = [string]$Arguments[$i]
        foreach ($option in $pathOptions) {
            if ($token -eq $option -and $i + 1 -lt $Arguments.Count) {
                $value = [string]$Arguments[++$i]
                if ($option -eq '--logger') {
                    if ($value -match '(?i)(?:^|[;=])(?:[A-Z]:[\\/]|\\\\|/)' -or $value -match '(?i)(?:^|[;=])[^;]*\.\.[\\/]') {
                        throw 'Logger output paths must remain inside the repository.'
                    }
                } else { Assert-XyeLocalPath $RepositoryRoot $value | Out-Null }
                break
            }
            if ($token.StartsWith($option + '=', [StringComparison]::OrdinalIgnoreCase)) {
                Assert-XyeLocalPath $RepositoryRoot $token.Substring($option.Length + 1) | Out-Null
                break
            }
        }
        if ($token -match '(?i)^--(?:results-directory|diag|configfile):(.+)$') {
            Assert-XyeLocalPath $RepositoryRoot $Matches[1] | Out-Null
        }
        if ($token -match '(?i)^(?:-flp|/flp|-fileloggerparameters)(?::|=|$)|^(?:-bl|/bl|--binarylogger)(?::|=|$)') {
            throw 'File and binary logger outputs are disabled by the repository-local execution gate.'
        }
        if ($token -match '(?i)^(?:(?:-p|/p)(?::|=)|--property(?::|=))(.*)$') {
            foreach ($assignment in ($Matches[1] -split '[;,]')) {
                $propertyName = ($assignment -split '=',2)[0].Trim()
                if ($propertyName -in @('ArtifactsPath','BaseOutputPath','OutputPath','OutDir','PublishDir',
                    'PackageOutputPath','BaseIntermediateOutputPath','IntermediateOutputPath',
                    'MSBuildProjectExtensionsPath','RestorePackagesPath','NuGetPackageRoot',
                    'RestoreFallbackFolders','RestoreAdditionalProjectSources','VSTestResultsDirectory',
                    'TestResultsDirectory','BaseTestResultsDirectory','VSTestLogger')) {
                    throw "Output/cache path property is forbidden: $propertyName"
                }
            }
        }
    }
}
