[CmdletBinding()]
param([ValidateSet('Debug','Release')][string]$Configuration = 'Debug',
    [string]$Project = 'XuanYu.Engine.slnx', [switch]$RestoreCompleted)

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
$dotnet = & (Join-Path $PSScriptRoot 'resolve-dotnet.ps1')
if ($LASTEXITCODE -ne 0 -or -not $dotnet) { throw 'The SDK resolver failed.' }
$sdk = (& $dotnet --version).Trim()
if ($LASTEXITCODE -ne 0) { throw 'The resolved SDK could not run.' }
$drive = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root))
if ($drive.AvailableFreeSpace -lt 24GB) { throw "Build stopped: only $($drive.AvailableFreeSpace) bytes remain on $($drive.Name)." }

function Quote-ProcessArgument([string]$Value) {
    if ($Value -match '[\s"]') { return '"' + $Value.Replace('"','\"') + '"' }
    return $Value
}

function Invoke-MonitoredDotnet([string]$Step, [string[]]$Arguments) {
    $logRoot = Join-Path $root 'obj/xye-build-logs'
    Ensure-RepositoryDirectory -RepositoryRoot $root -Path $logRoot | Out-Null
    $run = [guid]::NewGuid().ToString('N')
    $stdout = Join-Path $logRoot "$run.stdout.log"
    $stderr = Join-Path $logRoot "$run.stderr.log"
    $line = ($Arguments | ForEach-Object { Quote-ProcessArgument ([string]$_) }) -join ' '
    $startFree = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root)).AvailableFreeSpace
    $process = Start-Process -FilePath $dotnet -ArgumentList $line -PassThru -NoNewWindow `
        -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    while (-not $process.WaitForExit(2000)) {
        $free = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root)).AvailableFreeSpace
        if ($free -lt 24GB -or $startFree - $free -gt 8GB) {
            $taskkill = Join-Path $env:SystemRoot 'System32\taskkill.exe'
            & $taskkill /PID $process.Id /T /F | Out-Null
            $process.WaitForExit()
            throw "Stopped ${Step}: free space=$free bytes; operation growth=$($startFree-$free) bytes."
        }
    }
    $process.WaitForExit()
    if (Test-Path -LiteralPath $stdout) { Get-Content -LiteralPath $stdout | ForEach-Object { [Console]::WriteLine($_) } }
    if (Test-Path -LiteralPath $stderr) { Get-Content -LiteralPath $stderr | ForEach-Object { [Console]::Error.WriteLine($_) } }
    return $process.ExitCode
}

function Get-AssetFingerprint {
    $hash = [Security.Cryptography.IncrementalHash]::CreateHash([Security.Cryptography.HashAlgorithmName]::SHA256)
    try {
        $projects = @(& git -C $root ls-files -co --exclude-standard -- '*.csproj' | Sort-Object -Unique)
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

$rootHash = [Security.Cryptography.SHA256]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes($root.ToLowerInvariant()))
$mutexName = 'Local\XYE-Canonical-Build-' + [BitConverter]::ToString($rootHash).Replace('-','').Substring(0,16)
$mutex = [Threading.Mutex]::new($false, $mutexName)
if (-not $mutex.WaitOne(0)) { throw 'Another canonical build/restore owns the shared output lock.' }
try {
    $active = @(Get-CimInstance Win32_Process -Filter "Name='dotnet.exe' OR Name='MSBuild.exe'" |
        Where-Object { $_.CommandLine -and $_.CommandLine.IndexOf($root,[StringComparison]::OrdinalIgnoreCase) -ge 0 })
    if ($active.Count) { throw "A build process already targets this repository (PID $($active[0].ProcessId))." }

    $identityFiles = @(& git -C $root ls-files -co --exclude-standard | Where-Object {
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
    $identity = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes("$sourceFingerprint`0$assetFingerprint`0$sdk`0$Configuration`0$target"))).Replace('-','')
    $manifest = Join-Path (Split-Path -Parent $target) 'obj\.xye-build-identity.json'
    $cached = if (Test-Path -LiteralPath $manifest) { Get-Content $manifest -Raw | ConvertFrom-Json } else { $null }
    $outputsValid = $false
    if ($cached -and $cached.OutputHashes) {
        $hashRecords = @($cached.OutputHashes.PSObject.Properties)
        $outputsValid = $hashRecords.Count -gt 0
        foreach ($hashRecord in $hashRecords) {
            $outputPath = Join-Path $root $hashRecord.Name
            if (-not (Test-Path -LiteralPath $outputPath -PathType Leaf) -or
                (Get-FileHash -LiteralPath $outputPath -Algorithm SHA256).Hash -ne $hashRecord.Value) {
                $outputsValid = $false
                break
            }
        }
    }
    if ($cached -and $cached.SourceFingerprint -eq $sourceFingerprint -and
        $cached.AssetFingerprint -eq $assetFingerprint -and $cached.SDK -eq $sdk -and
        $cached.Configuration -eq $Configuration -and $cached.Target -eq $target -and
        $outputsValid) {
        Write-Output "BUILD REUSED: identity=$identity SDK=$sdk Configuration=$Configuration"
        $global:LASTEXITCODE = 0
        return
    }

    if ($RestoreCompleted) {
        Write-Output "RESTORE REUSED: $target (prior restore completed successfully; no second restore started)"
    } else {
        Write-Output "RESTORE: $target"
        $restoreExit = Invoke-MonitoredDotnet 'Restore' @('restore',$target,'--configfile',(Join-Path $root 'NuGet.Config'),'--nologo')
        if ($restoreExit -ne 0) { throw "Restore failed with exit code $restoreExit." }
    }
    $assetFingerprint = Get-AssetFingerprint
    $identity = [BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash(
        [Text.Encoding]::UTF8.GetBytes("$sourceFingerprint`0$assetFingerprint`0$sdk`0$Configuration`0$target"))).Replace('-','')
    Write-Output "BUILD: $target (single shared-output owner)"
    $buildExit = Invoke-MonitoredDotnet 'Build' @('build',$target,'--no-restore','--configuration',$Configuration,'--nologo','-m:1','-nr:false','-p:UseSharedCompilation=false')
    if ($buildExit -ne 0) { throw "Build failed with exit code $buildExit." }

    $outputs = @(Get-ChildItem -LiteralPath $root -Directory -Recurse -Filter bin -ErrorAction SilentlyContinue |
        ForEach-Object { Get-ChildItem -LiteralPath $_.FullName -File -Recurse -Filter '*.dll' -ErrorAction SilentlyContinue } |
        ForEach-Object { $_.FullName.Substring($root.Length).TrimStart('\','/') })
    if ($outputs.Count -eq 0) { throw 'Build exited successfully but produced no reusable DLL outputs.' }
    $outputHashes = @{}
    foreach ($relative in $outputs) {
        $outputHashes[$relative] = (Get-FileHash -LiteralPath (Join-Path $root $relative) -Algorithm SHA256).Hash
    }
    $record = [pscustomobject]@{
        Identity=$identity; SourceFingerprint=$sourceFingerprint; AssetFingerprint=$assetFingerprint
        SDK=$sdk; Configuration=$Configuration; Target=$target; Outputs=$outputs; OutputHashes=$outputHashes
    }
    Write-BuildIdentityManifest -RepositoryRoot $root -ManifestPath $manifest -Record $record
    $after = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($root)).AvailableFreeSpace
    if ($drive.AvailableFreeSpace - $after -gt 8GB) {
        throw "Build used more than 8 GiB; investigate before continuing (delta=$($drive.AvailableFreeSpace-$after) bytes)."
    }
    Write-Output "BUILD COMPLETE: identity=$identity outputs=$($outputs.Count) freeBytes=$after"
    $global:LASTEXITCODE = 0
} finally {
    $mutex.ReleaseMutex()
    $mutex.Dispose()
}
