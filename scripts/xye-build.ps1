[CmdletBinding()]
param([ValidateSet('Debug','Release')][string]$Configuration = 'Debug',
    [string]$Project = 'XuanYu.Engine.slnx', [switch]$RestoreCompleted,
    [string]$AuthorizationId)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Assert-RepositoryPath([string]$RepositoryRoot, [string]$Path) {
    $canonicalRoot = [IO.Path]::GetFullPath((Resolve-Path -LiteralPath $RepositoryRoot -ErrorAction Stop).Path).
        TrimEnd([char[]]@('\','/'))
    $fullPath = [IO.Path]::GetFullPath($Path)
    $rootPrefix = $canonicalRoot + [IO.Path]::DirectorySeparatorChar
    if (-not $fullPath.StartsWith($rootPrefix,[StringComparison]::OrdinalIgnoreCase)) {
        throw "Path is outside the repository: $Path"
    }

    $relative = $fullPath.Substring($rootPrefix.Length)
    $current = $canonicalRoot
    foreach ($segment in ($relative -split '[\\/]+')) {
        if ([string]::IsNullOrWhiteSpace($segment)) { continue }
        $current = Join-Path $current $segment
        try {
            $item = Get-Item -LiteralPath $current -Force -ErrorAction Stop
        } catch [System.Management.Automation.ItemNotFoundException] {
            continue
        } catch {
            throw "Unable to inspect repository path '$current': $($_.Exception.Message)"
        }

        if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
            throw "Repository path contains a reparse point: $current"
        }
    }
    return $fullPath
}

function Ensure-RepositoryDirectory([string]$RepositoryRoot, [string]$Path) {
    $safePath = Assert-RepositoryPath -RepositoryRoot $RepositoryRoot -Path $Path
    try {
        New-Item -ItemType Directory -Path $safePath -Force -ErrorAction Stop | Out-Null
    } catch {
        throw "Unable to create repository directory '$safePath': $($_.Exception.Message)"
    }
    return Assert-RepositoryPath -RepositoryRoot $RepositoryRoot -Path $safePath
}

function Write-BuildIdentityManifest {
    param(
        [string]$RepositoryRoot,
        [string]$ManifestPath,
        [object]$Record,
        [scriptblock]$Writer = {
            param($Path, $Content)
            Set-Content -LiteralPath $Path -Value $Content -Encoding utf8 -ErrorAction Stop
        }
    )

    $safeManifest = Assert-RepositoryPath -RepositoryRoot $RepositoryRoot -Path $ManifestPath
    $manifestDirectory = Split-Path -Parent $safeManifest
    Ensure-RepositoryDirectory -RepositoryRoot $RepositoryRoot -Path $manifestDirectory | Out-Null
    $safeManifest = Assert-RepositoryPath -RepositoryRoot $RepositoryRoot -Path $safeManifest
    $content = $Record | ConvertTo-Json -Depth 4
    try {
        & $Writer $safeManifest $content
    } catch {
        throw "Failed to write build identity manifest '$safeManifest': $($_.Exception.Message)"
    }
}

$target = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $root $Project)
if (-not (Test-Path -LiteralPath $target -PathType Leaf)) {
    throw "Build target does not exist as a file: $target"
}
$drive = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root))
if ($drive.AvailableFreeSpace -lt 24GB) { throw "Build stopped: only $($drive.AvailableFreeSpace) bytes remain on $($drive.Name)." }

$localRoot = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $root '.xye-local')
$localPaths = [ordered]@{
    DOTNET_CLI_HOME = Join-Path $localRoot 'dotnet'
    NUGET_PACKAGES = Join-Path $localRoot 'nuget/packages'
    NUGET_HTTP_CACHE_PATH = Join-Path $localRoot 'nuget/http'
    NUGET_SCRATCH = Join-Path $localRoot 'nuget/scratch'
    NUGET_PLUGINS_CACHE_PATH = Join-Path $localRoot 'nuget/plugins'
    TEMP = Join-Path $localRoot 'temp'
    TMP = Join-Path $localRoot 'temp'
}
$localBuild = Join-Path $localRoot 'build'
$localLogs = Join-Path $localRoot 'logs'
$previousEnvironment = @{}
foreach ($name in @($localPaths.Keys)) {
    $previousEnvironment[$name] = [Environment]::GetEnvironmentVariable($name, 'Process')
}
foreach ($name in @($localPaths.Keys)) {
    $path = Assert-RepositoryPath -RepositoryRoot $root -Path $localPaths[$name]
    Ensure-RepositoryDirectory -RepositoryRoot $root -Path $path | Out-Null
    $localPaths[$name] = $path
}
Ensure-RepositoryDirectory -RepositoryRoot $root -Path $localBuild | Out-Null
Ensure-RepositoryDirectory -RepositoryRoot $root -Path $localLogs | Out-Null
$previousEnvironment['DOTNET_CLI_TELEMETRY_OPTOUT'] = [Environment]::GetEnvironmentVariable('DOTNET_CLI_TELEMETRY_OPTOUT', 'Process')
$previousEnvironment['DOTNET_SKIP_FIRST_TIME_EXPERIENCE'] = [Environment]::GetEnvironmentVariable('DOTNET_SKIP_FIRST_TIME_EXPERIENCE', 'Process')
$previousEnvironment['MSBUILDDISABLENODEREUSE'] = [Environment]::GetEnvironmentVariable('MSBUILDDISABLENODEREUSE', 'Process')
$previousEnvironment['DOTNET_CLI_USE_MSBUILD_SERVER'] = [Environment]::GetEnvironmentVariable('DOTNET_CLI_USE_MSBUILD_SERVER', 'Process')
$previousEnvironment['UseSharedCompilation'] = [Environment]::GetEnvironmentVariable('UseSharedCompilation', 'Process')
try {
    [Environment]::SetEnvironmentVariable('DOTNET_CLI_TELEMETRY_OPTOUT', '1', 'Process')
    [Environment]::SetEnvironmentVariable('DOTNET_SKIP_FIRST_TIME_EXPERIENCE', '1', 'Process')
    [Environment]::SetEnvironmentVariable('MSBUILDDISABLENODEREUSE', '1', 'Process')
    [Environment]::SetEnvironmentVariable('DOTNET_CLI_USE_MSBUILD_SERVER', '0', 'Process')
    [Environment]::SetEnvironmentVariable('UseSharedCompilation', 'false', 'Process')
    foreach ($name in $localPaths.Keys) {
        [Environment]::SetEnvironmentVariable($name, $localPaths[$name], 'Process')
        $effective = [IO.Path]::GetFullPath([Environment]::GetEnvironmentVariable($name, 'Process'))
        $expected = [IO.Path]::GetFullPath($localPaths[$name])
        if (-not $effective.Equals($expected, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Controlled path mismatch for ${name}: $effective"
        }
    }
} catch {
    foreach ($name in $previousEnvironment.Keys) {
        [Environment]::SetEnvironmentVariable($name, $previousEnvironment[$name], 'Process')
    }
    throw
}

function Quote-ProcessArgument([string]$Value) {
    if ($Value -match '[\s"]') { return '"' + $Value.Replace('"','\"') + '"' }
    return $Value
}

function Write-BuildEvent {
    param(
        [string]$Stage, [string]$Status, [Nullable[int]]$ProcessId,
        [Nullable[int]]$ExitCode, [string]$ExitCodeStatus,
        [string]$Stdout, [string]$Stderr, [string]$Detail = ''
    )
    $eventPath = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $localLogs 'build-events.jsonl')
    $event = [pscustomobject]@{
        Owner = 'xye-build.ps1'; SchemaVersion = 1
        Utc = [DateTimeOffset]::UtcNow.ToString('O'); Target = $target
        AuthorizationId = $AuthorizationId; Identity = $identity
        Stage = $Stage; Status = $Status; ProcessId = $ProcessId
        ExitCode = $ExitCode; ExitCodeStatus = $ExitCodeStatus
        Stdout = $Stdout; Stderr = $Stderr; Detail = $Detail
    } | ConvertTo-Json -Compress
    if (Test-Path -LiteralPath $eventPath -PathType Leaf) {
        foreach ($line in (Get-Content -LiteralPath $eventPath)) {
            if ([string]::IsNullOrWhiteSpace($line)) { continue }
            try { $priorEvent = $line | ConvertFrom-Json } catch { throw "Build event log is unreadable; stopped: $eventPath" }
            if ($priorEvent.Owner -ne 'xye-build.ps1' -or $priorEvent.SchemaVersion -ne 1) {
                throw "Build event log contains unknown data; stopped: $eventPath"
            }
        }
    }
    Add-Content -LiteralPath $eventPath -Value $event -Encoding utf8 -ErrorAction Stop
}

function Invoke-MonitoredDotnet([string]$Step, [string[]]$Arguments) {
    $run = [guid]::NewGuid().ToString('N')
    $stdout = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $localLogs "$run.stdout.log")
    $stderr = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $localLogs "$run.stderr.log")
    if ((Test-Path -LiteralPath $stdout) -or (Test-Path -LiteralPath $stderr)) {
        throw "Log path collision; no process was started: $stdout"
    }
    $line = ($Arguments | ForEach-Object { Quote-ProcessArgument ([string]$_) }) -join ' '
    $startFree = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root)).AvailableFreeSpace
    $process = $null
    try {
        $process = Start-Process -FilePath $dotnet -ArgumentList $line -WorkingDirectory $root `
            -PassThru -NoNewWindow -ErrorAction Stop -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    } catch {
        Write-BuildEvent -Stage $Step -Status 'START_FAILED' -ProcessId $null -ExitCode $null `
            -ExitCodeStatus 'NOT_STARTED' -Stdout $stdout -Stderr $stderr -Detail $_.Exception.Message
        throw "${Step} process failed to start; no retry was attempted. See $stdout and $stderr."
    }
    if (-not $process) {
        Write-BuildEvent -Stage $Step -Status 'START_FAILED' -ProcessId $null -ExitCode $null `
            -ExitCodeStatus 'NOT_STARTED' -Stdout $stdout -Stderr $stderr -Detail 'Start-Process returned no process.'
        throw "${Step} process failed to start; no retry was attempted."
    }
    try {
        Write-BuildEvent -Stage $Step -Status 'STARTED' -ProcessId $process.Id -ExitCode $null `
            -ExitCodeStatus 'PENDING' -Stdout $stdout -Stderr $stderr
    } catch {
        $taskkill = Join-Path $env:SystemRoot 'System32\taskkill.exe'
        & $taskkill /PID $process.Id /T /F | Out-Null
        $process.WaitForExit()
        throw "${Step} process stopped because its start record could not be written; no retry was attempted."
    }

    while (-not $process.WaitForExit(2000)) {
        $free = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root)).AvailableFreeSpace
        if ($free -lt 24GB -or $startFree - $free -gt 8GB) {
            $taskkill = Join-Path $env:SystemRoot 'System32\taskkill.exe'
            & $taskkill /PID $process.Id /T /F | Out-Null
            $process.WaitForExit()
            $stoppedExitCode = $null
            try { $stoppedExitCode = [int]$process.ExitCode } catch { $stoppedExitCode = $null }
            $stoppedExitStatus = if ($null -eq $stoppedExitCode) { 'UNKNOWN' } else { 'KNOWN' }
            Write-BuildEvent -Stage $Step -Status 'STOPPED_SPACE_GATE' -ProcessId $process.Id `
                -ExitCode $stoppedExitCode -ExitCodeStatus $stoppedExitStatus -Stdout $stdout -Stderr $stderr `
                -Detail "freeBytes=$free growthBytes=$($startFree-$free)"
            throw "Stopped ${Step} on disk-space gate; exitCodeStatus=$stoppedExitStatus and no retry was attempted."
        }
    }
    try {
        $process.WaitForExit()
        if (-not $process.HasExited) { throw 'Process is not confirmed exited.' }
        $exitCode = [int]$process.ExitCode
    } catch {
        Write-BuildEvent -Stage $Step -Status 'EXIT_CODE_UNKNOWN' -ProcessId $process.Id `
            -ExitCode $null -ExitCodeStatus 'UNKNOWN' -Stdout $stdout -Stderr $stderr -Detail $_.Exception.Message
        throw "${Step} ended but its exit code is UNKNOWN; no retry was attempted. See $stdout and $stderr."
    }
    if (Test-Path -LiteralPath $stdout) { Get-Content -LiteralPath $stdout | ForEach-Object { [Console]::WriteLine($_) } }
    if (Test-Path -LiteralPath $stderr) { Get-Content -LiteralPath $stderr | ForEach-Object { [Console]::Error.WriteLine($_) } }
    Write-BuildEvent -Stage $Step -Status 'EXITED' -ProcessId $process.Id -ExitCode $exitCode `
        -ExitCodeStatus 'KNOWN' -Stdout $stdout -Stderr $stderr
    return [pscustomobject]@{ ExitCode = $exitCode; ProcessId = $process.Id; Stdout = $stdout; Stderr = $stderr }
}

function Get-AssetFingerprint {
    $hash = [Security.Cryptography.IncrementalHash]::CreateHash([Security.Cryptography.HashAlgorithmName]::SHA256)
    try {
        $projectPaths = @(& git -C $root ls-files -co --exclude-standard -- '*.csproj')
        if ($LASTEXITCODE -ne 0) { throw 'Could not enumerate project files for the asset fingerprint.' }
        $projects = @($projectPaths | Sort-Object -Unique)
        foreach ($project in $projects) {
            $asset = Join-Path (Split-Path -Parent (Join-Path $root $project)) 'obj/project.assets.json'
            if (Test-Path -LiteralPath $asset -PathType Leaf) {
                $relative = $asset.Substring($root.Length).TrimStart('\','/')
                $hash.AppendData([Text.Encoding]::UTF8.GetBytes($relative + "`0"))
                $hash.AppendData([IO.File]::ReadAllBytes($asset))
            }
        }
        return [BitConverter]::ToString($hash.GetHashAndReset()).Replace('-','')
    } finally { $hash.Dispose() }
}

function Get-RestoreInputFingerprint {
    $hash = [Security.Cryptography.IncrementalHash]::CreateHash([Security.Cryptography.HashAlgorithmName]::SHA256)
    try {
        $inputPaths = @(& git -C $root ls-files -co --exclude-standard)
        if ($LASTEXITCODE -ne 0) { throw 'Could not enumerate restore inputs.' }
        $inputs = @($inputPaths | Where-Object {
            $_ -match '\.(csproj|props|targets|config)$' -or
            $_ -match '(^|/)(global\.json|packages\.lock\.json|NuGet\.Config|Directory\.Build\.(props|targets)|Directory\.Packages\.props)$'
        })
        foreach ($relative in ($inputs | Sort-Object -Unique)) {
            $full = Join-Path $root $relative
            if (Test-Path -LiteralPath $full -PathType Leaf) {
                $hash.AppendData([Text.Encoding]::UTF8.GetBytes($relative + "`0"))
                $hash.AppendData([IO.File]::ReadAllBytes($full))
            }
        }
        return [BitConverter]::ToString($hash.GetHashAndReset()).Replace('-','')
    } finally { $hash.Dispose() }
}

$rootHash = [Security.Cryptography.SHA256]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes($root.ToLowerInvariant()))
$mutexName = 'Local\XYE-Canonical-Build-' + [BitConverter]::ToString($rootHash).Replace('-','').Substring(0,16)
$mutex = [Threading.Mutex]::new($false, $mutexName)
$mutexAcquired = $false
try {
    if (-not $mutex.WaitOne(0)) { throw 'Another canonical build/restore owns the shared output lock.' }
    $mutexAcquired = $true
    $active = @(Get-CimInstance Win32_Process -Filter "Name='dotnet.exe' OR Name='MSBuild.exe'" |
        Where-Object { $_.CommandLine -and $_.CommandLine.IndexOf($root,[StringComparison]::OrdinalIgnoreCase) -ge 0 })
    if ($active.Count) { throw "A build process already targets this repository (PID $($active[0].ProcessId))." }

    $dotnet = & (Join-Path $PSScriptRoot 'resolve-dotnet.ps1')
    if ($LASTEXITCODE -ne 0 -or -not $dotnet) { throw 'The SDK resolver failed; no Restore/Build was started.' }
    $sdkOutput = @(& $dotnet --version 2>&1)
    $sdkExitCode = $LASTEXITCODE
    if ($sdkExitCode -ne 0 -or @($sdkOutput).Count -eq 0) {
        throw "The resolved SDK version is UNKNOWN (exit=$sdkExitCode); no Restore/Build was started."
    }
    $sdk = ($sdkOutput -join "`n").Trim()

    $identityPaths = @(& git -C $root ls-files -co --exclude-standard)
    if ($LASTEXITCODE -ne 0) { throw 'Could not enumerate source identity inputs; stopped without Restore/Build.' }
    $identityFiles = @($identityPaths | Where-Object {
        $_ -match '\.(cs|axaml|resx|csproj|props|targets|slnx|config|editorconfig)$' -or
        $_ -match '(^|/)(global\.json|packages\.lock\.json|NuGet\.Config|Directory\.Build\.(props|targets)|Directory\.Packages\.props)$'
    })
    $sha = [Security.Cryptography.IncrementalHash]::CreateHash([Security.Cryptography.HashAlgorithmName]::SHA256)
    foreach ($relative in ($identityFiles | Sort-Object -Unique)) {
        $full = Join-Path $root $relative
        if (Test-Path -LiteralPath $full -PathType Leaf) {
            $bytes = [Text.Encoding]::UTF8.GetBytes($relative + "`0")
            $sha.AppendData($bytes)
            $sha.AppendData([IO.File]::ReadAllBytes($full))
        }
    }
    $sourceFingerprint = [BitConverter]::ToString($sha.GetHashAndReset()).Replace('-','')
    $assetFingerprint = Get-AssetFingerprint
    $restoreArguments = @('restore',$target,'--configfile',(Join-Path $root 'NuGet.Config'),'--nologo',
        '-p:RestoreFallbackFolders=')
    $restoreArgumentsFingerprint = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes(($restoreArguments -join "`0")))).Replace('-','')
    $restoreInputFingerprint = Get-RestoreInputFingerprint
    $restoreIdentity = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes("$restoreInputFingerprint`0$dotnet`0$sdk`0$target`0$restoreArgumentsFingerprint"))).Replace('-','')
    $restoreStatePath = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $localBuild 'restore-state.json')
    $restoreState = if (Test-Path -LiteralPath $restoreStatePath -PathType Leaf) {
        Get-Content -LiteralPath $restoreStatePath -Raw | ConvertFrom-Json
    } else { $null }
    if ($restoreState -and ($restoreState.Owner -ne 'xye-build.ps1' -or $restoreState.SchemaVersion -ne 1)) {
        throw "Restore state contains unknown data; stopped without Restore/Build: $restoreStatePath"
    }
    $restoreReusable = $restoreState -and $restoreState.Status -eq 'SUCCEEDED' -and
        $restoreState.Identity -eq $restoreIdentity -and $restoreState.AssetFingerprint -eq $assetFingerprint
    if ($RestoreCompleted -and -not $restoreReusable) {
        throw 'RestoreCompleted was requested without a matching successful restore record; stopped without Restore/Build.'
    }
    $buildArguments = @('build',$target,'--no-restore','--configuration',$Configuration,'--nologo','-m:1','-nr:false','-p:UseSharedCompilation=false')
    $buildArgumentsFingerprint = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes(($buildArguments -join "`0")))).Replace('-','')
    $identity = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes("$sourceFingerprint`0$assetFingerprint`0$dotnet`0$sdk`0$Configuration`0$target`0$buildArgumentsFingerprint"))).Replace('-','')
    $manifest = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $localBuild 'build-identity.json')
    $legacyManifest = Join-Path (Split-Path -Parent $target) 'obj\.xye-build-identity.json'
    $hasLocalManifest = Test-Path -LiteralPath $manifest -PathType Leaf
    $manifestToRead = if ($hasLocalManifest) { $manifest } else { $legacyManifest }
    $cached = if (Test-Path -LiteralPath $manifestToRead -PathType Leaf) { Get-Content -LiteralPath $manifestToRead -Raw | ConvertFrom-Json } else { $null }
    if ($hasLocalManifest -and ($cached.Owner -ne 'xye-build.ps1' -or $cached.SchemaVersion -ne 2)) {
        throw "Build identity manifest contains unknown data; stopped without Restore/Build: $manifest"
    }
    $outputsValid = $false
    if ($cached -and $cached.OutputHashes) {
        $hashRecords = @($cached.OutputHashes.PSObject.Properties)
        $outputsValid = $hashRecords.Count -gt 0
        foreach ($hashRecord in $hashRecords) {
            $outputPath = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $root $hashRecord.Name)
            if (-not (Test-Path -LiteralPath $outputPath -PathType Leaf) -or
                (Get-FileHash -LiteralPath $outputPath -Algorithm SHA256).Hash -ne $hashRecord.Value) {
                $outputsValid = $false
                break
            }
        }
    }
    $cachedSdkPathMatches = -not $cached.SDKPath -or $cached.SDKPath -eq $dotnet
    $identityMatches = $cached -and $cached.SourceFingerprint -eq $sourceFingerprint -and
        $cached.AssetFingerprint -eq $assetFingerprint -and $cached.SDK -eq $sdk -and $cachedSdkPathMatches -and
        $cached.Configuration -eq $Configuration -and $cached.Target -eq $target -and
        (!$cached.BuildArgumentsFingerprint -or $cached.BuildArgumentsFingerprint -eq $buildArgumentsFingerprint)
    if ($identityMatches -and -not $outputsValid) {
        throw "Cached output identity is invalid for $target; stopped without Restore/Build or retry. Inspect $manifestToRead."
    }
    if ($identityMatches -and $outputsValid) {
        Write-Output "BUILD REUSED: identity=$identity SDK=$sdk Configuration=$Configuration"
        Write-BuildEvent -Stage 'Build' -Status 'CACHE_REUSED' -ProcessId $null -ExitCode 0 `
            -ExitCodeStatus 'NOT_RUN' -Stdout '' -Stderr ''
        $global:LASTEXITCODE = 0
        return
    }

    if ([string]::IsNullOrWhiteSpace($AuthorizationId) -or $AuthorizationId -notmatch '^[A-Za-z0-9][A-Za-z0-9._-]{2,127}$') {
        throw 'No valid one-time AuthorizationId was supplied; no Restore/Build was started.'
    }
    $ledger = Assert-RepositoryPath -RepositoryRoot $root -Path (Join-Path $localBuild 'build-authorizations.jsonl')
    if (Test-Path -LiteralPath $ledger -PathType Leaf) {
        foreach ($line in (Get-Content -LiteralPath $ledger)) {
            if ([string]::IsNullOrWhiteSpace($line)) { continue }
            try { $prior = $line | ConvertFrom-Json } catch { throw "Authorization ledger is unreadable; stopped without Restore/Build: $ledger" }
            if ($prior.Owner -ne 'xye-build.ps1' -or $prior.SchemaVersion -ne 1) {
                throw "Authorization ledger contains unknown data; stopped without Restore/Build: $ledger"
            }
            if ($prior.AuthorizationId -eq $AuthorizationId) {
                throw "AuthorizationId was already consumed; stopped without Restore/Build: $AuthorizationId"
            }
        }
    }
    $authorization = [pscustomobject]@{
        Owner = 'xye-build.ps1'; SchemaVersion = 1
        Utc = [DateTimeOffset]::UtcNow.ToString('O'); AuthorizationId = $AuthorizationId
        Target = $target; Identity = $identity; Status = 'CONSUMED_BEFORE_EXECUTION'
    } | ConvertTo-Json -Compress
    Add-Content -LiteralPath $ledger -Value $authorization -Encoding utf8 -ErrorAction Stop
    Write-BuildEvent -Stage 'Authorization' -Status 'CONSUMED' -ProcessId $null -ExitCode $null `
        -ExitCodeStatus 'NOT_STARTED' -Stdout '' -Stderr '' -Detail $AuthorizationId

    if ($restoreReusable) {
        Write-Output "RESTORE REUSED: $target (matching successful restore identity=$restoreIdentity)"
        Write-BuildEvent -Stage 'Restore' -Status 'CACHE_REUSED' -ProcessId $null -ExitCode 0 `
            -ExitCodeStatus 'NOT_RUN' -Stdout '' -Stderr '' -Detail $restoreIdentity
    } else {
        Write-Output "RESTORE: $target"
        $restoreRecord = [pscustomobject]@{
            Owner='xye-build.ps1'; SchemaVersion=1; Status='IN_PROGRESS'; Identity=$restoreIdentity
            RestoreInputFingerprint=$restoreInputFingerprint; SDK=$sdk; SDKPath=$dotnet
            Target=$target; RestoreArgumentsFingerprint=$restoreArgumentsFingerprint; AssetFingerprint=$assetFingerprint
        }
        Write-BuildIdentityManifest -RepositoryRoot $root -ManifestPath $restoreStatePath -Record $restoreRecord
        $restoreResult = Invoke-MonitoredDotnet 'Restore' $restoreArguments
        if ($restoreResult.ExitCode -ne 0) { throw "Restore failed with exit code $($restoreResult.ExitCode); no retry was attempted." }
        $assetFingerprint = Get-AssetFingerprint
        $restoreRecord.Status = 'SUCCEEDED'
        $restoreRecord.AssetFingerprint = $assetFingerprint
        Write-BuildIdentityManifest -RepositoryRoot $root -ManifestPath $restoreStatePath -Record $restoreRecord
    }
    $assetFingerprint = Get-AssetFingerprint
    $identity = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes("$sourceFingerprint`0$assetFingerprint`0$dotnet`0$sdk`0$Configuration`0$target`0$buildArgumentsFingerprint"))).Replace('-','')
    Write-Output "BUILD: $target (single shared-output owner)"
    $buildResult = Invoke-MonitoredDotnet 'Build' $buildArguments
    if ($buildResult.ExitCode -ne 0) { throw "Build failed with exit code $($buildResult.ExitCode); no retry was attempted." }

    $outputs = @(Get-ChildItem -LiteralPath $root -Directory -Recurse -Filter bin -ErrorAction SilentlyContinue |
        ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -File -Recurse -Filter '*.dll' -ErrorAction SilentlyContinue } |
        ForEach-Object { $_.FullName.Substring($root.Length).TrimStart('\','/') })
    if ($outputs.Count -eq 0) { throw 'Build exited successfully but produced no reusable DLL outputs.' }
    $outputHashes = @{}
    foreach ($relative in $outputs) {
        $outputHashes[$relative] = (Get-FileHash -LiteralPath (Join-Path $root $relative) -Algorithm SHA256).Hash
    }
    $record = [pscustomobject]@{
        Owner='xye-build.ps1'; SchemaVersion=2
        Identity=$identity; SourceFingerprint=$sourceFingerprint; AssetFingerprint=$assetFingerprint
        SDK=$sdk; SDKPath=$dotnet; Configuration=$Configuration; Target=$target; BuildArgumentsFingerprint=$buildArgumentsFingerprint
        Outputs=$outputs; OutputHashes=$outputHashes
    }
    Write-BuildIdentityManifest -RepositoryRoot $root -ManifestPath $manifest -Record $record
    $after = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root)).AvailableFreeSpace
    if ($drive.AvailableFreeSpace - $after -gt 8GB) {
        throw "Build used more than 8 GiB; investigate before continuing (delta=$($drive.AvailableFreeSpace-$after) bytes)."
    }
    Write-Output "BUILD COMPLETE: identity=$identity outputs=$($outputs.Count) freeBytes=$after"
    $global:LASTEXITCODE = 0
} finally {
    if ($mutexAcquired) { $mutex.ReleaseMutex() }
    $mutex.Dispose()
    foreach ($name in $previousEnvironment.Keys) {
        [Environment]::SetEnvironmentVariable($name, $previousEnvironment[$name], 'Process')
    }
}
