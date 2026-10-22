[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][ValidateSet('Initialize','Invoke','Preview')][string]$Action,
    [string]$TaskType = 'CodexTask',
    [string]$ExecutablePath,
    [string[]]$ExecutableArguments = @(),
    [string]$RequestFile,
    [switch]$AsJson,
    [string]$RepositoryRoot = (Get-Location).Path,
    [string]$SourceRoot = (Get-Location).Path,
    [string]$WorkingDirectory,
    [string]$SnapshotCommit,
    [string[]]$OverlayPaths = @(),
    [string[]]$DeletedPaths = @()
)

$ErrorActionPreference = 'Stop'
Import-Module (Join-Path $PSScriptRoot 'XyeWorkspace.psm1') -Force -DisableNameChecking
$workspace = Get-XyeWorkspaceRoot

if ($Action -eq 'Initialize') {
    Assert-XyeWorkspaceBoundary $workspace $RepositoryRoot
    $workspace = Initialize-XyeWorkspace
    Write-Output "XYE workspace ready: $workspace"
    exit 0
}
if ($Action -eq 'Preview') {
    Assert-XyeWorkspaceBoundary $workspace $RepositoryRoot
    $workspace = Initialize-XyeWorkspace
    $preview = @(Get-XyeManifestPreview $workspace)
    if ($AsJson) { $preview | ConvertTo-Json -Depth 8 }
    else { $preview | Format-Table -AutoSize }
    exit 0
}
if (-not [string]::IsNullOrWhiteSpace($RequestFile)) {
    $request = Get-Content -LiteralPath $RequestFile -Raw | ConvertFrom-Json
    if ($request.TaskType) { $TaskType = [string]$request.TaskType }
    if ($request.ExecutablePath) { $ExecutablePath = [string]$request.ExecutablePath }
    if ($request.ExecutableArguments) { $ExecutableArguments = @($request.ExecutableArguments | ForEach-Object { [string]$_ }) }
    if ($request.RepositoryRoot) { $RepositoryRoot = [string]$request.RepositoryRoot }
    if ($request.SourceRoot) { $SourceRoot = [string]$request.SourceRoot }
    if ($request.WorkingDirectory) { $WorkingDirectory = [string]$request.WorkingDirectory }
    if ($request.SnapshotCommit) { $SnapshotCommit = [string]$request.SnapshotCommit }
    if ($request.OverlayPaths) { $OverlayPaths = @($request.OverlayPaths | ForEach-Object { [string]$_ }) }
    if ($request.DeletedPaths) { $DeletedPaths = @($request.DeletedPaths | ForEach-Object { [string]$_ }) }
}
if ([string]::IsNullOrWhiteSpace($ExecutablePath)) { throw '-ExecutablePath is required for Action Invoke.' }

$RepositoryRoot = [IO.Path]::GetFullPath($RepositoryRoot)
$SourceRoot = [IO.Path]::GetFullPath($SourceRoot)
Assert-XyeWorkspaceBoundary $workspace $RepositoryRoot
foreach ($candidatePath in @($SourceRoot, $RepositoryRoot, $WorkingDirectory)) {
    if (-not [string]::IsNullOrWhiteSpace($candidatePath)) {
        Assert-XyeChildPath $RepositoryRoot $candidatePath
    }
}
$workspace = Initialize-XyeWorkspace
$runId = [Guid]::NewGuid().ToString('N')
$sourceSnapshot = Join-Path (Join-Path $workspace 'Sources') $runId
$buildDirectory = if ($TaskType -like 'dotnet-*') {
    Join-Path (Join-Path $workspace 'Builds') 'Artifacts'
} else { Join-Path (Join-Path $workspace 'Builds') $runId }
$diagnosticsDirectory = Join-Path (Join-Path $workspace 'Diagnostics') $runId
$tempDirectory = Join-Path (Join-Path $workspace 'Temp') $runId
$manifestPath = Join-Path (Join-Path $workspace 'Manifests') "$runId.json"
$commandLogPath = Join-Path $diagnosticsDirectory 'commands.jsonl'
foreach ($path in @($sourceSnapshot, $buildDirectory, $diagnosticsDirectory, $tempDirectory)) {
    Assert-XyeChildPath $workspace $path
    New-Item -ItemType Directory -Force -Path $path | Out-Null
}

$currentHead = Get-XyeGitValue $RepositoryRoot @('rev-parse','HEAD')
$baseCommit = if ([string]::IsNullOrWhiteSpace($SnapshotCommit)) { $currentHead } else { $SnapshotCommit }
$gitState = Get-XyeGitState $RepositoryRoot $baseCommit
$workspaceDrive = [IO.DriveInfo]::new([IO.Path]::GetPathRoot($workspace))
$manifest = [ordered]@{
    SchemaVersion='XYE-WORKSPACE-MANIFEST/1'; RunId=$runId; TaskType=$TaskType
    CreatedUtc=[DateTimeOffset]::UtcNow.ToString('O'); UpdatedUtc=[DateTimeOffset]::UtcNow.ToString('O')
    Status='ACTIVE'; OwnerProcessId=$PID; Hostname=[Environment]::MachineName
    WorkspaceRoot=$workspace; RepositoryRoot=$RepositoryRoot; Branch=$gitState.Branch
    CurrentHead=$currentHead; BaseCommit=$gitState.BaseCommit; BaseTree=$gitState.BaseTree
    SourceDirty=$gitState.Dirty; DirtyFiles=$gitState.DirtyFiles; DirtyOverlayDigest=$gitState.DirtyDigest
    SourceDirectory=$SourceRoot; SourceSnapshotDirectory=$null; SourceFingerprint=$null
    CandidateDirty=$false; OverlayFiles=@(); SnapshotOverlayDigest=$null; DeletedOverlayPaths=@($DeletedPaths)
    BuildDirectory=$buildDirectory; DiagnosticsDirectory=$diagnosticsDirectory; TempDirectory=$tempDirectory
    Command=$ExecutablePath; Arguments=(Protect-XyeArguments $ExecutableArguments); Commands=@(); ExitCode=$null; Error=$null
    StartedUtc=$null; CompletedUtc=$null; WorkspaceBytes=0; DirectoryBytes=@{}; OutputHashes=@()
    FreeBytesAtStart=$workspaceDrive.AvailableFreeSpace; FreeBytesAtEnd=$null
}
Save-XyeManifest $manifestPath $manifest
$exitCode = 1
$oldEnvironment = @{}
$environmentNames = @('XYE_WORKSPACE_ROOT','XYE_TASK_RUN_ID','XYE_WORKSPACE_SOURCE_DIRECTORY',
    'XYE_BUILD_DIRECTORY','XYE_DIAGNOSTICS_DIRECTORY','XYE_TEMP_DIRECTORY','XYE_PROBE_DIRECTORY',
    'XYE_TASK_MANIFEST','XYE_TASK_COMMAND_LOG','XYE_SOURCE_HEAD','XYE_SOURCE_TREE',
    'XYE_SOURCE_BRANCH','XYE_SOURCE_DIRTY','XYE_SOURCE_FINGERPRINT','TEMP','TMP',
    'DOTNET_CLI_HOME','NUGET_PACKAGES','NUGET_HTTP_CACHE_PATH','NUGET_PLUGINS_CACHE_PATH','NUGET_SCRATCH')
foreach ($name in $environmentNames) { $oldEnvironment[$name] = [Environment]::GetEnvironmentVariable($name, 'Process') }

try {
    $actualSource = $SourceRoot
    if (-not [string]::IsNullOrWhiteSpace($SnapshotCommit)) {
        $archive = Join-Path $tempDirectory 'source-snapshot.tar'
        & git -C $RepositoryRoot archive --format=tar --output=$archive $SnapshotCommit
        if ($LASTEXITCODE -ne 0) { throw "git archive failed for $SnapshotCommit" }
        & tar.exe -xf $archive -C $sourceSnapshot
        if ($LASTEXITCODE -ne 0) { throw 'Could not extract candidate source archive.' }
        $overlayRecords = Copy-XyeOverlay $SourceRoot $sourceSnapshot $OverlayPaths $DeletedPaths
        Remove-Item -LiteralPath $archive -Force
        $actualSource = $sourceSnapshot
        $manifest.SourceSnapshotDirectory = $sourceSnapshot
        $manifest.OverlayFiles = $overlayRecords
        $manifest.CandidateDirty = ($overlayRecords.Count -gt 0 -or $DeletedPaths.Count -gt 0)
        $overlayBytes = [Text.Encoding]::UTF8.GetBytes(($overlayRecords -join "`n"))
        $overlaySha = [Security.Cryptography.SHA256]::Create()
        try { $manifest.SnapshotOverlayDigest = [BitConverter]::ToString($overlaySha.ComputeHash($overlayBytes)).Replace('-', '') }
        finally { $overlaySha.Dispose() }
        $manifest.SourceFingerprint = Get-XyeTreeFingerprint $sourceSnapshot
    } else {
        $manifest.OverlayFiles = @($gitState.DirtyFiles | ForEach-Object {
            $filePath = Join-Path $SourceRoot $_
            if (Test-Path -LiteralPath $filePath -PathType Leaf) { "$_|$(Get-XyeFileHash $filePath)" }
            else { "$_|DELETED" }
        })
        $manifest.SourceFingerprint = "$($gitState.BaseTree)|$($gitState.DirtyDigest)"
    }
    if ([string]::IsNullOrWhiteSpace($WorkingDirectory)) { $WorkingDirectory = $actualSource }
    $WorkingDirectory = [IO.Path]::GetFullPath($WorkingDirectory)
    $manifest.SourceDirectory = $actualSource
    $manifest.StartedUtc = [DateTimeOffset]::UtcNow.ToString('O')
    $manifest.UpdatedUtc = $manifest.StartedUtc
    Save-XyeManifest $manifestPath $manifest

    $env:XYE_WORKSPACE_ROOT = $workspace
    $env:XYE_TASK_RUN_ID = $runId
    $env:XYE_WORKSPACE_SOURCE_DIRECTORY = $actualSource
    $env:XYE_BUILD_DIRECTORY = $buildDirectory
    $env:XYE_DIAGNOSTICS_DIRECTORY = $diagnosticsDirectory
    $env:XYE_TEMP_DIRECTORY = $tempDirectory
    $env:XYE_TASK_MANIFEST = $manifestPath
    $env:XYE_TASK_COMMAND_LOG = $commandLogPath
    $env:XYE_SOURCE_HEAD = $gitState.BaseCommit
    $env:XYE_SOURCE_TREE = $gitState.BaseTree
    $env:XYE_SOURCE_BRANCH = $gitState.Branch
    $effectiveDirty = if (-not [string]::IsNullOrWhiteSpace($SnapshotCommit)) { $manifest.CandidateDirty } else { $manifest.SourceDirty }
    $env:XYE_SOURCE_DIRTY = if ($effectiveDirty) { 'YES' } else { 'NO' }
    $env:XYE_SOURCE_FINGERPRINT = $manifest.SourceFingerprint
    $env:XYE_PROBE_DIRECTORY = $diagnosticsDirectory
    $env:TEMP = $tempDirectory
    $env:TMP = $tempDirectory
    $env:DOTNET_CLI_HOME = Join-Path (Join-Path $workspace 'Cache') 'Dotnet'
    $env:NUGET_PACKAGES = Join-Path (Join-Path (Join-Path $workspace 'Cache') 'NuGet') 'Packages'
    $env:NUGET_HTTP_CACHE_PATH = Join-Path (Join-Path (Join-Path $workspace 'Cache') 'NuGet') 'Http'
    $env:NUGET_PLUGINS_CACHE_PATH = Join-Path (Join-Path (Join-Path $workspace 'Cache') 'NuGet') 'Plugins'
    $env:NUGET_SCRATCH = Join-Path (Join-Path (Join-Path $workspace 'Cache') 'NuGet') 'Scratch'
    foreach ($cachePath in @($env:DOTNET_CLI_HOME,$env:NUGET_PACKAGES,$env:NUGET_HTTP_CACHE_PATH,$env:NUGET_PLUGINS_CACHE_PATH,$env:NUGET_SCRATCH)) {
        Assert-XyeChildPath $workspace $cachePath
        New-Item -ItemType Directory -Force -Path $cachePath | Out-Null
    }

    $stdoutPath = Join-Path $diagnosticsDirectory 'stdout.log'
    $stderrPath = Join-Path $diagnosticsDirectory 'stderr.log'
    $arguments = @($ExecutableArguments | ForEach-Object { Quote-XyeArgument ([string]$_) }) -join ' '
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = New-Object System.Diagnostics.ProcessStartInfo
    $process.StartInfo.FileName = $ExecutablePath
    $process.StartInfo.Arguments = $arguments
    $process.StartInfo.WorkingDirectory = $WorkingDirectory
    $process.StartInfo.UseShellExecute = $false
    $process.StartInfo.CreateNoWindow = $true
    $process.StartInfo.RedirectStandardOutput = $true
    $process.StartInfo.RedirectStandardError = $true
    if (-not $process.Start()) { throw "Could not start child process: $ExecutablePath" }
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    $process.WaitForExit()
    $exitCode = $process.ExitCode
    $utf8 = New-Object System.Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($stdoutPath, $stdoutTask.GetAwaiter().GetResult(), $utf8)
    [IO.File]::WriteAllText($stderrPath, $stderrTask.GetAwaiter().GetResult(), $utf8)
    $manifest.ExitCode = $exitCode
    $manifest.Status = if ($exitCode -eq 0) { 'COMPLETED' } else { 'FAILED' }
    $manifest.Commands = @([pscustomobject]@{
        Executable=$ExecutablePath; Arguments=(Protect-XyeArguments $ExecutableArguments)
        WorkingDirectory=$WorkingDirectory; StartedUtc=$manifest.StartedUtc
        CompletedUtc=[DateTimeOffset]::UtcNow.ToString('O'); ExitCode=$exitCode
    })
} catch {
    $manifest.Status = 'FAILED'
    $manifest.Error = $_.Exception.Message
    $manifest.ExitCode = $exitCode
    Write-Error $_
} finally {
    foreach ($name in $environmentNames) {
        [Environment]::SetEnvironmentVariable($name, $oldEnvironment[$name], 'Process')
    }
    if ($manifest.Status -eq 'ACTIVE') { $manifest.Status = 'INTERRUPTED' }
    if (Test-Path -LiteralPath $commandLogPath) {
        $manifest.Commands = @($manifest.Commands) + @(
            Get-Content -LiteralPath $commandLogPath | ForEach-Object { $_ | ConvertFrom-Json })
    }
    $manifest.CompletedUtc = [DateTimeOffset]::UtcNow.ToString('O')
    $manifest.UpdatedUtc = $manifest.CompletedUtc
    $manifest.DirectoryBytes = [ordered]@{
        SourceSnapshot=(Get-XyeDirectoryBytes $sourceSnapshot)
        Build=(Get-XyeDirectoryBytes $buildDirectory)
        Diagnostics=(Get-XyeDirectoryBytes $diagnosticsDirectory)
        Temp=(Get-XyeDirectoryBytes $tempDirectory)
    }
    $manifest.WorkspaceBytes = ($manifest.DirectoryBytes.Values | Measure-Object -Sum).Sum
    $manifest.FreeBytesAtEnd = ([IO.DriveInfo]::new([IO.Path]::GetPathRoot($workspace))).AvailableFreeSpace
    $manifest.OutputHashes = Get-XyeOutputHashes @($buildDirectory,$diagnosticsDirectory)
    Save-XyeManifest $manifestPath $manifest
}

if ($exitCode -ne 0) { exit $exitCode }
Write-Output "XYE task $runId $($manifest.Status); manifest=$manifestPath; bytes=$($manifest.WorkspaceBytes)"
exit 0
